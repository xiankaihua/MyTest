using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using SqlSugar.IOC;
//using SqlSugar;
//using Model;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using AutoMapper;
using Swashbuckle.AspNetCore.Swagger;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
//using BLL;
//using Common;
//using DAL;

using Autofac;
using Autofac.Extensions.DependencyInjection;
using System.Reflection;
using XianKaiHua.Auxiliary;
using XianKaiHua.Extensions.Middlewares;
using XianKaiHua.Extensions.ServiceExtensions;
using SqlSugar.Extensions;
//using Common.HttpContextUser;
//using Common.GlobalDefs;
//using Element.IServece;
//using Element.Servece;
//using Element.IRepository;
//using Element.Repository;
//using Element.Extensions.Middlewares;
//using Element.Extensions.ServiceExtensions;


namespace XianKaiHua.WebApi
{
    /// <summary>
    /// 启动
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 创建数据库链接
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 服务类
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton(new Appsettings(Configuration));//获取相关配置信息类
            //services.AddControllersWithViews();//添加视图控件
            services.AddControllers();//添加控件API


            #region IOC依赖注入
            services.AddCustomIOC();
            #endregion

            //services.AddTransient<ITranTest, TranTest>();
            //services.AddSingleton<ISingTest, SingTest>();
            //services.AddScoped<ISconTest, SconTest>();
            //services.AddScoped<IAService, AService>();

            #region HttpContext 相关服务
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddScoped<IAspNetUser, AspNetUser>();
            #endregion



            #region Swagger
            services.AddSwaggerSetup();
            #endregion

            #region SqlSugarIOC容器
            services.AddSqlSugar(new IocConfig()
            {
                ConnectionString = this.Configuration["ConnectionString"],
                DbType = IocDbType.SqlServer,
                IsAutoCloseConnection = true
            });
            #endregion

            #region JWT鉴权及策略
            services.AddCustomJWT();
            #endregion

            //services.AddTransient<IBaseNewService>();
            #region 注册跨域服务 CORS跨域
            services.AddCorsSetup();
            #endregion
            //services.AddMiniProfiler();//分析器

            #region Session注入
            //services.AddSession();
            #endregion


            //一个接口多类注入
            //services.AddScoped(factory => {
            //    Func<string, IDemoService>accesor = key =>{
            //        if (key.Equals("ServiceA"))
            //        {
            //            return factory.GetService<DemoServiceA>();
            //        }
            //        else if (key.Equals("ServiceB"))
            //        {
            //            return factory.GetService<DemoServiceB>();
            //        }
            //        else
            //        {
            //            throw new ArithmeticException($"not suuport key: {key}");
            //        }
            //    };
            //    return accesor;

            //});

            #region  添加映射 AutoMapper
            //services.AddAutoMapperSetup();//添加映射
            #endregion


            #region 自定义事件格式依赖注入
            //services.AddControllers().AddJsonOptions(opt =>
            //{
            //    opt.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());//后续添加
            //});
            #endregion

