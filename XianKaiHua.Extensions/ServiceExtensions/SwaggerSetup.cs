using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XianKaiHua.Auxiliary;
using static XianKaiHua.Extensions.ServiceExtensions.CustomApiVersion;

namespace XianKaiHua.Extensions.ServiceExtensions
{
        /// <summary>
        /// 自定义版本
        /// </summary>
        public class CustomApiVersion
        {
            /// <summary>
            /// Api接口版本 自定义
            /// </summary>
            public enum ApiVersions
            {
                /// <summary>
                /// v1 版本
                /// </summary>
                v1 = 1,
                /// <summary>
                /// v2 版本
                /// </summary>
                v2 = 2,
            }
        }

    /// <summary>
    /// Swagger 启动服务
    /// </summary>
    public static class SwaggerSetup
    {

        public static void AddSwaggerSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            var basePath = AppContext.BaseDirectory;
            //var basePath2 = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;

            var ApiName = Appsettings.app(new string[] { "SwaggerSetup", "ApiName" });//获取swagger相关配置

            services.AddSwaggerGen(c =>
            {

                //遍历出全部的版本，做文档信息展示
                typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    c.SwaggerDoc(version, new OpenApiInfo
                    {
                        Version = version,
                        Title = $"{ApiName}接口文档--{RuntimeInformation.FrameworkDescription}",
                        Description = $"{ApiName} HTTP API " + version,
                        //TermsOfService = uri,

                        Contact = new OpenApiContact { Name = ApiName, Email = "XianKaiHua.Test.WebApi.Core@xxx.com", Url = new Uri("https://www.jianshu.com/u/94102b59cc2a") },//邮箱 new Uri("https://neters.club")
                        License = new OpenApiLicense { Name = ApiName + " 官方文档", Url = new Uri("http://apk.neters.club/.doc/") }//文档
                    });
                    c.OrderActionsBy(o => o.RelativePath);
                });

                try
                {
                    #region 读取xml信息
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, "XianKaiHua.Test.WebApi.xml");//这个就是刚刚配置的xml文件名
                    c.IncludeXmlComments(xmlPath, true);//默认的第二个参数是false，这个是controller的注释，记得修改

                    var xmlModelPath = Path.Combine(AppContext.BaseDirectory, "XianKaiHua.Models.xml");//这个就是Model层的xml文件名
                    c.IncludeXmlComments(xmlModelPath, true);

                    #endregion
                }
                catch (Exception ex)
                {
                    //记录错误日志("MyServerce.xml和Model.xml 丢失，请检查并拷贝。\n" + ex.Message);
                }

                #region 开启加权小锁
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                //// 在header中添加token，传递到后台
                c.OperationFilter<SecurityRequirementsOperationFilter>();

                #endregion

                #region Swagger组件使用鉴权Jwt;Token绑定到

                //方案名称“Blog.Core”可自定义，上下一致即可
                #region Jwt
                //c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                // Jwt Bearer 认证，必须是 oauth2
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey,
                    Name = "Authorization",//jwt默认的参数名称
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）",
                    BearerFormat = "JWT",
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                #endregion

                #region Id4
                //接入identityserver4
                //c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                //{
                //    Type = SecuritySchemeType.OAuth2,
                //    Flows = new OpenApiOAuthFlows
                //    {
                //        Implicit = new OpenApiOAuthFlow
                //        {
                //            AuthorizationUrl = new Uri($"{Appsettings.app(new string[] { "SwaggerSetup", "IdentityServer4", "AuthorizationUrl" })}/connect/authorize"),
                //            Scopes = new Dictionary<string, string> {
                //                {
                //                    "Element.core.api","ApiResource id"
                //                }
                //            }
                //        }
                //    }
                //});
                #endregion

                //添加header验证信息;3.0不需要了,可以删掉
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //  {
                //    new OpenApiSecurityScheme
                //    {
                //      Reference=new OpenApiReference
                //      {
                //        Type=ReferenceType.SecurityScheme,
                //        Id="Bearer"
                //      }
                //    },
                //    new string[] {}
                //  }
                //});

                #endregion


                // ids4和jwt切换

            });

        }



    }

}
