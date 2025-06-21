using Microsoft.AspNetCore.Diagnostics;

namespace TaskManagerApi
{
    public class AppExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var response = new ErrorResponse()
            {
                Success = false,
                Message = exception.Message,
                StatusCode = 500
            };

            await httpContext.Response.WriteAsJsonAsync(response);

            return true;
        }
    }
}
