using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Exception.Core.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(ex.Messages);
                        break;

                    case ForbiddenClientException ex:
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(ex.Messages);
                        break;

                    case NotFoundClientException<object> ex:
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsJsonAsync(ex.Messages);
                        break;

                    case ClientException ex:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsJsonAsync(ex.Messages);
                        break;
                        
                    default:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        break;
                };
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