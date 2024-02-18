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
    /// 医生路径管理
    /// </summary>
    public interface ICPManageRepository : IHisBaseService, IDependency
    {
        #region 入径管理
        /// <summary>
        /// 查询科室与疾病对应的路径名称
        /// </summary>
        /// <param name="ksbm"></param>
        /// <param name="jbbm"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<object> GetListCPNameForDeptIcd(string ksbm, string jbbm, int page, int limit, string filter, RefAsync<int> total);
        /// <summary>
        /// 判断科室、疾病是否有对应的路径名称
        /// </summary>
        /// <param name="ksbm"></param>
        /// <param name="jbbm"></param>
        /// <returns></returns>
        Task<int> JudgeCPNameForDeptIcd(string ksbm, string jbbm);
        /// <summary>
        /// 判断科室、疾病与路径是否匹配
        /// </summary>
        /// <param name="ksbm"></param>
        /// <param name="jbbm"></param>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        Task<int> JudgeCPNameForDeptIcd(string ksbm, string jbbm, string ljbm);
        /// <summary>
        /// 获取科室、疾病对应的路径名称
        /// </summary>
        /// <param name="ksbm"></param>
        /// <param name="jbbm"></param>
        /// <returns></returns>
        Task<List<CP_LJMC_VM>> GetListCPNameForDeptIcd(string ksbm, string jbbm);
        /// <summary>
        /// 查询路径对应的ICD编码列表
        /// </summary>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        Task<List<string>> GetListCpIcd(string ljbm);
        /// <summary>
        /// 入径登记保存
        /// </summary>
        /// <param name="djjl"></param>
        /// <returns></returns>
        Task<DbResult<bool>> SaveCPEntry(CP_DJJL_VM djjl);
        /// <summary>
        /// 更新登记信息
        /// </summary>
        /// <param name="djjl"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        Task<DbResult<bool>> UpdateCPEntry(CP_DJJL_VM djjl, string[] col);
        /// <summary>
        /// 保存退出路径申请
        /// </summary>
        /// <param name="qxjl"></param>
        /// <returns></returns>
        Task<DbResult<bool>> SaveCPCancelApply(CP_QXSQJL_VM qxjl);
        #endregion

        #region 路径执行
        /// <summary>
        /// 查询科室入径病人
        /// </summary>
        /// <param name="ksbm"></param>
        /// <param name="cybz">出院标志 0-在院 1-出院</param>
        /// <param name="ztbz">状态标志 0=正常在院 1=取消 2=变异 3=正常结束</param>
        /// <param name="rq1">查询开始日期</param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<object> GetListCPPatient(string ksbm, string cybz, string ztbz, string gcbr, string ysbm, DateTime? rq1, DateTime? rq2, int page, int limit, string filter, RefAsync<int> total);
        
        /// <summary>
        /// 病人执行路径阶段记录
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        Task<object> GetListPatientCPPhase(string zyh, string ljbm);
        /// <summary>
        /// 查询病人路径执行记录
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="ljbm"></param>
        /// <param name="jdbm"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<object> GetCPExecutionRecordDetails(string zyh, string ljbm, string jdbm, int page, int limit, string filter, RefAsync<int> total);
        /// <summary>
        /// 获取病人路径阶段评估记录
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="jdbm"></param>
        /// <returns></returns>
        Task<object> GetCPEvaluationRecord(string zyh, string jdbm);

        /// <summary>
        /// 查询病人评估记录
        /// </summary>
        /// <param name="zyh"></param>
        /// <returns></returns>
        Task<List<CP_PGJL_VM>> GetListPatientCPEvaluation(string zyh);
        /// <summary>
        /// 保存评估
        /// </summary>
        /// <param name="pgjl"></param>
        /// <param name="djjl"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        Task<DbResult<bool>> SaveCPEvaluation(CP_PGJL_VM pgjl, CP_DJJL_VM djjl, string[] col);
        /// <summary>
        /// 删除评估
        /// </summary>
        /// <param name="pgjl"></param>
        /// <returns></returns>
        Task<DbResult<bool>> DelCPEvaluation(CP_PGJL_VM pgjl);
        /// <summary>
        /// 查询路径和阶段对应的治疗方案列表
        /// </summary>
        /// <param name="ljbm"></param>
        /// <param name="jdbm"></param>
        /// <returns></returns>
        Task<object> GetListCPTherapeuticScheduleForCpcodePhase(string ljbm, string jdbm);
        /// <summary>
        /// 查询路径、阶段、治疗方案下的医嘱列表 
        /// </summary>
        /// <param name="listyzbm">医嘱编码列表</param>
        /// <returns></returns>
        Task<List<CPAdviceDetail_Model>> GetListCpMedicalAdviceExecution(List<string> listyzbm);
        /// <summary>
        /// 查询路径、阶段、治疗方案下的医嘱列表 执行路径使用
        /// </summary>
        /// <param name="ljbm">路径编码</param>
        /// <param name="jdbm">阶段编码</param>
        /// <param name="zlfa">治疗方案</param>
        /// <param name="listljxmbm"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<List<CPAdviceDetail_Model>> GetListCpMedicalAdvice(string ljbm, string jdbm, string zlfa, string zyh, List<string> listljxmbm, int page, int limit, string filter, RefAsync<int> total);
        /// <summary>
        /// 查询指定的医嘱列表 
        /// </summary>
        /// <param name="listyzbm">医嘱编码列表</param>
        /// <returns></returns>
        Task<List<CP_YZXM_VM>> GetListCpMedicalAdvice(List<string> listyzbm);
        /// <summary>
        /// 查询病人已保存的阶段医嘱列表
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="jdbm"></param>
        /// <returns></returns>
        Task<List<AdviceDetailSimple_Model>> GetListPatientAdvice(string zyh, string jdbm);

        /// <summary>
        /// 查询病人未停长期医嘱
        /// </summary>
        /// <param name="zyh"></param>
        /// <returns></returns>
        Task<List<INHOSD_YPYZMX_VM>> GetListPatientDrugAdvice(string zyh);
        /// <summary>
        /// 查询病人未停临期医嘱
        /// </summary>
        /// <param name="zyh"></param>
        /// <returns></returns>
        Task<List<INHOSD_YLYZMX_VM>> GetListPatientMedicalAdvice(string zyh);
        /// <summary>
        /// 保存执行
        /// </summary>
        /// <param name="listljxmzjjl"></param>
        /// <param name="listzxjl"></param>
        /// <param name="listylyz"></param>
        /// <param name="listylyzmx"></param>
        /// <param name="listypyz"></param>
        /// <param name="listypyzmx"></param>
        /// <param name="listypyzmx_tz"></param>
        /// <param name="listylyzmx_tz"></param>
        /// <returns></returns>
        Task<DbResult<bool>> CPPreExecution(List<CP_LJXM_ZXJL_VM> listljxmzjjl, List<CP_ZXJL_VM> listyzzxjl, List<INHOSD_YLYZ_VM> listylyz, List<INHOSD_YLYZMX_VM> listylyzmx, List<INHOSD_YPYZ_VM> listypyz, List<INHOSD_YPYZMX_VM> listypyzmx, List<INHOSD_YPYZMX_VM> listypyzmx_tz, List<INHOSD_YLYZMX_VM> listylyzmx_tz);
        #endregion

        #region 护士执行
        /// <summary>
        /// 根据住院号和路径编码获取护理评估记录
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        Task<List<CP_PGJL_HL_VM>> GetListNurseEvaluation(string zyh, string ljbm);
        /// <summary>
        /// 查询路径下的阶段以及是否有病人的评估
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        Task<object> GetListPhaseForCPCode(string zyh, string ljbm);
        /// <summary>
        /// 根据路径编码和阶段编码获取医生或护士路径项目
        /// </summary>
        /// <param name="ljbm"></param>
        /// <param name="jdbm"></param>
        /// <param name="xmzxr">执行人类别 1=医生 2=护士</param>
        /// <returns></returns>
        Task<List<CP_LJXM_VM>> GetListCPProject(string ljbm, string jdbm, string xmzxr);
        /// <summary>
        /// 查询病人路径执行记录
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="ljbm"></param>
        /// <param name="jdbm"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<object> GetNurseCPExecutionRecordDetails(string zyh, string ljbm, string jdbm, int page, int limit, string filter, RefAsync<int> total);
        /// <summary>
        /// 获取护士阶段评估记录
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="ljbm"></param>
        /// <param name="jddm"></param>
        /// <returns></returns>
        Task<object> GetSingleNurseCpEvaluate(string zyh, string ljbm, string jdbm);
        /// <summary>
        /// 执行护理临床路径
        /// </summary>
        /// <param name="pgjl"></param>
        /// <param name="listzxjl"></param>
        /// <returns></returns>
        Task<DbResult<bool>> NurseCPPreExecution(CP_PGJL_HL_VM pgjl, string[] col, List<CP_ZXJL_HL_VM> listzxjl);
        #endregion
    }
}
