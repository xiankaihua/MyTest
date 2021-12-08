using SqlSugar;
using SqlSugar.Extensions;
using SqlSugar.IOC;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
//using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XianKaiHua.Auxiliary;
using XianKaiHua.Models.ApplyEntity;
using XianKaiHua.Models.Entity;

namespace XianKaiHua.Repository
{
    public class BaseRepositoryMultlTenant<T> : SimpleClient<T> where T : class, new()
    {
        #region 建库对象
        //SqlSugarClient
        private static SqlSugarScope _db;
        protected ITenant itenant = null;//多租户事务 
        
        #endregion

        #region 建仓
        public BaseRepositoryMultlTenant(ISqlSugarClient context = null) : base(context)//注意这里要有默认值等于null
        {

            #region 多库配置操作
            var listConfig = new List<ConnectionConfig>();              //主库配置
            var listConfig_Slave = new List<SlaveConnectionConfig>();   //从库配置

            var multilist = Appsettings.app<MultiDBS>(new string[] { "DBS" }).Where(quest => quest.Enabled).ToList();//获取All数据库配置信息

            //筛选从库
            var multilist_slave = multilist.Where(it => it.IsMaster == false).ToList();
            multilist_slave.ForEach(multslavdb => listConfig_Slave.Add(new SlaveConnectionConfig()
            {
                HitRate = multslavdb.HitRate,//优先级别,数子越大有优先
                ConnectionString = multslavdb.Connection
            }));

            //使用主库
            //.Where(it => it.IsMaster == true).ToList()
            multilist.ForEach(multdb => listConfig.Add(new ConnectionConfig()
            {
                ConfigId = multdb.ConnId,
                DbType = multdb.DbType,
                ConnectionString = multdb.Connection,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                MoreSettings = new ConnMoreSettings()
                {
                    PgSqlIsAutoToLower = true,//自动转小写
                    IsAutoRemoveDataCache = true,//自动移除数据缓存
                },
                //自定义配置外部服务特性
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    EntityService = (property, column) =>
                    {
                        //column.ColumnDescription = "";
                        Console.WriteLine(column.EntityName);
                        Console.WriteLine(column.DbColumnName);
                        Console.WriteLine(column.DbTableName);
                        Console.WriteLine(column.ColumnDescription);
                        //是主键且类型为int型主键设置自增量
                        //if (column.IsPrimarykey && property.PropertyType == typeof(int))
                        //{
                        //    column.IsIdentity = true;
                        //}
                    }
                },
                //从库(子库)
                //SlaveConnectionConfigs = listConfig_Slave,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        string key = "【SQL参数】：";
                        foreach (var param in p)
                        {
                            key += $"{param.ParameterName}:{param.Value}\n";
                        }
                        //MiniProfiler.Current.CustomTiming("SQL：", key + "【SQL语句】：" + sql);
                        Console.WriteLine("SQL：" + key  + "【SQL语句】：" + sql);
                    },
                },
            }));
            #endregion

            #region 多库创建
            //方式1
            //_db = new SqlSugarScope(listConfig);//创建数据库


            //方式2(记日志)
            //_db = new SqlSugarScope(listConfig, db =>
            //{
            //    //里面可以循环
            //    db.GetConnection("0").Aop.OnLogExecuting = (sql, p) =>
            //    {
            //        Console.WriteLine(sql);
            //    };
            //    db.GetConnection("1").Aop.OnLogExecuting = (sql, p) =>
            //    {
            //        Console.WriteLine(sql);
            //    };
            //});

            //方式3
            base.Context = new SqlSugarScope(listConfig);

            //方式4
            //ISqlSugarClient _Idb = new SqlSugarScope(listConfig);
            #endregion

            #region 切换库
            //_db.IsAnyConnection(configId);//可以验证有没有当前连接
            //通过特性拿到ConfigId
            //var configId = typeof(T).GetCustomAttribute<TenantAttribute>().configId;
            //切换数据库 ConfigId = 1
            //base.Context = DbScoped.SugarScope.GetConnection(configId);
            //base.Context = SqlSugarClient.ChangeDatabase(configId); //改变db.的默认数据库
            //_db.ChangeDatabase(configId); //改变db.的默认数据库
            #endregion


            #region 建库(首次创建之后需去掉)

            #region 建仓
            //方式1
            base.Context.DbMaintenance.CreateDatabase();//

            //方式2
            //_db.DbMaintenance.CreateDatabase();//多库默认操作首个连接的库
            #endregion

            #region 建表
            //方式1
            base.Context.CodeFirst.InitTables(typeof(person));
            
            //方式2
            //_db.CodeFirst.InitTables(typeof(person));
            #endregion

            #endregion


            ////方式1：SqlSugar.Ioc用法
            //base.Context = DbScoped.SugarScope.GetConnection(configId);//子Db无租户方法，其他功能都有
            //itenant = DbScoped.SugarScope;//设置租户接口

            //方式2：Furion框架中用
            //base.Context=  App.GetService<ISqlSugarClient>().AsTenant().GetConnection(configId); 
            //itenant = App.GetService<ISqlSugarClient>().AsTenant() ; 


            //方式3:不用Ioc
            //base.Context=单例的SqlSugarScope.GetConnection(configId);
            //itenant=单例的SqlSugarScope;


            //方式4:.NET CORE自带的IOC 不推荐
            //需要自个封装成 SqlSugar.IOC或者Furion框架那种形式赋值，不能用构造函数注入

        }
        #endregion


        //实体加上标识;区分走哪个库 
        //[TenantAttribute("1")]
        //public class C1Table
        //{
        //    public string Id { get; set; }
        //}
    }
}
