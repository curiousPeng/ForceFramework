using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Force.Model;
using Force.Model.Request;
using StackExchange.Exceptional;

namespace Force.App.Middleware
{
    public class SOExceptionMiddleware
    {
        private readonly string _servicename;
        private readonly RequestDelegate _next;

        public SOExceptionMiddleware(
            string serviceName,
            RequestDelegate next)
        {
            _servicename = serviceName;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var current_request = context.Items["MRequest"] as MRequest;
                if (current_request == null)
                {
                    current_request = new MRequest();
                }
                var custom_data = new Dictionary<string, string>
                {
                    { "URL", context.Request.Path },
                    { "RemoteIP", context.Connection.RemoteIpAddress != null ? context.Connection.RemoteIpAddress.ToString() : "未知IP" },
                    { "UserCode", current_request.TokenInfo!=null ? current_request.TokenInfo.UserId.ToString() : "-1" }
                };
                ex.Log(context, customData: custom_data);

                // re-throw the original exception
                throw;
            }
        }
    }
}
