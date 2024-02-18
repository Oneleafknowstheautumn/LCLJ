using His.ClinicalPathway.Services;
using His.Entities.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace HISWEBLCLJ.Controllers
{
    [Route("ClinicalPathway/[controller]")]
    [ApiController]
    public class CPManageController : ControllerBase
    {
        private readonly ICPManageServices _IServices;
        public CPManageController(ICPManageServices IServices)
        {
            _IServices = IServices;
        }
        #region 入径登记
        /// <summary>
        /// 查询查询科室与疾病对应的路径名称
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetListCPNameForDeptIcd")]
        public async Task<string> GetListCPNameForDeptIcd([FromBody] InputEntity input)
        {
            return await _IServices.GetListCPNameForDeptIcd(input.Input);
        }
        /// <summary>
        /// 入径登记
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("SaveCPEntry")]
        public async Task<string> SaveCPEntry([FromBody] InputEntity input)
        {
            return await _IServices.SaveCPEntry(input.Input);
        }
        /// <summary>
        /// 取消入径
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("CancelCPEntry")]
        public async Task<string> CancelCPEntry([FromBody] InputEntity input)
        {
            return await _IServices.CancelCPEntry(input.Input);
        }
        #endregion

        #region 路径执行
        /// <summary>
        /// 查询科室入径病人
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetListCPPatient")]
        public async Task<string> GetListCPPatient([FromBody] InputEntity input)
        {
            return await _IServices.GetListCPPatient(input.Input);
        }
        //取单个路径阶段

        /// <summary>
        /// 病人执行路径阶段记录
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetListPatientCPPhase")]
        public async Task<string> GetListPatientCPPhase([FromBody] InputEntity input)
        {
            return await _IServices.GetListPatientCPPhase(input.Input);
        }
        /// <summary>
        /// 查询病人路径执行记录
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetCPExecutionRecordDetails")]
        public async Task<string> GetCPExecutionRecordDetails([FromBody] InputEntity input)
        {
            return await _IServices.GetCPExecutionRecordDetails(input.Input);
        }
        /// <summary>
        /// 获取病人路径阶段评估记录
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetCPEvaluationRecord")]
        public async Task<string> GetCPEvaluationRecord([FromBody] InputEntity input)
        {
            return await _IServices.GetCPEvaluationRecord(input.Input);
        }
        /// <summary>
        /// 保存评估
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("SaveCPEvaluation")]
        public async Task<string> SaveCPEvaluation([FromBody] InputEntity input)
        {
            return await _IServices.SaveCPEvaluation(input.Input);
        }
        /// <summary>
        /// 取消评估
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("CancelCPEvaluation")]
        public async Task<string> CancelCPEvaluation([FromBody] InputEntity input)
        {
            return await _IServices.CancelCPEvaluation(input.Input);
        }
        /// <summary>
        /// 路径执行前判断
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("CPPreExecutionJudgment")]
        public async Task<string> CPPreExecutionJudgment([FromBody] InputEntity input)
        {
            return await _IServices.CPPreExecutionJudgment(input.Input);
        }
        /// <summary>
        /// 查询路径、阶段、治疗方案下的医嘱列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetListCPMedicalAdvice")]
        public async Task<string> GetListCPMedicalAdvice([FromBody] InputEntity input)
        {
            return await _IServices.GetListCPMedicalAdvice(input.Input);
        }
        /// <summary>
        /// 路径执行
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("CPPreExecution")]
        public async Task<string> CPPreExecution([FromBody] InputEntity input)
        {
            return await _IServices.CPPreExecution(input.Input);
        }
        /// <summary>
        /// 查询病人表单信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetPatientForm")]
        public async Task<string> GetPatientForm([FromBody] InputEntity input)
        {
            return await _IServices.GetPatientForm(input.Input);
        }
        #endregion

        #region 护士执行
        /// <summary>
        /// 根据住院号和路径编码获取路径阶段
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetListCpProjectForPatientAndCpCode")]
        public async Task<string> GetListCpProjectForPatientAndCpCode([FromBody] InputEntity input)
        {
            return await _IServices.GetListCpProjectForPatientAndCpCode(input.Input);
        }
        /// <summary>
        /// 查询病人路径护理执行信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetNurseCPExecutionRecordDetails")]
        public async Task<string> GetNurseCPExecutionRecordDetails([FromBody] InputEntity input)
        {
            return await _IServices.GetNurseCPExecutionRecordDetails(input.Input);
        }
        /// <summary>
        /// 获取护士阶段评估记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetSingleNurseCpEvaluate")]
        public async Task<string> GetSingleNurseCpEvaluate([FromBody] InputEntity input)
        {
            return await _IServices.GetSingleNurseCpEvaluate(input.Input);
        }
        /// <summary>
        /// 护理路径执行
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("NurseCPPreExecution")]
        public async Task<string> NurseCPPreExecution([FromBody] InputEntity input)
        {
            return await _IServices.NurseCPPreExecution(input.Input);
        }
        /// <summary>
        /// 查询病人表单信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetPatientNurseForm")]
        public async Task<string> GetPatientNurseForm([FromBody] InputEntity input)
        {
            return await _IServices.GetPatientNurseForm(input.Input);
        }
        #endregion
    }
}
