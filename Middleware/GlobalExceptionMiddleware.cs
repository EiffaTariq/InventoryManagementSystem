
namespace IMS.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        ILogger<GlobalExceptionMiddleware> _logger;
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            catch(KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Unhandled exception occured");
                await HandleKeyNotFoundExceptionAsync(context);
            }
            catch(ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid Arguement received");
                await HandleInvalidArgumentExceptionAsync(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error");
                await HandleGenericExceptionAsync(context);
            }

        }
        private static Task HandleKeyNotFoundExceptionAsync(HttpContext context)
        {
           
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return context.Response.WriteAsJsonAsync(new
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = "Resource not found"
            });
        }
        private static Task HandleInvalidArgumentExceptionAsync(HttpContext context)
        {
            context.Response.ContentType= "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return context.Response.WriteAsJsonAsync(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Bad Request"
            });
        }
        private static Task HandleGenericExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return context.Response.WriteAsJsonAsync(new
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "INternal Server Error"
            });
        }
    }
}
