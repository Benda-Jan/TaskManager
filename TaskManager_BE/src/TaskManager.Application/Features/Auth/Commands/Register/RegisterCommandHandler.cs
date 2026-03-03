using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Exceptions;
using RefreshTokenEntity = TaskManager.Domain.Entities.RefreshToken;
using UserEntity = TaskManager.Domain.Entities.User;

namespace TaskManager.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordService passwordService,
    IJwtService jwtService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterCommand, AuthDto>
{
    public async Task<AuthDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailTaken = await userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (emailTaken)
            throw new ConflictException($"Email '{request.Email}' is already registered.");

        var user = new UserEntity(request.Email, request.Name);
        user.SetPasswordHash(passwordService.Hash(request.Password));
        await userRepository.AddAsync(user, cancellationToken);

        var (accessToken, refreshToken) = GenerateTokens(user);
        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthDto(accessToken, refreshToken.Token);
    }

    private (string access, RefreshTokenEntity refresh) GenerateTokens(UserEntity user)
    {
        var access = jwtService.GenerateAccessToken(user.Id, user.Email, user.Name);
        var refresh = new RefreshTokenEntity(user.Id, jwtService.GenerateRefreshToken(),
            DateTime.UtcNow.Add(jwtService.RefreshTokenExpiry));
        return (access, refresh);
    }
}
