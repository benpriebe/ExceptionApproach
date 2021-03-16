using Exception.Core;
using Exception.Core.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Exception.Host.Middleware
{
    internal static class ExceptionHandler
    {
        internal static Action<IApplicationBuilder> Middleware()
        {
            return appBuilder => appBuilder.Run(async context =>
            {
                var e = context.Features.Get<IExceptionHandlerPathFeature>().Error;
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
                }

                async Task GenerateResponse(ClientException clientException, int statusCode)
                {
                    var result = new ExceptionResult { Message = clientException?.Message, Extras = clientException?.Messages?.ToArray() };
                    context.Response.StatusCode = statusCode;
                    await context.Response.WriteAsJsonAsync(result);
                }
            });
        }
    }
}