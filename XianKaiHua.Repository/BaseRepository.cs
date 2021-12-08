using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XianKaiHua.IRepository;

namespace XianKaiHua.Repository
{
    public class BaseRepository<T> : SimpleClient<T>, IBaseRepository<T> where T : class, new()
    {
        //Type[] EntityTypes = { "BlogNews", "TypeInfo", "WriterInfo" } //获取实体方法名称
        public BaseRepository(ISqlSugarClient context = null) : base(context)
        {
            base.Context = DbScoped.Sugar;
            // 创建数据库
            base.Context.DbMaintenance.CreateDatabase();
            // 创建表
            base.Context.CodeFirst.InitTables(
              //typeof(BlogNews),
              //typeof(TypeInfo),
              //typeof(WriterInfo)
              );
            //base.Context.CodeFirst.InitTables(EntityTypes);
        }

        #region 增删改
        public Task<bool> CreateAsync(T Entity)
        {
            return base.InsertAsync(Entity);
        }

        public Task<bool> DeleteAsync(int id)
        {
            return base.DeleteByIdAsync(id);
        }

        public Task<bool> EditAsync(T Entity)
        {
            return base.UpdateAsync(Entity);
        }
        #endregion

        #region 条件查询
        public Task<T> FindAsync(int id)
        {
            return base.GetByIdAsync(id);
        }

        public Task<T> FindAsync(Expression<Func<T, bool>> Func)
        {
            return base.GetSingleAsync(Func);
        }

        public Task<List<T>> QueryAsync()
        {
            return base.GetListAsync();
        }

        public Task<List<T>> QueryAsync(Expression<Func<T, bool>> Func)
        {
            return base.GetListAsync(Func);
        }
        #endregion

        #region 分页查询
        public Task<List<T>> QueryAsync(int page, int size, RefAsync<int> total)
        {
            return base.Context.Queryable<T>().ToPageListAsync(page, size, total);
        }

        public Task<List<T>> QueryAsync(Expression<Func<T, bool>> Func, int page, int size, RefAsync<int> total)
        {
            return base.Context.Queryable<T>().Where(Func).ToPageListAsync(page, size, total);
        }
        #endregion

        //#region 异步实现
        //#region 增删改
        //public virtual async Task<bool> CreateAsync(T Entity)
        //{
        //    return await base.InsertAsync(Entity);
        //}

        //public virtual async Task<bool> DeleteAsync(int id)
        //{
        //    return await base.DeleteByIdAsync(id);
        //}

        //public virtual async Task<bool> EditAsync(T Entity)
        //{
        //    return await base.UpdateAsync(Entity);
        //}
        //#endregion

        //#region 条件查询
        //public virtual async Task<T> FindAsync(int id)
        //{
        //    return await base.GetByIdAsync(id);
        //}

        //public virtual async Task<T> FindAsync(Expression<Func<T, bool>> Func)
        //{
        //    return await base.GetSingleAsync(Func);
        //}

        //public virtual async Task<List<T>> QueryAsync()
        //{
        //    return await base.GetListAsync();
        //}

        //public virtual async Task<List<T>> QueryAsync(Expression<Func<T, bool>> Func)
        //{
        //    return await base.GetListAsync(Func);
        //}
        //#endregion

        //#region 分页查询
        //public virtual async Task<List<T>> QueryAsync(int page, int size, RefAsync<int> total)
        //{
        //    return await base.Context.Queryable<T>().ToPageListAsync(page, size, total);
        //}

        //public virtual async Task<List<T>> QueryAsync(Expression<Func<T, bool>> Func, int page, int size, RefAsync<int> total)
        //{
        //    return await base.Context.Queryable<T>().Where(Func).ToPageListAsync(page, size, total);
        //}
        //#endregion

        //#endregion

    }
}
