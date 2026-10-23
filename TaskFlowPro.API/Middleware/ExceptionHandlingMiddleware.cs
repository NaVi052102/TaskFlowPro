using System.Net; // Import namespace for HTTP status codes
using System.Text.Json; // Import namespace for JSON serialization

namespace TaskFlowPro.API.Middleware // Define namespace for API middleware components
{
    public class ExceptionHandlingMiddleware // Define public class for catching and handling unhandled exceptions globally
    {
        private readonly RequestDelegate _next; // Declare private read-only request delegate for pipeline continuation
        private readonly ILogger<ExceptionHandlingMiddleware> _logger; // Declare private read-only logger instance for recording errors

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) // Constructor injecting request delegate and logger via dependency injection
        {
            _next = next; // Assign injected request delegate to private field
            _logger = logger; // Assign injected logger instance to private field
        }

        public async Task InvokeAsync(HttpContext context) // Asynchronous middleware invocation method processing HTTP requests
        {
            try
            {
                await _next(context); // Pass HTTP context along to the next middleware component in the pipeline
            }
            catch (Exception ex) // Catch any unhandled exceptions thrown during request execution
            {
                _logger.LogError(ex, "An unhandled exception occurred during request processing."); // Log exception details to logging provider
                await HandleExceptionAsync(context, ex); // Invoke helper method to write standardized error response
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception) // Private static helper method formatting error payload
        {
            context.Response.ContentType = "application/json"; // Set response content type header to JSON format
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // Set HTTP status code to 500 Internal Server Error

            var response = new // Define anonymous object representing standardized error payload
            {
                status = context.Response.StatusCode, // Include HTTP status code property
                message = "An internal server error occurred.", // Include generic safe error message
                detail = exception.Message // Include exception message detail
            };

            var jsonResponse = JsonSerializer.Serialize(response); // Serialize anonymous error object into JSON string format
            return context.Response.WriteAsync(jsonResponse); // Write serialized JSON response back to HTTP response stream
        }
    }
}