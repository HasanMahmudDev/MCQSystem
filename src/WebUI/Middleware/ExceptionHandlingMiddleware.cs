namespace MCQSystem.WebUI.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception for {Path}", context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Redirect($"/Error?message={Uri.EscapeDataString("Something went wrong while processing your request.")}");
        }
    }
}
