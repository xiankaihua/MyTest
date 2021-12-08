using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianKaiHua.Auxiliary;
using static XianKaiHua.Extensions.ServiceExtensions.CustomApiVersion;

namespace XianKaiHua.Extensions.Middlewares
{
    public static class SwaggerMildd
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="streamHtml">将swagger首页 自定义为我们的页面;记得这个字符串的写法：{项目名.index.html}</param>
        public static void UseSwaggerMildd(this IApplicationBuilder app, Func<Stream> streamHtml)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                #region 之前是写死的
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyWebAPI.MyServerce v1");
                //c.RoutePrefix = "";//路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件
                #endregion
                var ApiName = Appsettings.app(new string[] { "SwaggerSetup", "ApiName" });

                //根据版本名称倒序 遍历展示
                typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                {
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiName} {version}");
                });


                //c.SwaggerEndpoint($"https://petstore.swagger.io/v2/swagger.json", $"{ApiName} pet");

                // 将swagger首页，设置成我们自定义的页面，记得这个字符串的写法：{项目名.index.html}
                //if (streamHtml.Invoke() == null)
                //{
                //    var msg = "index.html的属性，必须设置为嵌入的资源";
                //    //日志记录log.Error(msg);
                //    throw new Exception(msg);
                //}
                //c.IndexStream = streamHtml;

                #region Id4
                //if (Permissions.IsUseIds4)
                //{
                //    c.OAuthClientId("blogadminjs");
                //}
                #endregion


                //路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉，
                //如果你想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "doc";
                c.RoutePrefix = "LinSook";
            });
        }

    }

}
