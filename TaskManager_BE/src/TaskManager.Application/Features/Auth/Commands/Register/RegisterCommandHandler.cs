using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Features.Projects.Commands.AcceptInvitation;
using TaskManager.Domain.Exceptions;
using RefreshTokenEntity = TaskManager.Domain.Entities.RefreshToken;
using UserEntity = TaskManager.Domain.Entities.User;

namespace TaskManager.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordService passwordService,
    IJwtService jwtService,
    IUnitOfWork unitOfWork,
    ISender sender)
    : IRequestHandler<RegisterCommand, AuthDto>
{
    public async Task<AuthDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        bool emailTaken = await userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (emailTaken)
            throw new ConflictException($"Email '{request.Email}' is already registered.");

        UserEntity user = new(request.Email, request.Name);
        user.SetPasswordHash(passwordService.Hash(request.Password));
        await userRepository.AddAsync(user, cancellationToken);

        (string accessToken, RefreshTokenEntity refreshToken) = GenerateTokens(user);
        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.InvitationToken))
            await sender.Send(new AcceptInvitationCommand(request.InvitationToken, user.Id, user.Email), cancellationToken);

        return new AuthDto(accessToken, refreshToken.Token);
    }

    private (string access, RefreshTokenEntity refresh) GenerateTokens(UserEntity user)
    {
        string access = jwtService.GenerateAccessToken(user.Id, user.Email, user.Name);
        RefreshTokenEntity refresh = new(user.Id, jwtService.GenerateRefreshToken(),
            DateTime.UtcNow.Add(jwtService.RefreshTokenExpiry));
        return (access, refresh);
    }
}
