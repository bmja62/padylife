using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.Users.Commands;
using Application.Users.DTOs;
using Application.Users.Queries;
using Asp.Versioning;
using Common;
using Common.Exceptions;
using Common.GridResults;
using Common.Roles;
using Common.Utilities;
using Common.Utilities.Helpers;
using Data.Contracts;
using Data.Repositories.Extentions;
using Entities.Plans;
using Entities.Questions;
using Entities.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PadyLife.Api.Models.Users;
using PadyLife.Api.Models.Users.Extentions;
using Services;
using Services.Services.AuthServices;
using Services.Services.JwtServices;
using Services.Services.SmsStrategy.SmsServices;
using Services.Services.Visits.DTOs;
using Services.Services.WalletsServices;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر کاربران
    /// </summary>
    [ApiVersion("1")]
    public class UsersController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        IWalletService walletService,
        IUserRepository userRepository,
        ILogger<UsersController> logger,
        IJwtService jwtService,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        SignInManager<User> signInManager,
        IHttpContextAccessor accessor,
        IRepository<UserPlan> userPlanRepository,
        IRepository<UserPlanAnswer> userPlanAnswerRepository,
        IRepository<QuestionLinked> questionLinkRepository,
        IRepository<PlanQuestion> planQuestionRepository,
        IGoogleLoginService googleLoginService,
        IDistributedCache _distributedCache,
        ISmsService _smsService
        ) : BaseController
    {


        /// <summary>
        /// دریافت اطلاعات کاربر احراز شده
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public virtual async Task<ApiResult<UserDTO>> GetByToken(CancellationToken cancellationToken)
        => (ServiceResult.Ok(
            await userRepository.TableNoTracking.Where(a => a.Id == accessor.HttpContext.User.Identity.GetUserId<long>())
            .SelectUsers(
             userPlanRepository.Table,
            userPlanAnswerRepository.Table,
            questionLinkRepository.Table,
            planQuestionRepository.Table,
            true)
            .FirstOrDefaultAsync()
        )).ToApiResult();




        /// <summary>
        /// دریافت لیست کاربران
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="count"></param>
        /// <param name="isActive"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.Specialist}")]
        public virtual async Task<ApiResult<GlobalGridResult<UserDTO>>> Get(int pageNumber, int count, string roleName, string Search, CancellationToken cancellationToken, bool? isActive = null,bool? onlyExpertUser = false) =>
            (ServiceResult.Ok(new GlobalGridResult<UserDTO>
            {
                Data = await userRepository.TableNoTracking
                    .ApplyDefaultInclude()
                      .Where(t => (onlyExpertUser.HasValue && onlyExpertUser.Value) ? userPlanRepository.Table.Where(a => a.Experts.Any(e => e.ExpertId == accessor.HttpContext.User.Identity.GetUserId<long>())).Select(u => u.UserId).ToList().Contains(t.Id)  : true)
                      .Where(t => isActive.HasValue ? t.IsActive == isActive.Value : true)
                      .Where(t => !string.IsNullOrEmpty(Search) ? t.UserName.Contains(Search) ||
                t.PhoneNumber.Contains(Search) ||
                t.FullName.Contains(Search) : true)
                      .Where(t => !string.IsNullOrEmpty(roleName) ? t.UserRoles.Any(a => a.Role.Name.Contains(roleName)) : true)
                      .ApplyPagination(pageNumber, count)
                      .SelectUsers(
                          userPlanRepository.Table,
                          userPlanAnswerRepository.Table,
                          questionLinkRepository.Table,
                          planQuestionRepository.Table,
                          false)
                      .ToListAsync(cancellationToken),
                TotalCount = await userRepository.TableNoTracking
                    .ApplyDefaultInclude()
                    .Where(t => (onlyExpertUser.HasValue && onlyExpertUser.Value) ? userPlanRepository.Table.Where(a => a.Experts.Any(e => e.ExpertId == accessor.HttpContext.User.Identity.GetUserId<long>())).Select(u => u.UserId).ToList().Contains(t.Id) : true)
                    .Where(t => isActive.HasValue ? t.IsActive == isActive.Value : true)
                    .Where(t => !string.IsNullOrEmpty(roleName) ? t.UserRoles.Any(a => a.Role.Name.Contains(roleName)) : true)
                    .CountAsync(cancellationToken),
            })).ToApiResult();


        /// <summary>
        /// دریافت کاربر با شناسه
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.User},{UserRoleNames.Specialist}")]
        public virtual async Task<ApiResult<UserDTO>> Get(long id, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(cancellationToken, id);
            if (user == null)
                return NotFound();

            await userManager.UpdateSecurityStampAsync(user);
            return (ServiceResult.Ok(await userRepository.TableNoTracking
                .ApplyDefaultInclude()
                .Where(a => a.Id == id)
                .SelectUsers(
            userPlanRepository.Table,
            userPlanAnswerRepository.Table,
            questionLinkRepository.Table,
            planQuestionRepository.Table,
            true)
                .FirstOrDefaultAsync()).ToApiResult());
        }

        /// <summary>
        /// دربافت توکن
        /// </summary>
        /// <param name="tokenRequest">The information of token request</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<ApiResult<TokenResponseDTO>> Token([FromBody] TokenRequest tokenRequest, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByNameAsync(tokenRequest.username) ?? throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");
            var isPasswordValid = await userManager.CheckPasswordAsync(user, tokenRequest.password);
            if (!isPasswordValid)
                throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");

            var jwt = await jwtService.GenerateAsync(user);
            var userDTo = await userRepository.Table
                       .ApplyDefaultInclude()
                       .Where(x => x.Id == user.Id)
                           .SelectUsers(
                                userPlanRepository.Table,
                                userPlanAnswerRepository.Table,
                                questionLinkRepository.Table,
                                planQuestionRepository.Table,
                                true)
                       .FirstOrDefaultAsync();

            // مدیریت Session پس از لاگین موفق
            await ManageUserSessionAsync(user);

            var tokenReponse = new TokenResponseDTO
            {
                AccessToken = jwt,
                User = userDTo
            };
            return new ApiResult<TokenResponseDTO>(true, ApiResultStatusCode.Success, tokenReponse);
        }
        private async Task ManageUserSessionAsync(User user)
        {
            try
            {
                // ایجاد یا آپدیت Session
                string sessionId;
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("VisitorId")))
                {
                    sessionId = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("VisitorId", sessionId);
                }
                else
                {
                    sessionId = HttpContext.Session.GetString("VisitorId");
                }

                // ذخیره اطلاعات کاربر در Session
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Username", user.UserName);
                HttpContext.Session.SetString("UserEmail", user.Email ?? string.Empty);
                HttpContext.Session.SetString("LoginTime", DateTime.UtcNow.ToString("o"));

                // لاگ کردن اطلاعات Session
                logger.LogInformation($"User {user.UserName} (ID: {user.Id}) logged in with Session: {sessionId}");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Session management failed for user {UserId}", user.Id);
                // ادامه عملیات حتی اگر Session با خطا مواجه شد
            }
        }

        /// <summary>
        /// ویرایش کاربر
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public virtual async Task<ApiResult> Update(long id, UpdateUserDTO user, CancellationToken cancellationToken)
        {
            var updateUser = await userRepository.GetByIdAsync(cancellationToken, id);

            updateUser.SetFullName(user.FullName);
            updateUser.SetAge(user.Age);
            updateUser.SetGender(user.Gender);
            updateUser.SetIsActive(user.IsActive);
            updateUser.SetBirthdate(user.Birthdate);
            updateUser.SetHight(user.Hight);
            updateUser.SetWight(user.Wight);
            updateUser.SetJobTitle(user.JobTitle);
            updateUser.SetMaritalStatus(user.MaritalStatus);
            updateUser.SetInstagramId(user.InstagramId);
            updateUser.SetProfileImage(user.ProfileImage);

            await userManager.UpdateAsync(updateUser);

            return Ok();
        }

        /// <summary>
        /// حذف کاربر
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        public virtual async Task<ApiResult> Delete(long id, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(cancellationToken, id);
            await userRepository.DeleteAsync(user, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// فعال / غیرفعال کردن کاربر
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ApiResult> ToggleIsActive(long id, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(cancellationToken, id);
            user.SetIsActive(!user.IsActive);
            await userRepository.UpdateAsync(user, cancellationToken);

            return Ok();
        }
        /// <summary>
        /// ارسال کد تأیید به شماره موبایل برای ثبت‌نام
        /// </summary>
        [HttpPost("send-verification-code")]
        [AllowAnonymous]
        public virtual async Task<ApiResult<string>> SendOTPCodeToPhoneNumber([FromBody] SendVerificationCodeRequest request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var logId = Guid.NewGuid().ToString().Substring(0, 8);

            logger.LogInformation("[SendOTPCode:{LogId}] شروع ارسال کد تأیید برای: {PhoneNumber}", logId, request.PhoneNumber);

            // نرمال سازی شماره تلفن
            var normalizedPhone = NormalizePhoneNumber(request.PhoneNumber);

            // بررسی وجود کاربر با این شماره
            if (await userRepository.Table.AnyAsync(t => t.PhoneNumber.Equals(normalizedPhone), cancellationToken))
            {
                logger.LogWarning("[SendOTPCode:{LogId}] شماره موبایل تکراری: {PhoneNumber}", logId, normalizedPhone);
                return ServiceResult.BadRequest<string>("شماره موبایل تکراری است").ToApiResult();
            }

            // بررسی تعداد درخواست‌های اخیر (Rate Limiting)
            var rateLimitKey = $"Register_Verification_RateLimit_{normalizedPhone}";
            var recentAttempts = await GetRecentAttemptsCount(rateLimitKey);

            if (recentAttempts >= 3)
            {
                logger.LogWarning("[SendOTPCode:{LogId}] تعداد درخواست‌های بیش از حد برای: {PhoneNumber}", logId, normalizedPhone);
                return ServiceResult.Fail<string>("تعداد درخواست‌های شما بیش از حد مجاز است. لطفاً 15 دقیقه دیگر تلاش کنید.").ToApiResult();
            }

            // تولید کد 6 رقمی
            var verificationCode = GenerateResetCode();
            var cacheKey = $"Register_Verification:{normalizedPhone}";

            try
            {
                // ذخیره کد در Redis به مدت 10 دقیقه
                await SaveVerificationCodeToRedis(cacheKey, verificationCode);

                // افزایش شمارنده درخواست‌ها
                await IncrementAttemptsCount(rateLimitKey);

                logger.LogInformation("[SendOTPCode:{LogId}] کد تأیید برای شماره {PhoneNumber} ایجاد شد", logId, normalizedPhone);

                // ارسال SMS
                await SendVerificationCodeSms(normalizedPhone, verificationCode);

                sw.Stop();
                logger.LogInformation("[SendOTPCode:{LogId}] ارسال کد تأیید با موفقیت انجام شد - Duration: {ElapsedMs}ms", logId, sw.ElapsedMilliseconds);

                return ServiceResult.Ok<string>("کد تأیید با موفقیت ارسال شد").ToApiResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SendOTPCode:{LogId}] خطا در ارسال کد تأیید", logId);
                return ServiceResult.Fail<string>("خطا در ارسال کد تأیید").ToApiResult();
            }
        }

        /// <summary>
        /// ثبت‌نام کاربر با کد تأیید
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<ApiResult<string>> Register([FromBody] RegisterDTO input, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var logId = Guid.NewGuid().ToString().Substring(0, 8);

            logger.LogInformation("[Register:{LogId}] شروع فرآیند ثبت‌نام برای: {PhoneNumber}", logId, input.PhoneNumber);

            // اعتبارسنجی اولیه
            if (!input.UserName.ValidateUsername())
                return ServiceResult.BadRequest<string>("فقط حروف انگلیسی، اعداد و زیرخط (_) برای نام کاربری مجاز هستند").ToApiResult();

            // نرمال سازی شماره تلفن
            var normalizedPhone = NormalizePhoneNumber(input.PhoneNumber);

            // بررسی تکراری نبودن شماره موبایل
            if (await userRepository.Table.AnyAsync(t => t.PhoneNumber.Equals(normalizedPhone), cancellationToken))
                return ServiceResult.BadRequest<string>("شماره موبایل تکراری است").ToApiResult();

            // بررسی کد تأیید
            var cacheKey = $"Register_Verification:{normalizedPhone}";
            var cachedCode = await GetVerificationCodeFromRedis(cacheKey);

            if (cachedCode == null || cachedCode != input.VerificationCode)
            {
                logger.LogWarning("[Register:{LogId}] کد تأیید نامعتبر یا منقضی شده برای: {PhoneNumber}", logId, normalizedPhone);
                return ServiceResult.BadRequest<string>("کد تأیید نامعتبر یا منقضی شده است").ToApiResult();
            }

            try
            {
                if (input.Type == UserType.User)
                {
                    User newUser = Entities.Users.User.RegisterUser(input.UserName, normalizedPhone, input.Email);
                    newUser.SetIsActive(true);
                    var response = await userManager.CreateAsync(newUser, input.Password);
                    if (!response.Succeeded)
                    {
                        var errors = string.Join(',', response.Errors.Select(a => a.Description).ToArray());
                        logger.LogError("[Register:{LogId}] خطا در ایجاد کاربر - Errors: {Errors}", logId, errors);
                        return ServiceResult.Fail<string>(errors).ToApiResult();
                    }
                    await userManager.AddToRoleAsync(newUser, UserRoles.User.Value);
                    await walletService.GetOrCreateByUserId(newUser.Id);

                    logger.LogInformation("[Register:{LogId}] کاربر معمولی با موفقیت ایجاد شد - UserId: {UserId}", logId, newUser.Id);
                }

                if (input.Type == UserType.Specialist)
                {
                    var specialistUser = Expert.RegisterExpert(input.UserName, normalizedPhone, input.Email);
                    specialistUser.SetIsActive(false);
                    Expert expertUser = specialistUser as Expert;
                    var response = await userManager.CreateAsync(expertUser, input.Password);
                    if (!response.Succeeded)
                    {
                        var errors = string.Join(',', response.Errors.Select(a => a.Description).ToArray());
                        logger.LogError("[Register:{LogId}] خطا در ایجاد متخصص - Errors: {Errors}", logId, errors);
                        return ServiceResult.Fail<string>(errors).ToApiResult();
                    }

                    await userManager.AddToRoleAsync(specialistUser, UserRoles.Specialist.Value);
                    await walletService.GetOrCreateByUserId(specialistUser.Id);

                    logger.LogInformation("[Register:{LogId}] کاربر متخصص با موفقیت ایجاد شد - UserId: {UserId}", logId, specialistUser.Id);
                }

                // پاک کردن کد تأیید از Redis پس از استفاده موفق
                await RemoveVerificationCodeFromRedis(cacheKey);

                // پاک کردن شمارنده درخواست‌ها
                await RemoveRateLimit($"Register_Verification_RateLimit_{normalizedPhone}");

                sw.Stop();
                logger.LogInformation("[Register:{LogId}] ثبت‌نام با موفقیت انجام شد - Duration: {ElapsedMs}ms", logId, sw.ElapsedMilliseconds);

                return ServiceResult.Ok<string>("ثبت‌نام با موفقیت انجام شد").ToApiResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[Register:{LogId}] خطا در فرآیند ثبت‌نام", logId);
                return ServiceResult.Fail<string>("خطا در فرآیند ثبت‌نام").ToApiResult();
            }
        }

        /// <summary>
        /// افزودن نقش ها به کاربر
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<ApiResult<string>> AddRoleToUser([FromBody] AddRoleToUserDTO input, CancellationToken cancellationToken)
        {

            List<string> InvalidRoles = new();
            var userInDb = await userRepository.GetByIdAsync(cancellationToken, input.UserId);
            var identityResult = await userManager.AddToRolesAsync(userInDb, input.Roles);
            if (identityResult.Succeeded)
            {
                await userManager.UpdateSecurityStampAsync(userInDb);
                return (ServiceResult.Ok<string>(null)).ToApiResult();
            }

            var errors = string.Join(',', identityResult.Errors);
            return (ServiceResult.Ok<string>(errors)).ToApiResult();
        }


        /// <summary>
        /// حذف نقش ها از کاربر
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        [AllowAnonymous]
        public virtual async Task<ApiResult<string>> RemoveFromRolesUser([FromBody] AddRoleToUserDTO input, CancellationToken cancellationToken)
        {

            List<string> InvalidRoles = new();
            var userInDb = await userRepository.GetByIdAsync(cancellationToken, input.UserId);
            var identityResult = await userManager.RemoveFromRolesAsync(userInDb, input.Roles);

            if (identityResult.Succeeded)
            {
                await userManager.UpdateSecurityStampAsync(userInDb);
                return (ServiceResult.Ok<string>(null)).ToApiResult();
            }

            var errors = string.Join(',', identityResult.Errors);
            return (ServiceResult.Ok<string>(errors)).ToApiResult();
        }



        /// <summary>
        /// دریافت تمامی نقش ها
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public virtual async Task<ApiResult<GlobalGridResult<RoleDTO>>> GetAllRoles([FromQuery] int pageNumber, int count, CancellationToken cancellationToken)
        => (ServiceResult.Ok(new GlobalGridResult<RoleDTO>
        {
            Data = await roleManager.Roles.ApplyPagination(pageNumber, count).Select(t => RoleDTO.Create(t.Name, t.Description)).ToListAsync(cancellationToken),
            TotalCount = await roleManager.Roles.CountAsync(cancellationToken),
        })).ToApiResult();


        [HttpPost("google")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var logId = Guid.NewGuid().ToString().Substring(0, 8);

            logger.LogInformation("[GoogleLogin:{LogId}] شروع فرآیند ورود با گوگل", logId);

            var tokenResult = await googleLoginService.VerifyGoogleTokenAsync(request.Credential);
            if (!tokenResult.IsSuccess)
            {
                logger.LogWarning("[GoogleLogin:{LogId}] توکن گوگل معتبر نیست: {Message}", logId, tokenResult.Message);
                return BadRequest(new { isSuccess = false, message = tokenResult.Message });
            }

            var payload = tokenResult.Data!;
            var email = payload.Email;

            // بررسی وجود کاربر
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                logger.LogInformation("[GoogleLogin:{LogId}] کاربر جدید شناسایی شد - ایجاد حساب کاربری", logId);

                user = new User
                {
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true,
                    FullName = $"{payload.GivenName} {payload.FamilyName}"
                };

                var createResult = await userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    logger.LogError("[GoogleLogin:{LogId}] خطا در ایجاد کاربر جدید - Errors: {Errors}", logId, errors);
                    return BadRequest(new { isSuccess = false, message = "خطا در ایجاد کاربر: " + errors });
                }

                var roleResult = await userManager.AddToRoleAsync(user, UserRoleNames.User);
                if (!roleResult.Succeeded)
                {
                    var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    logger.LogWarning("[GoogleLogin:{LogId}] خطا در اضافه کردن نقش به کاربر - Errors: {RoleErrors}", logId, roleErrors);
                }

                logger.LogInformation("[GoogleLogin:{LogId}] کاربر جدید با موفقیت ایجاد شد - UserId: {UserId}", logId, user.Id);
            }
            else
            {
                logger.LogInformation("[GoogleLogin:{LogId}] کاربر موجود شناسایی شد - UserId: {UserId}, UserName: {UserName}", logId, user.Id, user.UserName);
            }

            // صدور JWT
            logger.LogDebug("[GoogleLogin:{LogId}] شروع ایجاد توکن JWT", logId);
            var jwt = await jwtService.GenerateAsync(user);
            logger.LogDebug("[GoogleLogin:{LogId}] توکن JWT با موفقیت ایجاد شد", logId);

            // دریافت اطلاعات کاربر
            var userDto = await userRepository.Table
                .Where(x => x.Id == user.Id)
                .Include(t => t.UserPlans)
                .SelectUsers(
                    userPlanRepository.Table,
                    userPlanAnswerRepository.Table,
                    questionLinkRepository.Table,
                    planQuestionRepository.Table,
                    true)
                .FirstOrDefaultAsync(cancellationToken);

            if (userDto == null)
            {
                logger.LogError("[GoogleLogin:{LogId}] اطلاعات کاربر پس از ایجاد پیدا نشد - UserId: {UserId}", logId, user.Id);
                return BadRequest(new { isSuccess = false, message = "خطا در دریافت اطلاعات کاربر" });
            }

            sw.Stop();
            logger.LogInformation("[GoogleLogin:{LogId}] لاگین با گوگل با موفقیت انجام شد - UserId: {UserId}, Duration: {ElapsedMs}ms", logId, user.Id, sw.ElapsedMilliseconds);

            return Ok(new
            {
                isSuccess = true,
                data = new
                {
                    accessToken = jwt,
                    user = userDto
                },
                message = "ورود با موفقیت انجام شد"
            });
        }



        /// <summary>
        /// اعمال پسورد برای کاربرانی که پسورد ندارند
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request)
        {
            var user = await userManager.GetUserAsync(User);
            var result = await userManager.AddPasswordAsync(user, request.NewPassword);

            if (result.Succeeded)
                return Ok("Password set successfully");

            return BadRequest(result.Errors);
        }


        /// <summary>
        /// دریافت لیست Badgeهای من
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<List<MyBadgeDTO>>> GetMyBadges([FromQuery] GetMyBadgesQuery input)
        {
            return (await queryDispatcher.SendAsync(input)).ToApiResult();
        }

        /// <summary>
        /// ثبت کاربر مشابه و صدور نشان
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> RegisterSimilarity([FromBody] RegisterSimilarUserCommand input)
            => (await commandDispatcher.SendAsync(input)).ToApiResult();


        /// <summary>
        /// دریافت اطلاعات Session جاری
        /// </summary>
        [HttpGet("session-info")]
        public async Task<ApiResult<SessionInfoDTO>> GetSessionInfo()
        {
            var sessionInfo = new SessionInfoDTO
            {
                VisitorId = HttpContext.Session.GetString("VisitorId"),
                UserId = HttpContext.Session.GetString("UserId"),
                Username = HttpContext.Session.GetString("Username"),
                IsAuthenticated = User.Identity.IsAuthenticated,
                LoginTime = HttpContext.Session.GetString("LoginTime")
            };

            // اگر کاربر لاگین است، اطلاعات کامل را برگردان
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int id))
                {
                    var user = await userRepository.TableNoTracking
                        .Where(u => u.Id == id)
                        .Select(u => new { u.UserName, u.Email })
                        .FirstOrDefaultAsync();

                    if (user != null)
                    {
                        sessionInfo.Username = user.UserName;
                        sessionInfo.Email = user.Email;
                    }
                }
            }

            return new ApiResult<SessionInfoDTO>(true, ApiResultStatusCode.Success, sessionInfo);
        }

        /// <summary>
        /// لاگ‌آوت و پاک کردن Session
        /// </summary>
        [HttpPost("logout")]
        public async Task<ApiResult> Logout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var username = HttpContext.Session.GetString("Username");

            // پاک کردن Session
            HttpContext.Session.Clear();

            // اگر از Cookie Authentication استفاده می‌کنید
            await HttpContext.SignOutAsync();

            logger.LogInformation($"User {username} (ID: {userId}) logged out and session cleared");

            return new ApiResult(true, ApiResultStatusCode.Success, "خروج موفقیت‌آمیز بود");
        }


        /// <summary>
        /// درخواست بازنشانی رمز عبور با ارسال کد SMS
        /// </summary>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public virtual async Task<ApiResult<string>> ForgotPassword([FromBody] ForgotPasswordRequestDTO request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var logId = Guid.NewGuid().ToString().Substring(0, 8);

            logger.LogInformation("[ForgotPassword:{LogId}] شروع فرآیند فراموشی رمز عبور برای: {PhoneNumber}", logId, request.PhoneNumber);

            // نرمال سازی شماره تلفن
            var normalizedPhone = NormalizePhoneNumber(request.PhoneNumber);

            // پیدا کردن کاربر با شماره تلفن
            var user = await userRepository.Table
                .FirstOrDefaultAsync(u => u.PhoneNumber == normalizedPhone, cancellationToken);

            if (user == null)
            {
                logger.LogWarning("[ForgotPassword:{LogId}] شماره تلفن یافت نشد: {PhoneNumber}", logId, normalizedPhone);
                // برای امنیت بیشتر، حتی اگر کاربر وجود نداشت هم پیام موفقیت آمیز برگردانید
                return ServiceResult.Ok<string>("اگر شماره تلفن در سیستم وجود داشته باشد، کد بازیابی ارسال خواهد شد").ToApiResult();
            }

            // بررسی تعداد درخواست‌های اخیر (Rate Limiting)
            var rateLimitKey = $"PasswordReset_RateLimit_{normalizedPhone}";
            var recentAttempts = await GetRecentAttemptsCount(rateLimitKey);

            if (recentAttempts >= 3)
            {
                logger.LogWarning("[ForgotPassword:{LogId}] تعداد درخواست‌های بیش از حد برای: {PhoneNumber}", logId, normalizedPhone);
                return ServiceResult.Fail<string>("تعداد درخواست‌های شما بیش از حد مجاز است. لطفاً 15 دقیقه دیگر تلاش کنید.").ToApiResult();
            }

            // تولید کد 6 رقمی
            var resetCode = GenerateResetCode();
            var cacheKey = $"PasswordReset:{normalizedPhone}";

            try
            {
                // ذخیره کد در Redis به مدت 10 دقیقه
                await SaveResetCodeToRedis(cacheKey, resetCode, user.Id);

                // افزایش شمارنده درخواست‌ها
                await IncrementAttemptsCount(rateLimitKey);

                logger.LogInformation("[ForgotPassword:{LogId}] کد بازیابی برای کاربر {UserId} ایجاد شد", logId, user.Id);

                // ارسال SMS
                await SendResetCodeSms(normalizedPhone, resetCode);

                sw.Stop();
                logger.LogInformation("[ForgotPassword:{LogId}] فرآیند فراموشی رمز عبور با موفقیت انجام شد - Duration: {ElapsedMs}ms", logId, sw.ElapsedMilliseconds);

                return ServiceResult.Ok<string>("کد بازیابی با موفقیت ارسال شد").ToApiResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[ForgotPassword:{LogId}] خطا در ارسال کد بازیابی", logId);
                return ServiceResult.Fail<string>("خطا در ارسال کد بازیابی").ToApiResult();
            }
        }

        /// <summary>
        /// تأیید کد بازیابی و بازنشانی رمز عبور
        /// </summary>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public virtual async Task<ApiResult<string>> ResetPassword([FromBody] ResetPasswordRequestDTO request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var logId = Guid.NewGuid().ToString().Substring(0, 8);

            logger.LogInformation("[ResetPassword:{LogId}] شروع فرآیند بازنشانی رمز عبور برای: {PhoneNumber}", logId, request.PhoneNumber);

            var normalizedPhone = NormalizePhoneNumber(request.PhoneNumber);
            var cacheKey = $"PasswordReset:{normalizedPhone}";

            var cachedData = await GetResetCodeFromRedis(cacheKey);

            if (cachedData == null)
            {
                logger.LogWarning("[ResetPassword:{LogId}] کد بازیابی منقضی شده یا وجود ندارد", logId);
                return ServiceResult.Fail<string>("کد بازیابی منقضی شده است. لطفاً مجدداً درخواست کد کنید.").ToApiResult();
            }

            if (cachedData.Code != request.ResetCode)
            {
                logger.LogWarning("[ResetPassword:{LogId}] کد بازیابی نامعتبر برای: {PhoneNumber}", logId, normalizedPhone);

                // افزایش شمارنده تلاش‌های ناموفق
                await IncrementFailedAttempts(cacheKey);
                var failedAttempts = await GetFailedAttemptsCount(cacheKey);

                if (failedAttempts >= 3)
                {
                    await RemoveResetCodeFromRedis(cacheKey);
                    logger.LogWarning("[ResetPassword:{LogId}] کد پس از 3 تلاش ناموفق حذف شد", logId);
                    return ServiceResult.Fail<string>("کد بازیابی پس از 3 تلاش ناموفق حذف شد. لطفاً مجدداً درخواست کد کنید.").ToApiResult();
                }

                return ServiceResult.Fail<string>("کد بازیابی نامعتبر است").ToApiResult();
            }

            // پیدا کردن کاربر
            var user = await userRepository.GetByIdAsync(cancellationToken, cachedData.UserId);
            if (user == null)
            {
                logger.LogError("[ResetPassword:{LogId}] کاربر یافت نشد - UserId: {UserId}", logId, cachedData.UserId);
                return ServiceResult.Fail<string>("کاربر یافت نشد").ToApiResult();
            }

            try
            {
                // اعتبارسنجی رمز عبور جدید
                var passwordValidator = new PasswordValidator<User>();
                var validationResult = await passwordValidator.ValidateAsync(userManager, user, request.NewPassword);

                if (!validationResult.Succeeded)
                {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.Description));
                    logger.LogWarning("[ResetPassword:{LogId}] رمز عبور جدید معتبر نیست - Errors: {Errors}", logId, errors);
                    return ServiceResult.Fail<string>("رمز عبور جدید معتبر نیست: " + errors).ToApiResult();
                }

                // بازنشانی رمز عبور
                var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                var result = await userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger.LogError("[ResetPassword:{LogId}] خطا در بازنشانی رمز عبور - Errors: {Errors}", logId, errors);
                    return ServiceResult.Fail<string>("خطا در بازنشانی رمز عبور: " + errors).ToApiResult();
                }

                // پاک کردن کد از Redis پس از استفاده موفق
                await RemoveResetCodeFromRedis(cacheKey);

                // پاک کردن شمارنده تلاش‌های ناموفق
                await RemoveFailedAttempts(cacheKey);

                // به‌روزرسانی Security Stamp
                await userManager.UpdateSecurityStampAsync(user);

                logger.LogInformation("[ResetPassword:{LogId}] رمز عبور با موفقیت بازنشانی شد - UserId: {UserId}", logId, user.Id);

                // ارسال ایمیل/پیامک تأیید
                //await SendPasswordChangedNotification(user);

                sw.Stop();
                logger.LogInformation("[ResetPassword:{LogId}] فرآیند بازنشانی با موفقیت انجام شد - Duration: {ElapsedMs}ms",
                    logId, sw.ElapsedMilliseconds);

                return ServiceResult.Ok<string>("رمز عبور با موفقیت بازنشانی شد").ToApiResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[ResetPassword:{LogId}] خطا در بازنشانی رمز عبور", logId);
                return ServiceResult.Fail<string>("خطا در بازنشانی رمز عبور").ToApiResult();
            }
        }
        private string GenerateResetCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // کد 6 رقمی
        }

        private string NormalizePhoneNumber(string phoneNumber)
        {
            // حذف فاصله و کاراکترهای غیرعددی
            return new string(phoneNumber.Where(char.IsDigit).ToArray());
        }

        private async Task SaveResetCodeToRedis(string cacheKey, string resetCode, long userId)
        {
            var resetData = new ResetCodeData
            {
                Code = resetCode,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var serializedData = System.Text.Json.JsonSerializer.Serialize(resetData);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // انقضای 10 دقیقه
            };

            await _distributedCache.SetStringAsync(cacheKey, serializedData, options);
        }

        private async Task<ResetCodeData?> GetResetCodeFromRedis(string cacheKey)
        {
            try
            {
                var cachedData = await _distributedCache.GetStringAsync(cacheKey);
                if (string.IsNullOrEmpty(cachedData))
                    return null;

                return System.Text.Json.JsonSerializer.Deserialize<ResetCodeData>(cachedData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطا در بازیابی داده از Redis برای کلید: {CacheKey}", cacheKey);
                return null;
            }
        }

        private async Task RemoveResetCodeFromRedis(string cacheKey)
        {
            try
            {
                await _distributedCache.RemoveAsync(cacheKey);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطا در حذف داده از Redis برای کلید: {CacheKey}", cacheKey);
            }
        }

        private async Task<int> GetRecentAttemptsCount(string rateLimitKey)
        {
            try
            {
                var countStr = await _distributedCache.GetStringAsync(rateLimitKey);
                return string.IsNullOrEmpty(countStr) ? 0 : int.Parse(countStr);
            }
            catch
            {
                return 0;
            }
        }

        private async Task IncrementAttemptsCount(string rateLimitKey)
        {
            try
            {
                var currentCount = await GetRecentAttemptsCount(rateLimitKey);
                var newCount = currentCount + 1;

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) // محدودیت 15 دقیقه
                };

                await _distributedCache.SetStringAsync(rateLimitKey, newCount.ToString(), options);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطا در افزایش شمارنده درخواست‌ها");
            }
        }

        private async Task<int> GetFailedAttemptsCount(string cacheKey)
        {
            var failedKey = $"{cacheKey}:failed";
            try
            {
                var countStr = await _distributedCache.GetStringAsync(failedKey);
                return string.IsNullOrEmpty(countStr) ? 0 : int.Parse(countStr);
            }
            catch
            {
                return 0;
            }
        }

        private async Task IncrementFailedAttempts(string cacheKey)
        {
            var failedKey = $"{cacheKey}:failed";
            try
            {
                var currentCount = await GetFailedAttemptsCount(cacheKey);
                var newCount = currentCount + 1;

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                await _distributedCache.SetStringAsync(failedKey, newCount.ToString(), options);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطا در افزایش شمارنده تلاش‌های ناموفق");
            }
        }

        private async Task RemoveFailedAttempts(string cacheKey)
        {
            var failedKey = $"{cacheKey}:failed";
            try
            {
                await _distributedCache.RemoveAsync(failedKey);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطا در حذف شمارنده تلاش‌های ناموفق");
            }
        }

        private async Task SendResetCodeSms(string phoneNumber, string resetCode)
        {
            try
            {
                // پیاده‌سازی ارسال SMS با سرویس‌دهنده SMS مورد نظر
                var message = $"کد بازیابی رمز عبور پادی لایف: {resetCode}\nاین کد تا 10 دقیقه معتبر است";

                // TODO: جایگزینی با سرویس SMS واقعی
                await _smsService.SendForgotPassword(phoneNumber, resetCode);

                logger.LogInformation("SMS sent to {PhoneNumber}: {Message}", phoneNumber, message);

                // در محیط توسعه، کد را در لاگ چاپ می‌کنیم
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    logger.LogInformation("DEV - Reset Code for {PhoneNumber}: {ResetCode}", phoneNumber, resetCode);
                }

                //await Task.Delay(100); // شبیه‌سازی ارسال SMS
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطا در ارسال SMS به {PhoneNumber}", phoneNumber);
                throw;
            }
        }

        private async Task SendPasswordChangedNotification(User user)
        {
            try
            {
                // ارسال notification برای تغییر رمز عبور
                logger.LogInformation("Password changed notification for user {UserId}", user.Id);

                // TODO: ارسال ایمیل یا پیامک اطلاع‌رسانی
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطا در ارسال اطلاع‌رسانی تغییر رمز عبور");
            }
        }

        private async Task SaveVerificationCodeToRedis(string cacheKey, string verificationCode)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // انقضای 10 دقیقه
            };

            await _distributedCache.SetStringAsync(cacheKey, verificationCode, options);
        }

        private async Task<string?> GetVerificationCodeFromRedis(string cacheKey)
        {
            try
            {
                return await _distributedCache.GetStringAsync(cacheKey);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطا در بازیابی کد تأیید از Redis برای کلید: {CacheKey}", cacheKey);
                return null;
            }
        }

        private async Task RemoveVerificationCodeFromRedis(string cacheKey)
        {
            try
            {
                await _distributedCache.RemoveAsync(cacheKey);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطا در حذف کد تأیید از Redis برای کلید: {CacheKey}", cacheKey);
            }
        }

        private async Task RemoveRateLimit(string rateLimitKey)
        {
            try
            {
                await _distributedCache.RemoveAsync(rateLimitKey);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطا در حذف rate limit برای کلید: {RateLimitKey}", rateLimitKey);
            }
        }

        private async Task SendVerificationCodeSms(string phoneNumber, string verificationCode)
        {
            try
            {
                // پیاده‌سازی ارسال SMS با سرویس‌دهنده SMS مورد نظر
                var message = $"کد تأیید ثبت‌نام پادی لایف: {verificationCode}\nاین کد تا 10 دقیقه معتبر است";

                // TODO: جایگزینی با سرویس SMS واقعی
                await _smsService.SendOTPVerificationCode(phoneNumber, verificationCode);

                logger.LogInformation("Verification SMS sent to {PhoneNumber}: {Message}", phoneNumber, message);

                // در محیط توسعه، کد را در لاگ چاپ می‌کنیم
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    logger.LogInformation("DEV - Verification Code for {PhoneNumber}: {VerificationCode}", phoneNumber, verificationCode);
                }

                //await Task.Delay(100); // شبیه‌سازی ارسال SMS
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطا در ارسال SMS تأیید به {PhoneNumber}", phoneNumber);
                throw;
            }
        }
    }
}
