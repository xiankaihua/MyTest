using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianKaiHua.Auxiliary;
using XianKaiHua.IRepository;

namespace XianKaiHua.Repository
{
    public class DbsContext<T> : SimpleClient<T>, IDbsContext<T> where T : class, new()
    {
        private static string ConnectionType = Appsettings.app(new string[] { "ConnectionType" });  //数据库类型
        private static string SqlConn = Appsettings.app(new string[] { "SqlConn", ConnectionType });
        public DbsContext(ISqlSugarClient context = null) : base(context)
        {
            if (context == null)
            {
                DbType XianKaiHuaPost = SqlSugar.DbType.PostgreSQL;
                base.Context = new SqlSugarClient(new ConnectionConfig()
                {
                    //DbType = SqlSugar.DbType.PostgreSQL,
                    DbType = XianKaiHuaPost,
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true,
                    ConnectionString = SqlConn,
                    MoreSettings = new ConnMoreSettings()
                    {
                        PgSqlIsAutoToLower = false
                    },
                });
            }
        }
    }

}
