using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace XianKaiHua.IRepository
{
    public interface IBaseRepository<T> where T : class, new()
    {

        #region 仓储模式

        #region 增删改

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        Task<bool> CreateAsync(T Entity);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// 编辑(更新)
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        Task<bool> EditAsync(T Entity);
        #endregion

        #region 条件查询

        /// <summary>
        /// id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> FindAsync(int id);

        /// <summary>
        /// 自定义查询
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        Task<T> FindAsync(Expression<Func<T, bool>> Func);

        /// <summary>
        /// 查询全部的数据
        /// </summary>
        /// <returns></returns>
        Task<List<T>> QueryAsync();

        /// <summary>
        /// 自定义条件查询
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        Task<List<T>> QueryAsync(Expression<Func<T, bool>> Func);
        #endregion

        #region 分页查询

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<List<T>> QueryAsync(int page, int size, RefAsync<int> total);

        /// <summary>
        /// 自定义条件分页查询
        /// </summary>
        /// <param name="Func"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<List<T>> QueryAsync(Expression<Func<T, bool>> Func, int page, int size, RefAsync<int> total);
        #endregion
        #endregion


    }
}
