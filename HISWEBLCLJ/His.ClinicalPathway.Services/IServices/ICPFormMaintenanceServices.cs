using His.Entities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace His.ClinicalPathway.Services
{
    /// <summary>
    /// 路径表单维护
    /// </summary>
    public interface ICPFormMaintenanceServices : IDependency
    {
        #region 路径阶段
        /// <summary>
        /// 查询路径阶段列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetListCpPhase(JsonObject data);
        /// <summary>
        /// 查询单个路径阶段信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetSingleCpPhase(JsonObject data);
        /// <summary>
        /// 保存路径阶段信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> SaveCpPhase(JsonObject data);
        /// <summary>
        /// 删除路径阶段
        /// </summary>
        /// <param name="ljjd"></param>
        /// <returns></returns>
        Task<string> DelCpPhase(JsonObject data);
        #endregion

        #region 路径项目
        /// <summary>
        /// 查询路径项目列表
        /// </summary>
        /// <returns></returns>
        Task<string> GetListCpPhaseProject(JsonObject data);
        /// <summary>
        /// 查询单个路径项目
        /// </summary>
        /// <returns></returns>
        Task<string> GetSingleCpProject(JsonObject data);
        /// <summary>
        /// 保存路径项目
        /// </summary>
        /// <returns></returns>
        Task<string> SaveCpProject(JsonObject data);
        /// <summary>
        /// 删除路径项目
        /// </summary>
        /// <returns></returns>
        Task<string> DelCpProject(JsonObject data);
        #endregion

        #region 治疗方案
        /// <summary>
        /// 查询治疗方案列表
        /// </summary>
        /// <returns></returns>
        Task<string> GetListCPTherapeuticSchedule(JsonObject data);
        /// <summary>
        /// 查询单个治疗方案信息
        /// </summary>
        /// <returns></returns>
        Task<string> GetSingleCPTherapeuticSchedule(JsonObject data);
        /// <summary>
        /// 保存治疗方案
        /// </summary>
        /// <returns></returns>
        Task<string> SaveCPTherapeuticSchedule(JsonObject data);
        /// <summary>
        /// 删除治疗方案
        /// </summary>
        /// <returns></returns>
        Task<string> DelCPTherapeuticSchedule(JsonObject data);
        #endregion

        #region 路径医嘱
        /// <summary>
        /// 查询路径项目医嘱列表
        /// </summary>
        /// <returns></returns>
        Task<string> GetListCpProjectMedicalAdvice(JsonObject data);
        /// <summary>
        /// 查询中药医嘱明细列表
        /// </summary>
        /// <returns></returns>
        Task<string> GetListCpChineseMedicalAdvice(JsonObject data);
        /// <summary>
        /// 查询单个医嘱
        /// </summary>
        /// <returns></returns>
        Task<string> GetSingleCpMedicalAdvice(JsonObject data);
        /// <summary>
        /// 保存医嘱项目
        /// </summary>
        /// <returns></returns>
        Task<string> SaveCpMedicalAdvice(JsonObject data);
        /// <summary>
        /// 保存中药医嘱
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> SaveCpChineseMedicalAdvice(JsonObject data);
        /// <summary>
        /// 删除医嘱项目
        /// </summary>
        /// <returns></returns>
        Task<string> DelCpMedicalAdvice(JsonObject data);
        /// <summary>
        /// 删除中药医嘱项目
        /// </summary>
        /// <returns></returns>
        Task<string> DelCpChineseMedicalAdvice(JsonObject data);
        #endregion

        #region 路径查询
        /// <summary>
        /// 查询科室路径名称列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetListCPNameByDept(JsonObject data);
        /// <summary>
        /// 查询路径下的阶段列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetCpPhase(JsonObject data);

        /// <summary>
        /// 查询路径下的项目列表
        /// </summary>
        /// <returns></returns>
        Task<string> GetListCpProject(JsonObject data);
        /// <summary>
        /// 路径下的医嘱项目
        /// </summary>
        /// <returns></returns>
        Task<string> GetListCpMedicalAdvice(JsonObject data);
        #endregion

        #region 路径表单

        #endregion
        #region 树型列表
        /// <summary>
        /// 查询路径下的阶段及项目
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetCpPhaseProjectTree(JsonObject data);
        /// <summary>
        /// 查询项目列表树
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetCpProjectTree(JsonObject data);
        #endregion
    }
}