            #region Autofac
            //var builder = new ContainerBuilder();
            //builder.RegisterType<SingTest>().As<ISingTest>();
            //builder.RegisterType<AdvertisementServices>().As<IAdvertisementServices>();
            //builder.Populate(services);
            //var ApplicationContainer = builder.Build();
            //return new AutofacServiceProvider(ApplicationContainer);
            #endregion

        }


        #region Factory接管注入
        //public void ConfigureContainer(ContainerBuilder builder)
        //{

        //    #region Autofac 注入
        //    //builder.RegisterType<MenuServices>().As<IMenuServices>();
        //    //builder.RegisterType<MenuRepository>().As<IMenuRepository>();

        //    //builder.RegisterType<BasRepoService>().As<IBasRepoService>();
        //    //builder.RegisterType<BasRepoRepository>().As<IBasRepoRepository>();

        //    //builder.RegisterType<AdvertisementServices>().As<IAdvertisementServices>();
        //    //builder.RegisterType<AdvertisementRepository>().As<IAdvertisementRepository>();

        //    //builder.RegisterType<TranTest>().As<ITranTest>();
        //    //builder.RegisterType<SingTest>().As<ISingTest>();
        //    //builder.RegisterType<SconTest>().As<ISconTest>();
        //    //builder.RegisterType<AService>().As<IAService>();


        //    #region 整个DLL程序集批量注入

        //    #region 服务程序集注入方式--未解耦 1;注:未解耦;只有引用实现类，没有引用接口类

        //    //var assemblysServices = Assembly.Load("BLL");//要记得!!!这个注入的是实现类层，不是接口层！不是 IServices
        //    //builder.RegisterAssemblyTypes(assemblysServices).AsImplementedInterfaces();//指定已扫描程序集中的类型注册为提供所有其实现的接口。
        //    //var assemblysRepository = Assembly.Load("DAL");//模式是 Load(解决方案名)
        //    //builder.RegisterAssemblyTypes(assemblysRepository).AsImplementedInterfaces();

        //    ////1.接口形式注入
        //    ////2.没有接口实体方式注入
        //    ////3.没有接口的单独实体,只能注入该类的虚方法
        //    //builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(Love)));

        //    #endregion

        //    #region 服务程序集注入方式--解耦 2;注:解耦;只引用接口类，没有引用实现类;工程只依赖抽象

        //    //var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;//获取项目路径
        //    //var servicesDllFile = Path.Combine(basePath, "BLL.dll");//获取注入项目绝对路径
        //    //var assemblysServices1 = Assembly.LoadFile(servicesDllFile);//直接采用加载文件的方法
        //    //builder.RegisterAssemblyTypes(assemblysServices1).AsImplementedInterfaces();//指定已扫描程序集中的类型注册为提供所有其实现的接口。

        //    //var servicesDllFile2 = Path.Combine(basePath, "DAL.dll");//获取注入项目绝对路径
        //    //var assemblysServices2 = Assembly.LoadFile(servicesDllFile2);//直接采用加载文件的方法
        //    //builder.RegisterAssemblyTypes(assemblysServices2).AsImplementedInterfaces();//指定已扫描程序集中的类型注册为提供所有其实现的接口。

        //    #endregion


        //    #endregion


        //    #endregion
        //}

        #endregion



        /// <summary>
        /// 中间件
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region 中间件

            #region 添加Http请求
            app.UseHttpsRedirection();
            #endregion

            #region 添加静态文件
            app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //    Path.Combine(Directory.GetCurrentDirectory(), "upload")),
            //    RequestPath = "/upload",
            //    OnPrepareResponse = ctx =>
            //    {
            //        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=36000");
            //    }
            //});
            #endregion


            #region Swagger
            app.UseSwaggerMildd(() => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("XianKaiHua.WebApi.Index.html"));
            #endregion

            #region 使用cookie
            app.UseCookiePolicy();
            #endregion

            #region 使用Session
            //app.UseSession();
            #endregion

            #region 返回错误码
            app.UseStatusCodePages();//把错误码返回前台，比如是404
            #endregion

            app.UseRouting();

            #region 添加跨域
            //用 app.UserMvc() 或者 app.UseHttpsRedirection()中间件;app.UseCors() 写在它们的上边，先进行跨域，再进行 Http 请求，否则会提示跨域失败
            app.UseCors(Appsettings.app(new string[] { "CorsSetup", "Cors", "PolicyName" }));
            //app.UseCorsMidd(new HttpContext());//
            //app.UseMiddleware<CorsMidd>();
            #endregion

            #region 开启鉴权认证
            app.UseAuthentication();
            #endregion

            #region 授权
            app.UseAuthorization();
            #endregion

            #region 开启性能分析
            //app.UseMiniProfiler();
            #endregion

            #endregion

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
                endpoints.MapControllers();
                //endpoints.MapControllerRoute(name: "areas",pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                //endpoints.MapControllerRoute(name: "default",pattern: "{controller=Home}/{action=Index}");

                //<AspNetCoreHostingModel>InProcess</AspNetCoreHostingModel>//项目文件配置托管 InProcess托管 OutOfProcess托管(内、外部服务器)；InProcess托管仅有一个web服务器，内外部服务器之间不能通讯。也是请求吞吐量高的原因
                //System.Diagnostics.Process.GetCurrentProcess().ProcessName;//获取进程的名字
                //Kestrel服务器就是为ASP.NET Core打造的跨平台的web 服务器

            });
        }

        //public CorsFilter corsFilter()
        //{
        //     UrlBasedCorsConfigurationSource source = new UrlBasedCorsConfigurationSource();
        //     CorsConfiguration config = new CorsConfiguration();
        //    config.setAllowCredentials(true); // 允许cookies跨域
        //    config.addAllowedOrigin("*");// 允许向该服务器提交请求的URI，*表示全部允许。。这里尽量限制来源域，比如http://xxxx:8080 ,以降低安全风险。。
        //    config.addAllowedHeader("*");// 允许访问的头信息,*表示全部
        //    config.setMaxAge(18000L);// 预检请求的缓存时间（秒），即在这个时间段里，对于相同的跨域请求不会再预检了
        //    config.addAllowedMethod("*");// 允许提交请求的方法，*表示全部允许，也可以单独设置GET、PUT等
        //    source.registerCorsConfiguration("/**", config);
        //    return new CorsFilter(source);
        //}

        //public CorsWebFilter corsWebFilter()
        //{
        //    UrlBasedCorsConfigurationSource source = new UrlBasedCorsConfigurationSource();

        //    CorsConfiguration corsConfiguration = new CorsConfiguration();

        //    //1、配置跨域
        //    corsConfiguration.addAllowedHeader("*");
        //    corsConfiguration.addAllowedMethod("*");
        //    corsConfiguration.addAllowedOrigin("*");
        //    corsConfiguration.setAllowCredentials(true);

        //    source.registerCorsConfiguration("/**", corsConfiguration);
        //    return new CorsWebFilter(source);
        //}

        //public void addCorsMappings(CorsRegistry registry)
        //{
        //    registry.addMapping("/**").allowedOrigins("http://localhost" + ":9528")
        //            .allowCredentials(true);
        //}

    }

    /// <summary>
    /// IOC容器
    /// </summary>
    public static class IOCExtend
    {
        /// <summary>
        /// 添加容器服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomIOC(this IServiceCollection services)
        {
            //services.AddScoped<IMenuServices, MenuServices>();
            //services.AddScoped<IMenuRepository, MenuRepository>();

            //services.AddScoped<IBasRepoService, BasRepoService>();
            //services.AddScoped<IBasRepoRepository, BasRepoRepository>();

            //services.AddScoped<IUserRoleServices, UserRoleServices>();
            //services.AddScoped<IUserRoleRepository, UserRoleRepository>();

            //services.AddScoped<IAdvertisementServices, AdvertisementServices>();
            //services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();

            //services.AddScoped<ILogServices, LogServices>();
            //services.AddScoped<ILogRepositorys, LogRepositorys>();

            //services.AddScoped<IVueMenuServices, VueMenuServices>();
            return services;
        }

        #region 添加鉴权Jwt
        /// <summary>
        /// 鉴权服务
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

                            if (jwtToken.Issuer != "Myservice")
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
