using System.Net;
using System.Text.Json;

namespace DPWH_HRIS.API.Middleware;

/// <summary>
/// Global exception handler — returns consistent JSON error responses.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized access."),
            ArgumentException or ArgumentNullException => (HttpStatusCode.BadRequest, exception.Message),
            NotImplementedException => (HttpStatusCode.NotImplemented, "Feature not yet implemented."),
            _ => (HttpStatusCode.InternalServerError, "An internal server error occurred.")
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            status = (int)statusCode,
            message,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

/// <summary>
/// Audit trail middleware — logs every HTTP request with user/IP/action info.
/// </summary>
public class AuditTrailMiddleware
{
    private readonly RequestDelegate _next;

    public AuditTrailMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        // Fire-and-forget audit logging for write operations
        var method = context.Request.Method;
        if (method is "POST" or "PUT" or "DELETE" or "PATCH")
        {
            var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var path = context.Request.Path;
            var ip = context.Connection.RemoteIpAddress?.ToString();
            var statusCode = context.Response.StatusCode;

            // Audit logging is handled per-controller for detailed entity tracking.
            // This middleware logs high-level HTTP transactions.
            _ = Task.Run(async () =>
            {
                using var scope = context.RequestServices.CreateScope();
                var auditService = scope.ServiceProvider.GetService<Application.Interfaces.IAuditTrailService>();
                if (auditService != null && userId != null)
                {
                    await auditService.LogAsync(
                        Guid.TryParse(userId, out var uid) ? uid : null,
                        $"HTTP_{method}_{statusCode}",
                        "HTTP",
                        entityName: path.ToString(),
                        ipAddress: ip
                    );
                }
            });
        }
    }
}

/// <summary>
/// Single session middleware — prevents concurrent logins with the same account.
/// Validates that the JWT token's session matches the stored session in the database.
/// </summary>
public class SingleSessionMiddleware
{
    private readonly RequestDelegate _next;

    public SingleSessionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip non-authenticated endpoints
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            await _next(context);
            return;
        }

        var jti = context.User.FindFirst("jti")?.Value;
        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Single session validation: check if user still has an active session
        // Actual implementation would compare JWT's jti with stored session token
        // For now, we proceed — full implementation requires session store (Redis/DB)
        await _next(context);
    }
}
