using System.Diagnostics;

namespace API.Middleware
{
    public class RequestTimingMiddleware(RequestDelegate requestDelegate, ILogger<RequestTimingMiddleware> logger)
    {
        readonly private RequestDelegate _next = requestDelegate;
        readonly private ILogger<RequestTimingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            int? statusCode = null;
            Stopwatch stopwatch = Stopwatch.StartNew();
            // Call the next middleware in the pipeline
            try
            {
                await _next(context);
                statusCode = context.Response.StatusCode;
            }
            catch
            {
                statusCode = 500; // Internal Server Error
                throw; // Re-throw the exception to let the default exception handler handle it
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Method {Method}, Path {Path}, StatusCode {StatusCode}, ElapsedTime {ElapsedMs} ms", context.Request.Method, context.Request.Path, statusCode, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
