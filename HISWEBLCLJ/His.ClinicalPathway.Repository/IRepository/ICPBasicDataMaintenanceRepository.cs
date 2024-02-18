using His.Entities;
using His.DAL;
using His.Entities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.ClinicalPathway.Repository
{
    /// <summary>
    /// 路径名称维护
    /// </summary>
    public interface ICPBasicDataMaintenanceRepository : IHisBaseService, IDependency
    {
        #region 路径名称
        /// <summary>
        /// 查询路径名称列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<object> GetListCpName(int page, int limit, string filter, RefAsync<int> total);
        /// <summary>
        /// 获取单个路径名称信息
        /// </summary>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        Task<object> GetSingleCpName(string ljbm);
        /// <summary>
        /// 保存单个路径名称
        /// </summary>
        /// <param name="ljmc"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        Task<DbResult<bool>> SaveCpName(CP_LJMC_VM ljmc, string[] col);
        /// <summary>
        /// 删除单个路径名称
        /// </summary>
        /// <param name="ljmc"></param>
        /// <returns></returns>
        Task<DbResult<bool>> DelCpName(CP_LJMC_VM ljmc);
        #endregion

        #region 科室路径
        /// <summary>
        /// 查询科室路径列表
        /// </summary>
        /// <param name="ksbm"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<object> GetListDeptCP(string ksbm, string ljbm, int page, int limit, string filter, RefAsync<int> total);
        /// <summary>
        /// 查询单个科室路径信息
        /// </summary>
        /// <param name="ljbm"></param>
        /// <param name="ksbm"></param>
        /// <returns></returns>
        Task<object> GetSingleDeptCp(string ljbm, string ksbm);
        /// <summary>
        /// 更新单个科室路径信息
        /// </summary>
        /// <param name="ksljold"></param>
        /// <param name="ksljnew"></param>
        /// <returns></returns>
        Task<DbResult<bool>> SaveDeptCp(CP_KSLJ_VM ksljold, CP_KSLJ_VM ksljnew);
        /// <summary>
        /// 删除单个科室路径信息
        /// </summary>
        /// <param name="ksjl"></param>
        /// <returns></returns>
        Task<DbResult<bool>> DelDeptCp(CP_KSLJ_VM ksjl);
        #endregion

        #region 路径病种
        /// <summary>
        /// 查询路径病种列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<object> GetListCpIcd(string ljbm, string jbbm, int page, int limit, string filter, RefAsync<int> total);
        /// <summary>
        /// 查询单个路径病种信息
        /// </summary>
        /// <param name="ljbm"></param>
        /// <param name="jbbm"></param>
        /// <returns></returns>
        Task<object> GetSingleCpIcd(string ljbm, string jbbm);
        /// <summary>
        /// 保存单个科室路径信息
        /// </summary>
        /// <param name="ljicdold"></param>
        /// <param name="ljicdnew"></param>
        /// <returns></returns>
        Task<DbResult<bool>> SaveCpIcd(CP_LJICD_VM ljicdold, CP_LJICD_VM ljicdnew);
        /// <summary>
        /// 删除单个科室路径信息
        /// </summary>
        /// <param name="ljicd"></param>
        /// <returns></returns>
        Task<DbResult<bool>> DelCpIcd(CP_LJICD_VM ljicd);
        #endregion

        #region 执行大类
        /// <summary>
        /// 获取执行大类列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<List<CP_ZXDL_VM>> GetListExecuteLargeClass(int page, int limit, string filter, RefAsync<int> total);
        /// <summary>
        /// 获取单个执行大类信息
        /// </summary>
        /// <param name="dlbm"></param>
        /// <returns></returns>
        Task<CP_ZXDL_VM> GetSingleExecuteLargeClass(string dlbm);
        /// <summary>
        /// 保存执行大类
        /// </summary>
        /// <param name="dl"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        Task<DbResult<bool>> SaveExecuteLargeClass(CP_ZXDL_VM dl, string[] col);
        /// <summary>
        /// 删除执行大类
        /// </summary>
        /// <param name="dl"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        Task<DbResult<bool>> DelExecuteLargeClass(CP_ZXDL_VM dl);
        #endregion

        #region 执行细类
        /// <summary>
        /// 获取执行细类列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<List<CP_ZXXL_VM>> GetListExecuteSubclass(int page, int limit, string filter, RefAsync<int> total);
        /// <summary>
        /// 获取单个执行细类信息
        /// </summary>
        /// <param name="xlbm"></param>
        /// <returns></returns>
        Task<CP_ZXXL_VM> GetSingleExecuteSubclass(string xlbm);
        /// <summary>
        /// 保存执行细类
        /// </summary>
        /// <param name="xl"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        Task<DbResult<bool>> SaveExecuteSubclass(CP_ZXXL_VM xl, string[] col);
        /// <summary>
        /// 删除执行细类
        /// </summary>
        /// <param name="xl"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        Task<DbResult<bool>> DelExecuteSubclass(CP_ZXXL_VM xl);
        #endregion

        
    }
}
