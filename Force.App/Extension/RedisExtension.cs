﻿using Force.Common.RedisTools;
using Microsoft.Extensions.DependencyInjection;
using RedisTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Force.App.Extension
{
    public static class RedisExtension
    {
        public static IServiceCollection AddRedis(this IServiceCollection services,string redisConnString)
        {
            services.AddSingleton(typeof(IRedisBase), _ => new RedisBase(redisConnString));

            services.AddSingleton<IRedisHelper,RedisHelper>();

            return services;
        }
    }
}
