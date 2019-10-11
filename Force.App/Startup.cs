using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Force.App.Extension;
using Force.App.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace Force.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMemoryCache();
            services.AddNLog();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddTheExceptional(Configuration.GetSection("Exceptional"));
            services.AddRedis(Configuration.GetSection("RedisConn").Value);
            services.AddRabbitMQ(Common.LightMessager.DAL.DataBaseEnum.SqlServer, Configuration.GetSection("RabbitMqConn").Value);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            //注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Force API",
                    Description = "A simple ASP.NET Core Web API Framework",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "CuriousPeng",
                        Email = "CuriousPeng@outlook.com",
                        Url = "https://github.com/curiousPeng"
                    },
                    License = new License
                    {
                        Name = "Apache License 2.0",
                        Url = "https://github.com/curiousPeng/ForceFramework/blob/master/LICENSE"
                    }
                });
                // 为 Swagger JSON and UI设置xml文档注释路径
                var basePath = AppContext.BaseDirectory;//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var xmlPath = Path.Combine(basePath, "Swagger.xml");
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(options =>
                {
                    options.Run(TextPlainExceptionHandler.Hander);
                });
            }
            app.UseMiddleware(typeof(SOExceptionMiddleware), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);

            var time_error = 5;
            var request_limit = 10;
            app.UseMiddleware(typeof(RequestSentryMiddleware), time_error, request_limit);

            app.UseHttpsRedirection();
            app.UseMvc();
            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Force API V1");
            });
        }
    }
}
