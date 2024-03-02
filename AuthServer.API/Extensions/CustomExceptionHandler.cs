using AuthServer.API.Controllers;
using AuthServer.Core.CommonDTOs;
using AuthServer.Core.DTOs;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using System.Text.Json;

namespace AuthServer.API.Extensions
{
    public static class CustomExceptionHandler
    {
        public static void UseHandleGlobalException(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var errorFeauture = context.Features.Get<IExceptionHandlerFeature>();

                    if(errorFeauture != null)
                    {
                        var exception = errorFeauture.Error;

                        if (exception != null)
                        {
                            Log.ForContext<AuthController>().Write(Serilog.Events.LogEventLevel.Error, "Custom Exception Handler => {@exception}", exception);

                            var response = Response<NoDataDto>.Fail(exception.Message, 500, false);

                            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                        }
                    }
                });
            });
        }
    }
}
