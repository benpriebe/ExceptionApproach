using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Exception.Core;
using Exception.Core.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Exception.Host.Middleware
{
    public class I18NMiddleware
    {
        private readonly RequestDelegate next;

        public I18NMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        
        public async Task Invoke(HttpContext context)
        {
            string locale = null;

            if (context.Request.Query.ContainsKey("lng"))
            {
                locale = context.Request.Query["lng"];
            } 
            
            if (string.IsNullOrWhiteSpace(locale))
            {
                // Otherwise get it from the browser.
                locale = !string.IsNullOrWhiteSpace(context.Request.Headers["Accept-Language"])
                    ? context.Request.Headers["Accept-Language"].ToArray().First().Split(',')[0]
                    : "en-US";
            }

            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(locale);

            try
            {
                await next.Invoke(context);
            }
            catch (System.Exception e)
            {
                switch (e)
                {
                    case NotAuthenticatedClientException ex:
                        await GenerateResponse(ex, StatusCodes.Status401Unauthorized);
                        break;

                    case ForbiddenClientException ex:
                        await GenerateResponse(ex, StatusCodes.Status403Forbidden);
                        break;

                    case NotFoundClientException<object> ex:
                        await GenerateResponse(ex, StatusCodes.Status404NotFound);
                        break;

                    case ClientException ex:
                        await GenerateResponse(ex, StatusCodes.Status400BadRequest);
                        break;
                        
                    default:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        break;
                };
            }

            async Task GenerateResponse(ClientException clientException, int statusCode)
            {
                var result = new ExceptionResult { Message = clientException?.Message, Extras = clientException?.Messages?.ToArray() };
                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsJsonAsync(result);
            }
        }
    }

    public static class I18NMiddlewareExtensions
    {
        public static IApplicationBuilder UseI18NMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<I18NMiddleware>();
        }
    }}