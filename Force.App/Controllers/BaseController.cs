using Force.Common.RedisTool.Helper;
using Force.Model.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Force.App.Controllers
{
    public class BaseController<T> : ControllerBase
    {
        private HttpContext _http_context;
        private MRequest _request_obj;
        private ILogger<T> _logger;
        private IMemoryCache _cache;
        private IRedisHelper _redis_helper;
        public BaseController(IHttpContextAccessor httpContextAccessor)
        {
            _http_context = httpContextAccessor.HttpContext;
            _request_obj = _http_context.Items["MRequest"] as MRequest;
        }

        protected ILogger<T> Logger
        {
            get
            {
                return _logger ?? (_logger = _http_context.RequestServices.GetService<ILogger<T>>());
            }
        }

        protected IMemoryCache Cache
        {
            get
            {
                return _cache ?? (_cache = _http_context.RequestServices.GetService<IMemoryCache>());
            }
        }

        protected IRedisHelper RedisHlper
        {
            get
            {
                return _redis_helper ?? (_redis_helper = _http_context.RequestServices.GetService<IRedisHelper>());
            }
        }

        protected MRequest RequestObj
        {
            private set { this._request_obj = value; }
            get { return this._request_obj; }
        }
    }
}
