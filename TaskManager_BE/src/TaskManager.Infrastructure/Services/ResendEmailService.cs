using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using TaskManager.Application.Common.Interfaces;

namespace TaskManager.Infrastructure.Services;

public sealed class ResendEmailService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : IEmailService
{
    public async Task SendInvitationAsync(string toEmail, string inviterName, string projectName, string inviteUrl, CancellationToken cancellationToken = default)
    {
        string apiKey = configuration["Resend:ApiKey"] ?? string.Empty;
        string fromEmail = configuration["Resend:FromEmail"] ?? "noreply@example.com";

        if (string.IsNullOrWhiteSpace(apiKey))
            return;

        HttpClient httpClient = httpClientFactory.CreateClient("Resend");
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        string html = $"""
            <div style="font-family:sans-serif;max-width:480px;margin:0 auto">
              <h2 style="color:#111">You've been invited to {projectName}</h2>
              <p style="color:#555"><strong>{inviterName}</strong> has invited you to collaborate on <strong>{projectName}</strong> in TaskManager.</p>
              <p style="margin:32px 0">
                <a href="{inviteUrl}"
                   style="background:#2563eb;color:#fff;text-decoration:none;padding:12px 24px;border-radius:6px;font-weight:600;display:inline-block">
                  Accept invitation
                </a>
              </p>
              <p style="color:#999;font-size:12px">This invitation expires in 7 days. If you did not expect this email, you can safely ignore it.</p>
            </div>
            """;

        object payload = new
        {
            from = fromEmail,
            to = new[] { toEmail },
            subject = $"You've been invited to {projectName}",
            html
        };

        await httpClient.PostAsJsonAsync("emails", payload, cancellationToken);
    }
}
