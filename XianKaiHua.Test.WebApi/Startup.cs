using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using XianKaiHua.Auxiliary;
using XianKaiHua.Extensions.Middlewares;
using XianKaiHua.Extensions.ServiceExtensions;

namespace XianKaiHua.Test.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        #region 启动类
        /// <summary>
        /// 创建数据库链接
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        #region 服务容器
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new Appsettings(Configuration));//获取相关配置信息类
            //services.AddControllersWithViews();//添加视图控件
            services.AddControllers();//添加控件API

            #region IOC依赖注入
            services.AddCustomIOC();
            //services.AddTransient<ITranTest, TranTest>();三种注入方式
            //services.AddSingleton<ISingTest, SingTest>();
            //services.AddScoped<ISconTest, SconTest>();
            #endregion

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

            #region 注册跨域服务 CORS跨域
            services.AddCorsSetup();
            #endregion

            #region Session注入
            //services.AddSession();
            #endregion

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

        #endregion

        #region 中间件
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
            app.UseSwaggerMildd(() => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("XianKaiHua.Test.WebApi.Index.html"));
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
        #endregion
    }
}
