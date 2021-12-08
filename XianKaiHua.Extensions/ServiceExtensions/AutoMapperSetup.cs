using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianKaiHua.Extensions.AutoMapper;

namespace XianKaiHua.Extensions.ServiceExtensions
{
    /// <summary>
    /// Automapper 映射 启动服务
    /// </summary>
    public static class AutoMapperSetup
    {
        public static void AddAutoMapperSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper(typeof(CustomAutoMapperProfile));
        }
    }

}
