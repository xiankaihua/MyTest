using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianKaiHua.Auxiliary;
using XianKaiHua.Extensions.ServiceExtensions;
using XianKaiHua.IServices;
using XianKaiHua.Services;

namespace XianKaiHua.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new Appsettings(Configuration));//获取相关配置信息类
            services.AddControllersWithViews();//添加视图控件
            //services.AddControllers();//添加控件API

            services.AddScoped<IPersonServices, PersonServices>();
            //services.AddRazorPages();

            #region SqlSugarIOC容器
            services.AddSqlSugarIocSetup();
            //services.AddSqlSugar(new IocConfig()
            //{
            //    ConnectionString = this.Configuration["ConnectionString"],
            //    DbType = IocDbType.SqlServer,
            //    IsAutoCloseConnection = true
            //});
            #endregion


            #region HttpContext 相关服务
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //else
            //{
            //    app.UseExceptionHandler("/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

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

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                //endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "Areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }

    /// <summary>
    /// IOC容器
    /// </summary>
    public static class IOCExtend
    {
        public static IServiceCollection AddCustomJWT(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                  .AddJwtBearer(options =>
                  {
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuerSigningKey = true,
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SDMC-CJAS1-SAD-DFSFA-SADHJVF-VF")),
                          ValidateIssuer = true,
                          ValidIssuer = "http://localhost:6060",
                          ValidateAudience = true,
                          ValidAudience = "http://localhost:5000",
                          ValidateLifetime = true,
                          ClockSkew = TimeSpan.FromMinutes(60)
                      };
                  });
            return services;
        }
    }
}
