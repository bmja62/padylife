using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Visits;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services;
using Services.Hubs;
using Services.Services.GeoLocationServices;
using Services.Services.IpServices;
using Services.Services.OnlineUsersServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Visits.Commands
{
    public class TrackVisitCommand : ICommand<ServiceResult>
    {
        public TrackVisitDTO Dto { get; }
        public string UserIp { get; }
        public string UserAgent { get; }
        public string SessionId { get; }
        public long? UserId { get; }
        public bool IsLoggedIn { get; }

        public TrackVisitCommand(TrackVisitDTO dto, string userIp, string userAgent, string sessionId, long? userId = null)
        {
            Dto = dto;
            UserIp = userIp;
            UserAgent = userAgent;
            SessionId = sessionId;
            UserId = userId;
            IsLoggedIn = userId.HasValue;
        }
    }

    public class TrackVisitCommandValidator : AbstractValidator<TrackVisitCommand>
    {
        public TrackVisitCommandValidator()
        {
            RuleFor(x => x.Dto.PageUrl)
                .NotEmpty().WithMessage("آدرس صفحه الزامی است")
                .MaximumLength(500).WithMessage("آدرس صفحه نمی‌تواند بیش از 500 کاراکتر باشد");

            RuleFor(x => x.UserIp)
                .NotEmpty().WithMessage("IP کاربر الزامی است");

            RuleFor(x => x.UserAgent)
                .NotEmpty().WithMessage("User Agent الزامی است");
        }
    }

    public class TrackVisitCommandHandler : ICommandHandler<TrackVisitCommand, ServiceResult>
    {
        private readonly IRepository<PageVisit> _pageVisitRepository;
        private readonly ISecureIpService _secureIpService;
        private readonly IGeoLocationService _geoLocationService;
        private readonly IOnlineUsersService _onlineUsersService;
        private readonly ILogger<TrackVisitCommandHandler> _logger;


        public TrackVisitCommandHandler(
            IRepository<PageVisit> pageVisitRepository,
            ISecureIpService secureIpService,
            ILogger<TrackVisitCommandHandler> logger,
            IGeoLocationService geoLocationService
,
            IOnlineUsersService onlineUsersService)
        {
            _pageVisitRepository = pageVisitRepository;
            _secureIpService = secureIpService;
            _logger = logger;
            _geoLocationService = geoLocationService;
            _onlineUsersService = onlineUsersService;
        }

        public async Task<ServiceResult> Handle(TrackVisitCommand request, CancellationToken cancellationToken)
        {
            // پردازش امن IP
            var processedIp = _secureIpService.ProcessIpForStorage(request.UserIp);
            var hashedIpForTracking = _secureIpService.ProcessIpForTracking(request.UserIp);

            // بررسی بازدید تکراری در 30 دقیقه گذشته
            var thirtyMinutesAgo = DateTime.UtcNow.AddMinutes(-30);

            var existingVisitQuery = _pageVisitRepository.TableNoTracking
                .Where(v => v.PageUrl == request.Dto.PageUrl &&
                           v.VisitDate >= thirtyMinutesAgo);

            // دریافت اطلاعات جغرافیایی
            var geoLocation = await _geoLocationService.GetLocationFromIpAsync(request.UserIp, request.UserAgent);

            // اگر کاربر لاگین شده، بر اساس UserId چک کن
            if (request.UserId.HasValue)
            {
                existingVisitQuery = existingVisitQuery.Where(v => v.UserId == request.UserId);
            }
            else
            {
                // اگر کاربر لاگین نشده، بر اساس SessionId و IP پردازش شده چک کن
                existingVisitQuery = existingVisitQuery.Where(v =>
                    v.SessionId == request.SessionId ||
                    v.ProcessedUserIp == processedIp);
            }

            var isUnique = !await existingVisitQuery.AnyAsync(cancellationToken);

            // ایجاد بازدید جدید
            var visit = new PageVisit
            {
                PageUrl = request.Dto.PageUrl,
                PageTitle = request.Dto.PageTitle,
                UserIp = request.UserIp, // IP اصلی
                ProcessedUserIp = processedIp, // IP ناشناس شده
                HashedUserIp = hashedIpForTracking, // IP هش شده
                UserAgent = request.UserAgent,
                SessionId = request.SessionId,
                UserId = request.UserId,
                IsLoggedIn = request.IsLoggedIn,
                IsUniqueVisit = isUnique,
                VisitDate = DateTime.UtcNow,
                Referrer = request.Dto.Referrer,
                IsLocalIp = _secureIpService.IsLocalIp(request.UserIp),
                Country = geoLocation.Country,
                City = geoLocation.City,
                Region = geoLocation.Region
            };

            // تشخیص خودکار موجودیت
            visit.AutoDetectEntity();

            // تشخیص اطلاعات دستگاه (اگر متد موجود باشد)
            try
            {
                visit.DetectDeviceInfo();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Device detection failed for user agent: {UserAgent}", request.UserAgent);
            }

            await _pageVisitRepository.AddAsync(visit, cancellationToken);

            _logger.LogInformation("Visit tracked - URL: {Url}, User: {UserId}, Unique: {IsUnique}, Session: {SessionId}",
                request.Dto.PageUrl, request.UserId, isUnique, request.SessionId);

            // 🔥 ارسال به سیستم Real-time - نسخه اصلاح شده
            try
            {
                await _onlineUsersService.UpdateUserActivityAsync(
                    request.UserId?.ToString() ?? request.SessionId,
                    request.Dto.PageUrl,
                    request.Dto.PageTitle,
                    request.UserAgent,
                    request.UserIp
                );

                _logger.LogDebug("Real-time activity updated for user: {UserId}", request.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update real-time activity for user");
            }

            return ServiceResult.Ok();
        }
    }
}