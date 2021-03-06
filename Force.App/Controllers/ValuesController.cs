﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Force.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : BaseController
    {
        public ValuesController(IHttpContextAccessor httpContextAccessor):base(httpContextAccessor){}
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            ///示例各中间件用法
            ///rabbitmq消费端的用法参考 https://github.com/curiousPeng/LightMessager 里的demo
            //this.RabbitMQHelper.Send(new TestMessage { CreatedTime = DateTime.Now, id = "1", num = 2, Source = Guid.NewGuid().ToString() });
            //this.RedisHelper.HashObjSet<TestMessage>("1a2b", new TestMessage { a="11",b=2,c="333"});
            //var a = this.RedisHelper.HashFieldGet<string>("1a2b", "a");
            //this.Cache.GetOrCreate(key, entry =>
            //{
            //    // 设置一个滑动过期时间
            //    entry.SetSlidingExpiration(TimeSpan.FromDays(1));
            //    //or 固定过期时间
            //    //entry.SetAbsoluteExpiration(TimeSpan.FromDays(1));
            //    var a = new TestMessage { CreatedTime = DateTime.Now, id = "1", num = 2 };
            //    return a;
            //});
            logger.Info("测试记录");
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        /// <summary>
        /// 这是一个带参数的get请求
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Get api/Values/1
        /// </remarks>
        /// <param name="id">主键</param>
        /// <returns>测试字符串</returns> 
        /// <response code="201">返回value字符串</response>
        /// <response code="400">如果id为空</response>  
        // GET api/values/2
        [HttpGet("{id}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
