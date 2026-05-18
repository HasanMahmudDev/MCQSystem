using System.Security.Claims;
using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Entities;

namespace MCQSystem.WebUI.Middleware;

public sealed class AuditTrailMiddleware(RequestDelegate next, ILogger<AuditTrailMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IRepository<AuditLog> auditLogs)
    {
        await next(context);

        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            return;
        }

        if (!HttpMethods.IsPost(context.Request.Method)
            && !HttpMethods.IsPut(context.Request.Method)
            && !HttpMethods.IsDelete(context.Request.Method))
        {
            return;
        }

        try
        {
            await auditLogs.AddAsync(new AuditLog
            {
                UserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier),
                Action = context.Request.Method,
                EntityName = context.Request.Path,
                Details = $"Status {context.Response.StatusCode}",
                IpAddress = context.Connection.RemoteIpAddress?.ToString()
            }, context.RequestAborted);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Audit write failed for {Path}", context.Request.Path);
        }
    }
}
