using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Infinum.ZanP.Extensions
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate m_next;

        public CustomMiddleware(RequestDelegate p_next)
        {
            m_next = p_next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await m_next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message
            }.ToString());
        }
    }
}