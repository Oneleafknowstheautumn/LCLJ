using His.DAL;
using His.Entities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.ClinicalPathway.Repository
{
    public interface ICPPublicRepository : IHisBaseService, IDependency
    {
        /// <summary>
        /// 取单个路径对应的阶段
        /// </summary>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        Task<List<CP_LJJD_VM>> GetListPhaseForCPCode(string ljbm);
        /// <summary>
        /// 根据医嘱编码获取路径项目
        /// </summary>
        /// <param name="listyzbm"></param>
        /// <returns></returns>
        Task<List<CP_LJXM_VM>> GetListCPProjectForCodeList(List<string> listljxmbm);
        /// <summary>
        /// 查询科室路径名称列表
        /// </summary>
        /// <param name="ksbm"></param>
        /// <returns></returns>
        Task<object> GetListCPNameByDept(string ksbm, int page, int limit, string filter, RefAsync<int> total);
        /// <summary>
        /// 查询科室路径名称列表
        /// </summary>
        /// <param name="ksbm"></param>
        /// <returns></returns>
        Task<List<CP_LJMC_VM>> GetListCPNameByDept(string ksbm);
        /// <summary>
        /// 查询路径下的阶段
        /// </summary>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        Task<object> GetCpPhase(string ljbm, int page, int limit, string filter, RefAsync<int> total);
        /// <summary>
        /// 查询路径下的阶段及项目
        /// </summary>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        Task<List<CP_LJJD_VM>> GetCpPhase(string ljbm);
        /// <summary>
        /// 查询路径项目列表
        /// </summary>
        /// <param name="ljbm">路径编码</param>
        /// <returns></returns>
        Task<List<CP_LJXM_VM>> GetListCpProject(string ljbm);
        /// <summary>
        /// 查询路径项目列表
        /// </summary>
        /// <param name="ljbm">路径编码</param>
        /// <param name="jdbm">阶段编码</param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<object> GetListCpProject(string ljbm, string jdbm, int page, int limit, string filter, RefAsync<int> total);
        /// <summary>
        /// 查询路径医嘱列表
        /// </summary>
        /// <param name="ljbm"></param>
        /// <param name="jdbm"></param>
        /// <param name="xmbm"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<object> GetListCpMedicalAdvice(string ljbm, string jdbm, string xmbm, int page, int limit, string filter, RefAsync<int> total);
        /// <summary>
        /// 查询中药医嘱明细
        /// </summary>
        /// <param name="yzbm"></param>
        /// <returns></returns>
        Task<object> GetListCpChineseMedicalAdvice(string yzbm);
        #region 路径表单
        /// <summary>
        /// 根据路径编码查询路径阶段项目信息
        /// </summary>
        /// <param name="ljbm"></param>
        /// <param name="zyh"></param>
        /// <returns></returns>
        Task<object> GetListCpProject(string ljbm, string zyh);
        /// <summary>
        /// 查询病人路径执行记录
        /// </summary>
        /// <param name="ljbm"></param>
        /// <param name="zyh"></param>
        /// <returns></returns>
        Task<object> GetListPatientCpExecutionRecord(string ljbm, string zyh);
        /// <summary>
        /// 获取病人路径阶段评估记录
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="jdbm"></param>
        /// <returns></returns>
        Task<object> GetPatientCPEvaluationRecord(string ljbm, string zyh);
        #endregion

        #region 护理表单
        /// <summary>
        /// 根据路径编码查询护理路径阶段项目信息
        /// </summary>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        Task<object> GetListNurseCpProject(string ljbm, string zyh);
        /// <summary>
        /// 获取病人护理路径阶段评估记录
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="jdbm"></param>
        /// <returns></returns>
        Task<object> GetPatientNurseCPEvaluationRecord(string ljbm, string zyh);
        #endregion
    }
}
