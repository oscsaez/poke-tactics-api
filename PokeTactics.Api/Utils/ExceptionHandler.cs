using System.Text.Json;
using PokeTactics.Core.Exceptions;

namespace PokeTactics.Api.Utils
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandler> _logger;

        public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error ocurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int statusCode = ex switch
            {
                EntityDoesNotExistException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            var result = JsonSerializer.Serialize(new
            {
                error = ex.Message
            });

            context.Response.ContentType = ApiConstants.ContentType;
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(result);
        }
    }
}