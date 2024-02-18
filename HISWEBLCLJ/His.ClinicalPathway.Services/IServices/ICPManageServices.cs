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
    /// 医生路径管理
    /// </summary>
    public interface ICPManageServices : IDependency
    {
        #region 入径登记
        /// <summary>
        /// 查询查询科室与疾病对应的路径名称
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetListCPNameForDeptIcd(JsonObject data);
        /// <summary>
        /// 入径登记
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> SaveCPEntry(JsonObject data);
        /// <summary>
        /// 取消入径
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> CancelCPEntry(JsonObject data);
        #endregion

        #region 路径执行
        /// <summary>
        /// 查询科室入径病人
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetListCPPatient(JsonObject data);
        //取单个路径阶段

        /// <summary>
        /// 病人执行路径阶段记录
        /// </summary>
        /// <returns></returns>
        Task<string> GetListPatientCPPhase(JsonObject data);
        /// <summary>
        /// 查询病人路径执行记录
        /// </summary>
        /// <returns></returns>
        Task<string> GetCPExecutionRecordDetails(JsonObject data);
        /// <summary>
        /// 获取病人路径阶段评估记录
        /// </summary>
        /// <returns></returns>
        Task<string> GetCPEvaluationRecord(JsonObject data);
        /// <summary>
        /// 保存评估
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> SaveCPEvaluation(JsonObject data);
        /// <summary>
        /// 取消评估
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> CancelCPEvaluation(JsonObject data);
        /// <summary>
        /// 路径执行前判断
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> CPPreExecutionJudgment(JsonObject data);
        /// <summary>
        /// 查询路径、阶段、治疗方案下的医嘱列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetListCPMedicalAdvice(JsonObject data);
        /// <summary>
        /// 路径执行
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> CPPreExecution(JsonObject data);
        /// <summary>
        /// 查询病人表单信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetPatientForm(JsonObject data);
        #endregion

        #region 护士执行
        /// <summary>
        /// 根据住院号和路径编码获取路径阶段
        /// </summary>
        /// <returns></returns>
        Task<string> GetListCpProjectForPatientAndCpCode(JsonObject data);
        /// <summary>
        /// 查询病人路径护理执行信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetNurseCPExecutionRecordDetails(JsonObject data);
        /// <summary>
        /// 获取护士阶段评估记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetSingleNurseCpEvaluate(JsonObject data);
        /// <summary>
        /// 护理路径执行
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> NurseCPPreExecution(JsonObject data);
        /// <summary>
        /// 查询病人表单信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetPatientNurseForm(JsonObject data);
        #endregion
    }
}
