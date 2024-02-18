using His.ClinicalPathway.Services;
using His.Entities.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace HISWEBLCLJ.Controllers
{
    /// <summary>
    /// 路径表单维护
    /// </summary>
    [Route("ClinicalPathway/[controller]")]
    [ApiController]
    public class CPFormMaintenanceController : ControllerBase
    {
        private readonly ICPFormMaintenanceServices _IServices;
        public CPFormMaintenanceController(ICPFormMaintenanceServices IServices)
        {
            _IServices = IServices;
        }
        #region 路径阶段
        /// <summary>
        /// 查询路径阶段列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetListCpPhase")]
        public async Task<string> GetListCpPhase([FromBody] InputEntity input)
        {
            return await _IServices.GetListCpPhase(input.Input);
        }
        /// <summary>
        /// 查询单个路径阶段信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetSingleCpPhase")]
        public async Task<string> GetSingleCpPhase([FromBody] InputEntity input)
        {
            return await _IServices.GetSingleCpPhase(input.Input);
        }
        /// <summary>
        /// 保存路径阶段信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("SaveCpPhase")]
        public async Task<string> SaveCpPhase([FromBody] InputEntity input)
        {
            return await _IServices.SaveCpPhase(input.Input);
        }
        /// <summary>
        /// 删除路径阶段
        /// </summary>
        /// <param name="ljjd"></param>
        /// <returns></returns>
        [HttpPost("DelCpPhasePhase")]
        public async Task<string> DelCpPhasePhase([FromBody] InputEntity input)
        {
            return await _IServices.DelCpPhase(input.Input);
        }
        #endregion

        #region 路径项目
        /// <summary>
        /// 查询路径项目列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetListCpPhaseProject")]
        public async Task<string> GetListCpPhaseProject([FromBody] InputEntity input)
        {
            return await _IServices.GetListCpPhaseProject(input.Input);
        }
        /// <summary>
        /// 查询单个路径项目
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetSingleCpProject")]
        public async Task<string> GetSingleCpProject([FromBody] InputEntity input)
        {
            return await _IServices.GetSingleCpProject(input.Input);
        }
        /// <summary>
        /// 保存路径项目
        /// </summary>
        /// <returns></returns>
        [HttpPost("SaveCpProject")]
        public async Task<string> SaveCpProject([FromBody] InputEntity input)
        {
            return await _IServices.SaveCpProject(input.Input);
        }
        /// <summary>
        /// 删除路径项目
        /// </summary>
        /// <returns></returns>
        [HttpPost("DelCpProject")]
        public async Task<string> DelCpProject([FromBody] InputEntity input)
        {
            return await _IServices.DelCpProject(input.Input);
        }
        #endregion

        #region 治疗方案
        /// <summary>
        /// 查询治疗方案列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetListCPTherapeuticSchedule")]
        public async Task<string> GetListCPTherapeuticSchedule([FromBody] InputEntity input)
        {
            return await _IServices.GetListCPTherapeuticSchedule(input.Input);
        }
        /// <summary>
        /// 查询单个治疗方案信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetSingleCPTherapeuticSchedule")]
        public async Task<string> GetSingleCPTherapeuticSchedule([FromBody] InputEntity input)
        {
            return await _IServices.GetSingleCPTherapeuticSchedule(input.Input);
        }
        /// <summary>
        /// 保存治疗方案
        /// </summary>
        /// <returns></returns>
        [HttpPost("SaveCPTherapeuticSchedule")]
        public async Task<string> SaveCPTherapeuticSchedule([FromBody] InputEntity input)
        {
            return await _IServices.SaveCPTherapeuticSchedule(input.Input);
        }
        /// <summary>
        /// 删除治疗方案
        /// </summary>
        /// <returns></returns>
        [HttpPost("DelCPTherapeuticSchedule")]
        public async Task<string> DelCPTherapeuticSchedule([FromBody] InputEntity input)
        {
            return await _IServices.DelCPTherapeuticSchedule(input.Input);
        }
        #endregion

        #region 路径医嘱
        /// <summary>
        /// 查询路径项目医嘱列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetListCpProjectMedicalAdvice")]
        public async Task<string> GetListCpProjectMedicalAdvice([FromBody] InputEntity input)
        {
            return await _IServices.GetListCpProjectMedicalAdvice(input.Input);
        }
        /// <summary>
        /// 查询中药医嘱明细列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetListCpChineseMedicalAdvice")]
        public async Task<string> GetListCpChineseMedicalAdvice([FromBody] InputEntity input)
        {
            return await _IServices.GetListCpChineseMedicalAdvice(input.Input);
        }
        /// <summary>
        /// 查询单个医嘱
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetSingleCpMedicalAdvice")]
        public async Task<string> GetSingleCpMedicalAdvice([FromBody] InputEntity input)
        {
            return await _IServices.GetSingleCpMedicalAdvice(input.Input);
        }
        /// <summary>
        /// 保存医嘱项目
        /// </summary>
        /// <returns></returns>
        [HttpPost("SaveCpMedicalAdvice")]
        public async Task<string> SaveCpMedicalAdvice([FromBody] InputEntity input)
        {
            return await _IServices.SaveCpMedicalAdvice(input.Input);
        }
        /// <summary>
        /// 保存中药医嘱
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("SaveCpChineseMedicalAdvice")]
        public async Task<string> SaveCpChineseMedicalAdvice([FromBody] InputEntity input)
        {
            return await _IServices.SaveCpChineseMedicalAdvice(input.Input);
        }
        /// <summary>
        /// 删除医嘱项目
        /// </summary>
        /// <returns></returns>
        [HttpPost("DelCpMedicalAdvice")]
        public async Task<string> DelCpMedicalAdvice([FromBody] InputEntity input)
        {
            return await _IServices.DelCpMedicalAdvice(input.Input);
        }
        /// <summary>
        /// 删除单个中药医嘱项目
        /// </summary>
        /// <returns></returns>
        [HttpPost("DelCpChineseMedicalAdvice")]
        public async Task<string> DelCpChineseMedicalAdvice([FromBody] InputEntity input)
        {
            return await _IServices.DelCpChineseMedicalAdvice(input.Input);
        }
        #endregion

        #region 路径查询
        /// <summary>
        /// 查询科室路径名称列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetListCPNameByDept")]
        public async Task<string> GetListCPNameByDept([FromBody] InputEntity input)
        {
            return await _IServices.GetListCPNameByDept(input.Input);
        }
        /// <summary>
        /// 查询路径下的阶段列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetCpPhase")]
        public async Task<string> GetCpPhase([FromBody] InputEntity input)
        {
            return await _IServices.GetCpPhase(input.Input);
        }

        /// <summary>
        /// 查询路径下的项目列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetListCpProject")]
        public async Task<string> GetListCpProject([FromBody] InputEntity input)
        {
            return await _IServices.GetListCpProject(input.Input);
        }
        /// <summary>
        /// 路径下的医嘱项目
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetListCpMedicalAdvice")]
        public async Task<string> GetListCpMedicalAdvice([FromBody] InputEntity input)
        {
            return await _IServices.GetListCpMedicalAdvice(input.Input);
        }
        #endregion

        #region 路径表单

        #endregion
        #region 树型列表
        /// <summary>
        /// 查询路径下的阶段及项目
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetCpPhaseProjectTree")]
        public async Task<string> GetCpPhaseProjectTree([FromBody] InputEntity input)
        {
            return await _IServices.GetCpPhaseProjectTree(input.Input);
        }
        /// <summary>
        /// 查询项目列表树
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetCpProjectTree")]
        public async Task<string> GetCpProjectTree([FromBody] InputEntity input)
        {
            return await _IServices.GetCpProjectTree(input.Input);
        }
        #endregion
    }
}
