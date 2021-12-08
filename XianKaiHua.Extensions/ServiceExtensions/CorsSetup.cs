using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianKaiHua.Auxiliary;
using SqlSugar.Extensions;

namespace XianKaiHua.Extensions.ServiceExtensions
{
    public static class CorsSetup
    {
        public static void AddCorsSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddCors(options =>
            {
                if (!Appsettings.app(new string[] { "CorsSetup", "Cors", "EnableAllIPs" }).ObjToBool())
                {
                    options.AddPolicy(Appsettings.app(new string[] { "CorsSetup", "Cors", "PolicyName" }),
                        builder =>
                        {
                            builder.WithOrigins(Appsettings.app(new string[] { "CorsSetup", "Cors", "IPs" }).Split(','))
                            .AllowAnyHeader()//允许任意头
                            .AllowAnyMethod()// 允许任意方法
                            .AllowCredentials();//指定处理cookie
                        });
                }
                else
                {
                    //允许任意跨域请求
                    options.AddPolicy(Appsettings.app(new string[] { "CorsSetup", "Cors", "PolicyName" }), builder =>
                    {
                        //设定允许跨域的来源，有多个可以用','隔开
                        //builder.WithOrigins("http://localhost:21632")//指定跨域主机IP
                        builder.SetIsOriginAllowed((host) => true)// //允许任何来源的主机访问
                        .AllowAnyMethod()//允许任意方法
                        .AllowAnyHeader()//允许任意头
                        .AllowCredentials();//指定处理cookie
                    });
                }

            });
        }

    }
}
