using Microsoft.Extensions.DependencyInjection;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianKaiHua.Auxiliary;
using XianKaiHua.Models.ApplyEntity;

namespace XianKaiHua.Extensions.ServiceExtensions
{
    public static class SqlSugarIocSetup
    {
        /// <summary>
        /// 多租户模式
        /// </summary>
        /// <param name="services"></param>
        public static void AddSqlSugarIocSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            
            var multilist = Appsettings.app<MultiDBS>(new string[] { "DBS" }).Where(quest => quest.Enabled).ToList();//获取All数据库配置信息
            List<IocConfig> listIocConfig = new List<IocConfig>();
            multilist.ForEach(multi =>
            listIocConfig.Add(new IocConfig()
            {
                ConfigId = multi.ConnId,
                ConnectionString = multi.Connection,
                DbType = (IocDbType)multi.DbType,
                IsAutoCloseConnection = true,
                //SlaveConnectionConfigs = 
            }));
            services.AddSqlSugar(listIocConfig);
        }
    }
}
