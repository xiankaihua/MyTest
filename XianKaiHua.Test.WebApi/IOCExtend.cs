using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using XianKaiHua.Auxiliary;
using XianKaiHua.IServices;
using XianKaiHua.Services;

namespace XianKaiHua.Test.WebApi
{
    /// <summary>
    /// IOC容器
    /// </summary>
    public static class IOCExtend
    {
        #region 容器服务
        /// <summary>
        /// 添加容器服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomIOC(this IServiceCollection services)
        {
            //接口与服务
            //services.AddScoped<IPersonServices, PersonServices>();
            //services.AddScoped<IMenuRepository, MenuRepository>();

            //services.AddScoped<IBasRepoService, BasRepoService>();
            //services.AddScoped<IBasRepoRepository, BasRepoRepository>();


            return services;
        }
        #endregion

        #region Jwt鉴权
        /// <summary>
        /// 添加鉴权服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomJWT(this IServiceCollection services)
        {
            #region 鉴权策略自定义

            #region 方式1
            // 1.这个很简单，其他什么都不用做，只需要在API层的controller上边，增加特性即可
            // [Authorize(Roles = "Admin,System")]
            #endregion

            #region 方式2
            // 2、这个和上边的异曲同工，好处就是不用在controller中，写多个 roles 。
            // 然后这么写 [Authorize(Policy = "Admin")]
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Client", policy => policy.RequireRole("Client").Build());
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                options.AddPolicy("SystemOrAdmin", policy => policy.RequireRole("Admin", "System"));
                options.AddPolicy("A_S_O", policy => policy.RequireRole("Admin", "System", "Others"));
            });
            #endregion

            #region 方式3

            #region 参数

            #region 配置文件认证服务

            //var Issuer = Appsettings.app(new string[] { "Audience", "Issuer" });
            //var Audience = Appsettings.app(new string[] { "Audience", "Audience" });
            //var Secret = Appsettings.app(new string[] { "Audience", "Secret" });//对称密钥为Base64
            //var keyByteArray = Encoding.ASCII.GetBytes(Secret);
            //var signingKey1 = new SymmetricSecurityKey(keyByteArray);
            //var signingCredentials1 = new SigningCredentials(signingKey1, SecurityAlgorithms.HmacSha256);

            #endregion

            // 如果要数据库动态绑定，这里先留个空，后边处理器里动态赋值
            //var permission = new List<PermissionItem>();
            //permission.Add(new PermissionItem { Url = "/api/Repo", Role = "Admin" });
            //// 角色与接口的权限要求参数
            ////(拒绝授权的跳转地址（目前无用）,权限集合,声明类型基于角色的授权,发行人,订阅人,签名凭据,接口的过期时间)
            //var permissionRequirement = new PermissionRequirement
            //    ("/api/home",permission,ClaimTypes.Role, "http://localhost:6060" + Issuer, "http://localhost:5000" + Audience, signingCredentials1, TimeSpan.FromSeconds(60 * 60));

            #endregion

            // 3、自定义复杂的策略授权
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy(Permissions.Name, policy => policy.Requirements.Add(permissionRequirement));
            //});
            #endregion

            #region 方式4
            //基于Scope策略授权
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Scope_BlogModule_Policy", builder =>
            //    {
            //        //客户端Scope中包含blog.core.api.BlogModule才能访问
            //        // 同时引用nuget包：IdentityServer4.AccessTokenValidation
            //        builder.RequireScope("blog.core.api.BlogModule");
            //    });
            //    // 其他 Scope 策略
            //    // ...
            //});
            #endregion

            #endregion

            #region 配置认证服务
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //令牌验证参数
            var signingKey = AccountHash.GetSecurityKey();
            // new SymmetricSecurityKey(Encoding.ASCII.GetBytes("sdfsdfsrty45634kkhllghtdgdfss345t678fs"));
            //var signingCredentials = AccountHash.GetTokenSecurityKey();

            var Issuer = Appsettings.app(new string[] { "Audience", "Issuer" });
            var Audience = Appsettings.app(new string[] { "Audience", "Audience" });

            #endregion

            #region 身份类型认证 core官方JWT认证自带
            // 开启Bearer认证
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                //options.DefaultForbidScheme = nameof();
            })
            //添加JwtBearer服务
            .AddJwtBearer(options =>
            {
                //options.RequireHttpsMetadata = false;//不使用HTTPS
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,//发行人
                    ValidateAudience = true,
                    ValidAudience = Audience,//订阅人
                    ValidateLifetime = true,//验证令牌生存期间
                    ClockSkew = TimeSpan.FromMinutes(60),//时间戳
                    RequireExpirationTime = true,//令牌是否有过期值
                };
                options.SaveToken = true;
                options.Events = new JwtBearerEvents()
                {
                    // 在安全令牌通过验证和ClaimsIdentity通过验证之后调用
                    // 如果用户访问注销页面
                    OnTokenValidated = context =>
                    {
                        if (context.Request.Path.Value.ToString() == "/account/logout")
                        {
                            var token = ((context as TokenValidatedContext).SecurityToken as JwtSecurityToken).RawData;
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.Response.Headers.Add("Token-Error", context.ErrorDescription);
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var jwtHandler = new JwtSecurityTokenHandler();
                        var token = context.Request.Headers["Authorization"].ObjToString().Replace("Bearer ", "");
                        if (string.IsNullOrEmpty(token) && jwtHandler.CanReadToken(token))
                        {
                            var jwtToken = jwtHandler.ReadJwtToken(token);

                            if (jwtToken.Issuer != "XianKaiHua")
                            {
                                context.Response.Headers.Add("Token-Error-Iss", "issuer is wrong!");
                            }

                            if (jwtToken.Audiences.FirstOrDefault() != "System")
                            {
                                context.Response.Headers.Add("Token-Error-Aud", "Audience is wrong!");
                            }
                        }


                        // 如果过期，则把<是否过期>添加到，返回头信息中
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            #endregion

            // 这里冗余写了一次,因为很多人看不到
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //添加 httpcontext 拦截 注入权限处理器
            //services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            //services.AddSingleton(permissionRequirement);
            return services;
        }

        #endregion



    }
}
