using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace Clinic.API.Middleware
{
    public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IHostEnvironment env)
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };




        public async Task InvokeAsync(HttpContext context)
        {
            var startedAt = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                await HandleExceptionAsync(
                    context,
                    ex,
                    startedAt,
                    stopwatch.Elapsed);

                return;
            }

            stopwatch.Stop();
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception ex,
            DateTimeOffset startedAtUtc,
            TimeSpan elapsed)
        {
            var statusCode = ex switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound,
                UnauthorizedAccessException => HttpStatusCode.Forbidden,
                InvalidOperationException => HttpStatusCode.Conflict,
                ArgumentException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError,
            };

            var isInternalError = statusCode == HttpStatusCode.InternalServerError;

            var request = context.Request;

            var errorId =
                Activity.Current?.Id
                ?? context.TraceIdentifier
                ?? Guid.NewGuid().ToString("N");

            var traceId = Activity.Current?.TraceId.ToString();
            var spanId = Activity.Current?.SpanId.ToString();

            var userId = "todo:auth. anonymous";

            using (logger.BeginScope(new Dictionary<string, object>
            {
                ["ErrorId"] = errorId,
                ["TraceId"] = traceId ?? string.Empty,
                ["SpanId"] = spanId ?? string.Empty,
                ["TimestampUtc"] = startedAtUtc,
                ["ElapsedMilliseconds"] = elapsed.TotalMilliseconds,

                // Request
                ["Method"] = request.Method,
                ["Scheme"] = request.Scheme,
                ["Host"] = request.Host.ToString(),
                ["Path"] = request.Path.ToString(),
                ["QueryString"] = request.QueryString.ToString(),
                ["ContentType"] = request.ContentType ?? string.Empty,
                ["ContentLength"] = request.ContentLength ?? 0,
                ["Protocol"] = request.Protocol,

                // Connection
                ["RemoteIp"] =
                    context.Connection.RemoteIpAddress?.ToString() ?? string.Empty,

                // User
                ["UserId"] = userId,

                // Exception
                ["ExceptionType"] = ex.GetType().FullName ?? string.Empty,
                ["ExceptionMessage"] = ex.Message,
                ["InnerExceptionType"] =
                    ex.InnerException?.GetType().FullName ?? string.Empty,
                ["InnerExceptionMessage"] =
                    ex.InnerException?.Message ?? string.Empty,

                // Environment
                ["Environment"] = env.EnvironmentName,

                // Response
                ["StatusCode"] = (int)statusCode,
            }))
            {
                const string template =
                    """
            HTTP request execution failed.

            ErrorId: {ErrorId}
            TraceId: {TraceId}
            SpanId: {SpanId}

            TimestampUtc: {TimestampUtc}
            ElapsedMilliseconds: {ElapsedMilliseconds}

            StatusCode: {StatusCode}

            Request:
            {Method} {Path}{QueryString}

            Exception:
            {ExceptionType}
            """;

                if (isInternalError)
                {
                    logger.LogError(
                        ex,
                        template,
                        errorId,
                        traceId,
                        spanId,
                        startedAtUtc,
                        elapsed.TotalMilliseconds,
                        (int)statusCode,
                        request.Method,
                        request.Path,
                        request.QueryString,
                        ex.GetType().FullName);
                }
                else
                {
                    logger.LogWarning(
                        ex,
                        template,
                        errorId,
                        traceId,
                        spanId,
                        startedAtUtc,
                        elapsed.TotalMilliseconds,
                        (int)statusCode,
                        request.Method,
                        request.Path,
                        request.QueryString,
                        ex.GetType().FullName);
                }
            }

            object body;

            if (env.IsDevelopment())
            {
                body = new
                {
                    errorId,
                    traceId,
                    timestampUtc = startedAtUtc,
                    elapsedMilliseconds = elapsed.TotalMilliseconds,
                    message = ex.Message,
                    type = ex.GetType().FullName,
                    trace = ex.StackTrace,
                };
            }
            else
            {
                body = new
                {
                    errorId,
                    traceId,
                    timestampUtc = startedAtUtc,
                    message = isInternalError
                        ? "An unexpected error occurred."
                        : "Request could not be processed."
                };
            }

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(body, _jsonOptions));
        }
    }
}
