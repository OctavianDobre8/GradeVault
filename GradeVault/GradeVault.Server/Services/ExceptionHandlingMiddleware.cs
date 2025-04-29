using System.Net;
using System.Text.Json;

namespace GradeVault.Server.Services
{
    /**
     * @brief Middleware for handling unhandled exceptions throughout the application
     *
     * This middleware catches any unhandled exceptions that occur during request processing,
     * logs them appropriately, and returns a consistent error response to the client.
     */
    public class ExceptionHandlingMiddleware
    {
        /**
         * @brief Reference to the next middleware in the request pipeline
         */
        private readonly RequestDelegate _next;
        
        /**
         * @brief Logger for recording exception details
         */
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /**
         * @brief Constructor that initializes the middleware with dependencies
         *
         * @param next The next middleware delegate in the request processing pipeline
         * @param logger Logger for recording exception information
         */
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /**
         * @brief Invokes the middleware to process the current HTTP request
         *
         * Attempts to process the request normally by calling the next middleware,
         * and catches any exceptions that may occur during processing.
         *
         * @param context The HTTP context for the current request
         */
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        /**
         * @brief Handles exceptions by generating an appropriate error response
         *
         * Formats the exception into a consistent JSON error response with
         * appropriate HTTP status code.
         *
         * @param context The HTTP context for the current request
         * @param exception The exception that was caught
         * @return Task representing the async operation of writing the response
         */
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new 
            {
                error = "An error occurred while processing your request.",
                detail = exception.Message
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);
            
            return context.Response.WriteAsync(json);
        }
    }
}