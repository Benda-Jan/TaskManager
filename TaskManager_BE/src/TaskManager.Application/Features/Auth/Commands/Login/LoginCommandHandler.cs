using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Features.Projects.Commands.AcceptInvitation;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;
using RefreshTokenEntity = TaskManager.Domain.Entities.RefreshToken;

namespace TaskManager.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordService passwordService,
    IJwtService jwtService,
    IUnitOfWork unitOfWork,
    ISender sender)
    : IRequestHandler<LoginCommand, AuthDto>
{
    public async Task<AuthDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || user.PasswordHash is null ||
            !passwordService.Verify(user.PasswordHash, request.Password))
            throw new UnauthorizedException("Invalid email or password.");

        string access = jwtService.GenerateAccessToken(user.Id, user.Email, user.Name);
        RefreshTokenEntity refresh = new(user.Id, jwtService.GenerateRefreshToken(),
            DateTime.UtcNow.Add(jwtService.RefreshTokenExpiry));

        await refreshTokenRepository.AddAsync(refresh, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.InvitationToken))
            await sender.Send(new AcceptInvitationCommand(request.InvitationToken, user.Id, user.Email), cancellationToken);

        return new AuthDto(access, refresh.Token);
    }
}
