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
    /// 路径表单维护
    /// </summary>
    public interface ICPFormMaintenanceRepository : IHisBaseService, IDependency
    {
        #region 路径阶段
        /// <summary>
        /// 查询路径阶段列表
        /// </summary>
        /// <param name="ljbm"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<object> GetListCpPhase(string ljbm, int page, int limit, string filter, RefAsync<int> total);
        /// <summary>
        /// 查询单个路径阶段
        /// </summary>
        /// <param name="jdbm"></param>
        /// <returns></returns>
        Task<object> GetSingleCpPhase(string jdbm);
        /// <summary>
        /// 保存路径阶段
        /// </summary>
        /// <param name="ljjd"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        Task<DbResult<bool>> SaveCpPhase(CP_LJJD_VM ljjd, string[] col);
        /// <summary>
        /// 删除路径阶段
        /// </summary>
        /// <param name="ljjd"></param>
        /// <returns></returns>
        Task<DbResult<bool>> DelCpPhase(CP_LJJD_VM ljjd);
        #endregion

        #region 路径项目
        /// <summary>
        /// 查询单个路径项目
        /// </summary>
        /// <param name="xmbm"></param>
        /// <returns></returns>
        Task<CP_LJXM_VM> GetSingleCpProject(string xmbm);

        /// <summary>
        /// 保存路径项目
        /// </summary>
        /// <param name="ljxm"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        Task<DbResult<bool>> SaveCpProject(CP_LJXM_VM ljxm, string[] col);
        /// <summary>
        /// 删除路径项目
        /// </summary>
        /// <param name="ljxm"></param>
        /// <returns></returns>
        Task<DbResult<bool>> DelCpProject(CP_LJXM_VM ljxm);
        #endregion

        #region 治疗方案
        /// <summary>
        /// 查询治疗方案列表
        /// </summary>
        /// <param name="ljbm"></param>
        /// <param name="jdbm"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
       Task<object> GetListCPTherapeuticSchedule(string ljbm, string jdbm, int page, int limit, string filter, RefAsync<int> total);
        /// <summary>
        /// 查询单个治疗方案信息
        /// </summary>
        /// <param name="fabm"></param>
        /// <returns></returns>
        Task<object> GetSingleCPTherapeuticSchedule(string fabm);
        /// <summary>
        /// 保存治疗方案
        /// </summary>
        /// <param name="zlfa"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        Task<DbResult<bool>> SaveCPTherapeuticSchedule(CP_ZLFA_VM zlfa, string[] col);
        /// <summary>
        /// 删除治疗方案
        /// </summary>
        /// <param name="zlfa"></param>
        /// <returns></returns>
        Task<DbResult<bool>> DelCPTherapeuticSchedule(CP_ZLFA_VM zlfa);
        #endregion

        #region 路径医嘱
        /// <summary>
        /// 查询单个医嘱
        /// </summary>
        /// <param name="yzbm"></param>
        /// <returns></returns>
        Task<object> GetSingleCpMedicalAdvice(string yzbm);
        /// <summary>
        /// 保存医嘱项目
        /// </summary>
        /// <param name="yzxm"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        Task<DbResult<bool>> SaveCpMedicalAdvice(List<CP_YZXM_VM> yzxm, string[] col);
        /// <summary>
        /// 保存医嘱项目
        /// </summary>
        /// <param name="yzxm"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        Task<DbResult<bool>> SaveCpMedicalAdvice(CP_YZXM_VM yzxm, string[] col);
        /// <summary>
        /// 保存中药医嘱
        /// </summary>
        /// <param name="yzxm"></param>
        /// <param name="colyz"></param>
        /// <param name="listyzmx"></param>
        /// <returns></returns>
        Task<DbResult<bool>> SaveCpChineseMedicalAdvice(CP_YZXM_VM yzxm, string[] colyz, List<CP_YZXM_ZYMX_VM> listyzmx, string[] colyzmx);
        /// <summary>
        /// 删除医嘱项目
        /// </summary>
        /// <param name="yzxm"></param>
        /// <returns></returns>
        Task<DbResult<bool>> DelCpMedicalAdvice(CP_YZXM_VM yzxm);
        /// <summary>
        /// 删除医嘱项目 含中药明细
        /// </summary>
        /// <param name="yzxm"></param>
        /// <returns></returns>
        Task<DbResult<bool>> DelCpMedicalAdvice(CP_YZXM_VM yzxm, List<CP_YZXM_ZYMX_VM> listzymx);
        /// <summary>
        /// 删除中药医嘱项目
        /// </summary>
        /// <param name="yzxm"></param>
        /// <returns></returns>
        Task<DbResult<bool>> DelCpChineseMedicalAdvice(CP_YZXM_ZYMX_VM yzxm);
        #endregion

        #region 路径查询

        #endregion

        #region 路径表单

        #endregion
    }
}
