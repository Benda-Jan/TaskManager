namespace TaskManager.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendInvitationAsync(string toEmail, string inviterName, string projectName, string inviteUrl, CancellationToken cancellationToken = default);
}
