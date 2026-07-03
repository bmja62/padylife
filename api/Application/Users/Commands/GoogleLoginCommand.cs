using Application.Cqrs.Commands;
using MediatR;
using Services;
using Services.Services.AuthServices;
using Services.Services.AuthServices.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Application.Users.Commands
{
    public record GoogleLoginCommand(string Credential) : ICommand<ServiceResult<GoogleUserInfoDto>>;

    public class GoogleLoginCommandHandler : ICommandHandler<GoogleLoginCommand, ServiceResult<GoogleUserInfoDto>>
    {
        private readonly IGoogleLoginService _authService;

        public GoogleLoginCommandHandler(IGoogleLoginService authService)
        {
            _authService = authService;
        }

        public async Task<ServiceResult<GoogleUserInfoDto>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken) => await _authService.VerifyGoogleTokenAsync(request.Credential);
    }
}
