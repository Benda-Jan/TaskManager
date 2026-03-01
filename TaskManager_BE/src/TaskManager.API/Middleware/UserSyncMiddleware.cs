using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.API.Middleware;

public sealed class UserSyncMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUser,
        IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = currentUser.UserId;
            var email = currentUser.Email ?? string.Empty;
            var name = context.User.FindFirst("name")?.Value
                ?? context.User.FindFirst("preferred_username")?.Value
                ?? email;

            if (userId != Guid.Empty)
            {
                var exists = await userRepository.ExistsAsync(userId);
                if (!exists)
                {
                    await userRepository.AddAsync(new User(userId, email, name));
                    await unitOfWork.SaveChangesAsync();
                }
                else
                {
                    var user = await userRepository.GetByIdAsync(userId);
                    if (user is not null && (user.Email != email || user.Name != name))
                    {
                        user.UpdateProfile(email, name);
                        userRepository.Update(user);
                        await unitOfWork.SaveChangesAsync();
                    }
                }
            }
        }

        await next(context);
    }
}
