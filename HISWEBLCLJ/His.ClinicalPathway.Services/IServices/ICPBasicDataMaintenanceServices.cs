using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using His.Entities;
using SqlSugar;

namespace His.ClinicalPathway.Services
{
    /// <summary>
    /// 路径名称维护
    /// </summary>
    public interface ICPBasicDataMaintenanceServices : IDependency
    {
        #region 路径名称
        /// <summary>
        /// 查询路径名称列表
        /// </summary>
        /// <returns></returns>
        Task<string> GetListCpName(JsonObject data);
        /// <summary>
        /// 获取单个路径名称信息
        /// </summary>
        /// <returns></returns>
        Task<string> GetSingleCpName(JsonObject data);
        /// <summary>
        /// 保存单个路径名称
        /// </summary>
        /// <returns></returns>
        Task<string> SaveCpName(JsonObject data);
        /// <summary>
        /// 删除单个路径名称
        /// </summary>
        /// <returns></returns>
        Task<string> DelCpName(JsonObject data);
        #endregion

        #region 科室路径
        /// <summary>
        /// 查询科室路径列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetListDeptCP(JsonObject data);
        /// <summary>
        /// 查询单个科室路径信息
        /// </summary>
        /// <returns></returns>
        Task<string> GetSingleDeptCp(JsonObject data);
        /// <summary>
        /// 保存科室路径
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> SaveDeptCp(JsonObject data);
        /// <summary>
        /// 删除单个科室路径
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> DelDeptCp(JsonObject data);
        #endregion

        #region 路径病种
        /// <summary>
        /// 查询路径病种列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetListCpIcd(JsonObject data);
        /// <summary>
        /// 查询单个路径病种信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetSingleCpIcd(JsonObject data);
        /// <summary>
        /// 保存单个科室路径信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> SaveCpIcd(JsonObject data);
        /// <summary>
        /// 删除单个科室路径信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> DelCpIcd(JsonObject data);
        #endregion

        #region 执行大类
        /// <summary>
        /// 获取执行大类列表
        /// </summary>
        /// <returns></returns>
        Task<string> GetListExecuteLargeClass(JsonObject data);
        /// <summary>
        /// 获取单个执行大类信息
        /// </summary>
        /// <returns></returns>
        Task<string> GetSingleExecuteLargeClass(JsonObject data);
        /// <summary>
        /// 保存执行大类
        /// </summary>
        /// <returns></returns>
        Task<string> SaveExecuteLargeClass(JsonObject data);
        /// <summary>
        /// 删除执行大类
        /// </summary>
        /// <returns></returns>
        Task<string> DelExecuteLargeClass(JsonObject data);
        #endregion

        #region 执行细类
        /// <summary>
        /// 获取执行细类列表
        /// </summary>
        /// <returns></returns>
        Task<string> GetListExecuteSubclass(JsonObject data);
        /// <summary>
        /// 获取单个执行细类信息
        /// </summary>
        /// <returns></returns>
        Task<string> GetSingleExecuteSubclass(JsonObject data);
        /// <summary>
        /// 保存执行细类
        /// </summary>
        /// <returns></returns>
        Task<string> SaveExecuteSubclass(JsonObject data);
        /// <summary>
        /// 删除执行细类
        /// </summary>
        /// <returns></returns>
        Task<string> DelExecuteSubclass(JsonObject data);
        #endregion
    }
}
