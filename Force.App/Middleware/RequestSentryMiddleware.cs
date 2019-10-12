using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Force.Common.AES;
using Force.Model.Request;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Force.Common.DateTimeEx;
using Force.Model.Basic;

namespace Force.App.Middleware
{
    public class RequestSentryMiddleware
    {
        class ValidateError
        {
            public const int 正常 = 0;
            public const int token已过期 = 1000;
            public const int token为空 = 1001;
            public const int 用户已冻结 = 1002;
            public const int 用户已下线 = 1003;
            public const int 没有访问权限 = 1004;
            public const int timespan异常 = 2000;
            public const int 访问频率异常 = 2001;
            public const int 非法的token = 2002;
            public const int 请求的body为空 = 2003;
            public const int 非法的body = 2004;
        }

        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly string _error_template = "Validate检测不通过，{0}";
        private readonly byte[] _secret_key = Convert.FromBase64String("kyD0GK7X2KEUimQ2BPVS3iqFREJUdG5mykwh5A4nT3A=");
        private readonly byte[] _auth_key = Convert.FromBase64String("nr8hxFqfLkQek1mmp07L7/8s/AAsfIX5Wg0DnH/v2D0=");
        private readonly int _time_error;
        private readonly int _limit_count;
        private readonly double _token_expire;
        private readonly List<string> _site_white_list_without_token;
        private readonly List<string> __request_by_unconditionally;
        private readonly Regex _regex_phone_pattern = new Regex("^(13[0-9]|14[579]|15[0-3,5-9]|16[0-9]|17[0135678]|18[0-9]|19[89])\\d{8}$", RegexOptions.Compiled);

        public RequestSentryMiddleware(
            int timeError,
            int requestLimitRate,
            IMemoryCache cache,
            RequestDelegate next)
        {
            _next = next;
            _cache = cache;
            _time_error = timeError;
            _limit_count = requestLimitRate;
            _token_expire = 30; // 按分钟算 目前测试，先改为30分钟
            _site_white_list_without_token = new List<string>
            {
                "/api/login",
                "/api/regiest",
                "/api/values"
            };
            __request_by_unconditionally = new List<string>
            {
                "/api/values"
            };
        }

        public async Task Invoke(HttpContext context)
        {
            var flag = true;
            var out_str = string.Empty;
            MRequest json_obj = null;


            if (__request_by_unconditionally.Contains(context.Request.Path))
            {
                context.Items["MRequest"] = new MRequest();
                goto Next;
            }
            if (!context.Request.ContentLength.HasValue)
            {
                flag = false;
                EndPipelineProcessing(ValidateError.请求的body为空, context, "http请求body为空");
            }

            try
            {
                var raw_body_str = context.Request.Form["data"];
                json_obj = JsonConvert.DeserializeObject<MRequest>(raw_body_str);
            }
            catch
            {
                flag = false;
                EndPipelineProcessing(ValidateError.非法的body, context, "非法的http请求body");
            }

            var code = Validate(context, json_obj, out out_str);
            if (code == 0)
            {
                flag = true;
            }
            else
            {
                flag = false;
                EndPipelineProcessing(code, context, JsonConvert.SerializeObject(new { Msg = out_str, Status = code }), false);
                goto Pass;
            }
            context.Items["MRequest"] = json_obj;

        // Call the next delegate/middleware in the pipeline
        Next: if (flag)
            {
                await this._next(context);
            }

        Pass:;
        }

        /// <summary>
        /// 返回：
        /// 0 表示所有检测通过
        /// 0-code-10 不需要抛出异常的错误
        /// code > 10 抛出异常
        /// </summary>
        /// <param name="context"></param>
        /// <param name="request"></param>
        /// <param name="out_str"></param>
        /// <returns></returns>
        private int Validate(HttpContext context, MRequest request, out string out_str)
        {
            out_str = "OK";
            var timestamp = request.TimeStamp.ToDateTime();
            var timespan = (DateTime.Now - timestamp).TotalMinutes;
            if (timespan < -1 * _time_error || timespan > _time_error)
            {
                out_str = "timespan超过" + _time_error + "分钟，request.TimeStamp=" + request.TimeStamp;
                return ValidateError.timespan异常;
            }

            //TODO:检查用户登录状态
            //var user = _cache.Get<UserInfo_Session>(key);
            //if (user == null)
            //{
            //    out_str = nameof(ValidateError.用户已下线);
            //    return ValidateError.用户已下线;
            //}

            if (string.IsNullOrEmpty(request.Token))
            {
                // 只允许访问特定action，目前有login、register
                if (_site_white_list_without_token.Contains(context.Request.Path))
                {
                    return ValidateError.正常;
                    // 访问频率检测
                    //var ret_ip = CheckRate(request.Phone, ref out_str);
                    //if (!ret_ip)
                    //TODO:校验频繁操作，冻结账号处理
                    //return ValidateError.访问频率异常;
                }
                else
                {
                    out_str = nameof(ValidateError.token为空);
                    return ValidateError.token为空;
                }
            }
            else
            {

                var decrypted_info = string.Empty;
                try
                {
                    decrypted_info = AESUtil.DecryptStr(request.Token, _secret_key, _auth_key);
                }
                catch
                {
                    out_str = nameof(ValidateError.非法的token);
                    return ValidateError.非法的token;
                }

                //TODO:反序列化token
                var tokenInfo = JsonConvert.DeserializeObject<TokenInfo>(decrypted_info);
                if ((DateTime.Now - DateTime.Parse(tokenInfo.Time)).TotalMinutes > _token_expire)
                {
                    out_str = nameof(ValidateError.token已过期);
                    return ValidateError.token已过期;
                }
                request.TokenInfo = tokenInfo;
            }

            return ValidateError.正常;
        }


        private int Authentication()
        {
            return ValidateError.没有访问权限;
        }
        private bool CheckPhone(string phone_str)
        {
            if (string.IsNullOrWhiteSpace(phone_str))
            {
                return false;
            }

            var match = _regex_phone_pattern.Match(phone_str);
            return match.Success;
        }

        private bool CheckRate(string identity, ref string out_str)
        {
            // 访问频率检测
            var key = identity + "_limit";
            long cacheEntry;
            if (_cache.TryGetValue<long>(key, out cacheEntry))
            {
                if (cacheEntry > _limit_count)
                {
                    out_str = "访问频率过高（限制为" + _limit_count + "），count=" + cacheEntry;
                    return false;
                }
                else
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(1));
                    _cache.Set(key, ++cacheEntry, cacheEntryOptions);
                }
            }
            else
            {
                cacheEntry = 1;
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(1));
                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }

            return true;
        }

        private void EndPipelineProcessing(int code, HttpContext context, string msg, bool throw_exception = true)
        {
            if (code >= 2000)
            {
                throw new InvalidOperationException(string.Format(_error_template, msg));
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                if (code == 1004)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                }

                context.Response.ContentType = "text/json;charset=UTF-8";
                context.Response.WriteAsync(msg).ConfigureAwait(false);
            }
        }
    }
}
