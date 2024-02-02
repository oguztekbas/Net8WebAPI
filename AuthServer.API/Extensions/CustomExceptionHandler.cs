

using AuthServer.Core.CommonDTOs;
using Microsoft.AspNetCore.Diagnostics;
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
                            var response = Response<NoDataDto>.Fail(exception.Message, 500, false);

                            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                        }
                    }
                });
            });
        }
    }
}
