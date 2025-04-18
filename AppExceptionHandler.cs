using Microsoft.AspNetCore.Diagnostics;

namespace TaskManagerApi
{
    public class AppExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            await httpContext.Response.WriteAsJsonAsync("Something wrong D:");


            return true;
        }
    }
}
