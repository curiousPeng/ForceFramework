using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Force.App.Middleware
{
   public class TextPlainExceptionHandler
    {
        public static RequestDelegate Hander
        {
            get
            {
                return async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "text/html";
                    var ex = context.Features.Get<IExceptionHandlerFeature>();
                    if (ex != null)
                    {
#if DEBUG
                        var err = ex.Error.Message + "</br>" + ex.Error.StackTrace;
#else
                        var err = string.Empty;
                        if (context.Request.Headers.ContainsKey("X-WRK-TEST"))
                        {
                            err = ex.Error.Message;
                        }
                        else
                        {
                            err = "your request sucks!";
                        }
#endif
                        await context.Response.WriteAsync(err).ConfigureAwait(false);
                    }
                };
            }
        }
    }
}
