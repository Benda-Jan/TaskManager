using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Exceptions;
using RefreshTokenEntity = TaskManager.Domain.Entities.RefreshToken;

namespace TaskManager.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IJwtService jwtService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RefreshTokenCommand, AuthDto>
{
    public async Task<AuthDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existing = await refreshTokenRepository.GetByTokenAsync(request.Token, cancellationToken);

        if (existing is null || !existing.IsValid)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        var user = await userRepository.GetByIdAsync(existing.UserId, cancellationToken)
            ?? throw new UnauthorizedException("User not found.");

        existing.Revoke();
        refreshTokenRepository.Remove(existing);

        var access = jwtService.GenerateAccessToken(user.Id, user.Email, user.Name);
        var refresh = new RefreshTokenEntity(user.Id, jwtService.GenerateRefreshToken(),
            DateTime.UtcNow.Add(jwtService.RefreshTokenExpiry));

        await refreshTokenRepository.AddAsync(refresh, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthDto(access, refresh.Token);
    }
}
