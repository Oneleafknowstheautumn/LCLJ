using His.ClinicalPathway.Services;
using His.Entities.DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace HISWEBLCLJ.Controllers
{
    [Route("ClinicalPathway/[controller]")]
    [ApiController]
    public class CPBasicDataMaintenanceController : ControllerBase
    {
        private readonly ICPBasicDataMaintenanceServices _IServices;
        public CPBasicDataMaintenanceController(ICPBasicDataMaintenanceServices IServices)
        {
            _IServices = IServices;
        }
        #region 路径名称
        /// <summary>
        /// 查询路径名称列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetListCpName")]
        public async Task<string> GetListCpName([FromBody] InputEntity input)
        {
            return await _IServices.GetListCpName(input.Input);
        }
        /// <summary>
        /// 获取单个路径名称信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetSingleCpName")]
        public async Task<string> GetSingleCpName([FromBody] InputEntity input)
        {
            return await _IServices.GetSingleCpName(input.Input);
        }
        /// <summary>
        /// 保存单个路径名称
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("SaveCpName")]
        public async Task<string> SaveCpName([FromBody] InputEntity input)
        {
            return await _IServices.SaveCpName(input.Input);
        }
        /// <summary>
        /// 删除单个路径名称
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("DelCpName")]
        public async Task<string> DelCpName([FromBody] InputEntity input)
        {
            return await _IServices.DelCpName(input.Input);
        }
        #endregion

        #region 科室路径
        /// <summary>
        /// 查询科室路径列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetListDeptCP")]
        public async Task<string> GetListDeptCP([FromBody] InputEntity input)
        {
            return await _IServices.GetListDeptCP(input.Input);
        }
        /// <summary>
        /// 查询单个科室路径信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetSingleDeptCp")]
        public async Task<string> GetSingleDeptCp([FromBody] InputEntity input)
        {
            return await _IServices.GetSingleDeptCp(input.Input);
        }
        /// <summary>
        /// 保存科室路径
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("SaveDeptCp")]
        public async Task<string> SaveDeptCp([FromBody] InputEntity input)
        {
            return await _IServices.SaveDeptCp(input.Input);
        }
        /// <summary>
        /// 删除单个科室路径
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("DelDeptCp")]
        public async Task<string> DelDeptCp([FromBody] InputEntity input)
        {
            return await _IServices.DelDeptCp(input.Input);
        }
        #endregion

        #region 路径病种
        /// <summary>
        /// 查询路径病种列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetListCpIcd")]
        public async Task<string> GetListCpIcd([FromBody] InputEntity input)
        {
            return await _IServices.GetListCpIcd(input.Input);
        }
        /// <summary>
        /// 查询单个路径病种信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetSingleCpIcd")]
        public async Task<string> GetSingleCpIcd([FromBody] InputEntity input)
        {
            return await _IServices.GetSingleCpIcd(input.Input);
        }
        /// <summary>
        /// 保存单个科室路径信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("SaveCpIcd")]
        public async Task<string> SaveCpIcd([FromBody] InputEntity input)
        {
            return await _IServices.SaveCpIcd(input.Input);
        }
        /// <summary>
        /// 删除单个科室路径信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("DelCpIcd")]
        public async Task<string> DelCpIcd([FromBody] InputEntity input)
        {
            return await _IServices.DelCpIcd(input.Input);
        }
        #endregion

        #region 执行大类
        /// <summary>
        /// 获取执行大类列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetListExecuteLargeClass")]
        public async Task<string> GetListExecuteLargeClass([FromBody] InputEntity input)
        {
            return await _IServices.GetListExecuteLargeClass(input.Input);
        }
        /// <summary>
        /// 获取单个执行大类信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetSingleExecuteLargeClass")]
        public async Task<string> GetSingleExecuteLargeClass([FromBody] InputEntity input)
        {
            return await _IServices.GetSingleExecuteLargeClass(input.Input);
        }
        /// <summary>
        /// 保存执行大类
        /// </summary>
        /// <returns></returns>
        [HttpPost("SaveExecuteLargeClass")]
        public async Task<string> SaveExecuteLargeClass([FromBody] InputEntity input)
        {
            return await _IServices.SaveExecuteLargeClass(input.Input);
        }
        /// <summary>
        /// 删除执行大类
        /// </summary>
        /// <returns></returns>
        [HttpPost("DelExecuteLargeClass")]
        public async Task<string> DelExecuteLargeClass([FromBody] InputEntity input)
        {
            return await _IServices.DelExecuteLargeClass(input.Input);
        }
        #endregion

        #region 执行细类
        /// <summary>
        /// 获取执行细类列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetListExecuteSubclass")]
        public async Task<string> GetListExecuteSubclass([FromBody] InputEntity input)
        {
            return await _IServices.GetListExecuteSubclass(input.Input);
        }
        /// <summary>
        /// 获取单个执行细类信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetSingleExecuteSubclass")]
        public async Task<string> GetSingleExecuteSubclass([FromBody] InputEntity input)
        {
            return await _IServices.GetSingleExecuteSubclass(input.Input);
        }
        /// <summary>
        /// 保存执行细类
        /// </summary>
        /// <returns></returns>
        [HttpPost("SaveExecuteSubclass")]
        public async Task<string> SaveExecuteSubclass([FromBody] InputEntity input)
        {
            return await _IServices.SaveExecuteSubclass(input.Input);
        }
        /// <summary>
        /// 删除执行细类
        /// </summary>
        /// <returns></returns>
        [HttpPost("DelExecuteSubclass")]
        public async Task<string> DelExecuteSubclass([FromBody] InputEntity input)
        {
            return await _IServices.DelExecuteSubclass(input.Input);
        }
        #endregion
    }
}
