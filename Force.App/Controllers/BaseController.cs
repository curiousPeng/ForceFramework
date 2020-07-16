using Force.Common.LightMessager.Helper;
using Force.Common.RedisTools;
using Force.Model.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace Force.App.Controllers
{
    public class BaseController : ControllerBase
    {
        private HttpContext _http_context;
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private IMemoryCache _cache;
        private IRedisHelper _redis_helper;
        private IRabbitMQProducer _rabbitmq;
        public BaseController(IHttpContextAccessor httpContextAccessor)
        {
            _http_context = httpContextAccessor.HttpContext;
            RequestObj = _http_context.Items["MRequest"] as MRequest;
        }

        protected IMemoryCache Cache
        {
            get
            {
                return _cache ?? (_cache = _http_context.RequestServices.GetService<IMemoryCache>());
            }
        }

        protected IRedisHelper RedisHelper
        {
            get
            {
                return _redis_helper ?? (_redis_helper = _http_context.RequestServices.GetService<IRedisHelper>());
            }
        }

        protected IRabbitMQProducer RabbitMQHelper
        {
            get
            {
                return _rabbitmq ?? (_rabbitmq = _http_context.RequestServices.GetService<IRabbitMQProducer>());
            }
        }

        protected MRequest RequestObj { private set; get; }
    }
}
