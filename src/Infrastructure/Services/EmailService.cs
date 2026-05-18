using MCQSystem.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace MCQSystem.Infrastructure.Services;

public sealed class EmailService(ILogger<EmailService> logger) : IEmailService
{
    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Email queued to {To} with subject {Subject}. Body length: {BodyLength}", to, subject, body.Length);
        return Task.CompletedTask;
    }
}
