using His.ClinicalPathway.Repository;
using His.Core;
using His.DAL;
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
    /// 路径名称维护
    /// </summary>
    public class CPBasicDataMaintenanceServices : ICPBasicDataMaintenanceServices
    {
        private readonly ICPBasicDataMaintenanceRepository _Repository;
        private readonly IJsonMethod _Json;
        private readonly IPublicYwxhRepository _Ywxh;
        public CPBasicDataMaintenanceServices(ICPBasicDataMaintenanceRepository IRepository, IJsonMethod IJson, IPublicYwxhRepository IYwxh)
        {
            _Repository = IRepository;
            _Json = IJson;
            _Ywxh = IYwxh;
        }
        #region 路径名称
        /// <summary>
        /// 查询路径名称列表
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetListCpName(JsonObject data)
        {
            int page = _Json.InInt(data["page"]);
            int limit = _Json.InInt(data["limit"]);
            string filter = _Json.InString(data["filterstr"]);
            RefAsync<int> total = 0;
            var list = await _Repository.GetListCpName(page,limit,filter,total);
            return _Json.GetResults(page,limit,total,list);
        }
        /// <summary>
        /// 获取单个路径名称信息
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetSingleCpName(JsonObject data)
        {
            string ljbm = _Json.InString(data["ljbm"]);
            if(string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能查询！");
            }
            return _Json.GetResults(await _Repository.GetSingleCpName(ljbm));
        }
        /// <summary>
        /// 保存单个路径名称
        /// </summary>
        /// <returns></returns>
        public async Task<string> SaveCpName(JsonObject data)
        {
            StringBuilder msg = new StringBuilder();
            CP_LJMC_VM ljmc = _Json.InEntities<CP_LJMC_VM>(data, msg);
            if(ljmc == null)
            {
                return _Json.GetErrMsg("路径名称信息为空" + msg.ToString());
            }

            if (string.IsNullOrWhiteSpace(ljmc.ljbm))
            {
                return _Json.GetErrMsg("请输入路径名称！");
            }

            CP_LJMC_VM ljmcold = await _Repository.GetEntity<CP_LJMC_VM>(a => a.ljbm == ljmc.ljbm);

            if (ljmcold == null) //新增
            {
                if (string.IsNullOrWhiteSpace(ljmc.ljmc))
                {
                    return _Json.GetErrMsg("请输入路径名称！");
                }
                if (string.IsNullOrWhiteSpace(ljmc.fl))
                {
                    return _Json.GetErrMsg("请指定路径类别！");
                }
                if (string.IsNullOrWhiteSpace(ljmc.blfx))
                {
                    return _Json.GetErrMsg("请指定病例分型！");
                }
                if (string.IsNullOrWhiteSpace(ljmc.sybq))
                {
                    return _Json.GetErrMsg("请指定适用病情！");
                }
                if (string.IsNullOrWhiteSpace(ljmc.syxb))
                {
                    return _Json.GetErrMsg("请指定适用性别！");
                }
                if (ljmc.bzts <= 0)
                {
                    return _Json.GetErrMsg("请输入标准住院天数！");
                }
                if (string.IsNullOrWhiteSpace(ljmc.sydx))
                {
                    return _Json.GetErrMsg("请输入适用对象！");
                }
                if (string.IsNullOrWhiteSpace(ljmc.bzzyr))
                {
                    return _Json.GetErrMsg("请输入标准住院日范围！");
                }
            }
            else //修改
            {
                if (data["ljmc"] != null && string.IsNullOrWhiteSpace(ljmc.ljmc))
                {
                    return _Json.GetErrMsg("请输入路径名称！");
                }
                if (data["fl"] != null && string.IsNullOrWhiteSpace(ljmc.fl))
                {
                    return _Json.GetErrMsg("请指定路径类别！");
                }
                if (data["blfx"] != null && string.IsNullOrWhiteSpace(ljmc.blfx))
                {
                    return _Json.GetErrMsg("请指定病例分型！");
                }
                if (data["sybq"] != null && string.IsNullOrWhiteSpace(ljmc.sybq))
                {
                    return _Json.GetErrMsg("请指定适用病情！");
                }
                if (data["syxb"] != null && string.IsNullOrWhiteSpace(ljmc.syxb))
                {
                    return _Json.GetErrMsg("请指定适用性别！");
                }
                if (data["bzts"] != null && ljmc.bzts <= 0)
                {
                    return _Json.GetErrMsg("请输入标准住院天数！");
                }
                if (data["sydx"] != null && string.IsNullOrWhiteSpace(ljmc.sydx))
                {
                    return _Json.GetErrMsg("请输入适用对象！");
                }
                if (data["bzzyr"] != null && string.IsNullOrWhiteSpace(ljmc.bzzyr))
                {
                    return _Json.GetErrMsg("请输入标准住院日范围！");
                }
            }

            DbResult<bool> result = await _Repository.SaveCpName(ljmc,_Json.ToJsonKeyArr(data));
            return _Json.GetResults(result);
        }
        /// <summary>
        /// 删除单个路径名称
        /// </summary>
        /// <returns></returns>
        public async Task<string> DelCpName(JsonObject data)
        {
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能删除！");
            }
            CP_LJMC_VM ljmc = await _Repository.GetEntity<CP_LJMC_VM>(a => a.ljbm == ljbm);
            if (ljmc == null)
            {
                return _Json.GetErrMsg($"未查询到编码：{ljbm}的路径名称信息！");
            }

            return _Json.GetResults(await _Repository.DelCpName(ljmc));
        }
        #endregion

        #region 科室路径
        /// <summary>
        /// 查询科室路径列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetListDeptCP(JsonObject data)
        {
            int page = _Json.InInt(data["page"]);
            int limit = _Json.InInt(data["limit"]);
            string filter = _Json.InString(data["filterstr"]);
            string ksbm = _Json.InString(data["ksbm"]);
            string ljbm = _Json.InString(data["ljbm"]);
            RefAsync<int> total = 0;
            var list = await _Repository.GetListDeptCP(ksbm, ljbm, page, limit, filter, total);
            return _Json.GetResults(page, limit, total, list);
        }
        /// <summary>
        /// 查询单个科室路径信息
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetSingleDeptCp(JsonObject data)
        {
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能查询！");
            }
            string ksbm = _Json.InString(data["ksbm"]);
            if (string.IsNullOrWhiteSpace(ksbm))
            {
                return _Json.GetErrMsg("科室编码为空，不能查询！");
            }
            return _Json.GetResults(await _Repository.GetSingleDeptCp(ljbm,ksbm));
        }
        /// <summary>
        /// 保存科室路径
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> SaveDeptCp(JsonObject data)
        {
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能保存！");
            }
            string ksbm = _Json.InString(data["ksbm"]);
            if (string.IsNullOrWhiteSpace(ksbm))
            {
                return _Json.GetErrMsg("科室编码为空，不能保存！");
            }

            CP_KSLJ_VM kslj = await _Repository.GetEntity<CP_KSLJ_VM>(a => a.ksbm == ksbm && a.ljbm == ljbm);
            if (kslj != null)
            {
                return _Json.GetErrMsg($"科室编码：{ksbm}，路径编码：{ljbm}对应关系已经存在！");
            }

            string ljbm_old = _Json.InString(data["ljbm_old"]); //旧路径编码
            CP_KSLJ_VM ksljnew = new CP_KSLJ_VM();
            ksljnew.ksbm = ksbm;
            ksljnew.ljbm = ljbm;

            if (!string.IsNullOrWhiteSpace(ljbm_old)) //更新
            {
                CP_KSLJ_VM ksljold = new CP_KSLJ_VM();
                ksljold.ksbm = ksbm;
                ksljold.ljbm = ljbm_old;

                return _Json.GetResults(await _Repository.SaveDeptCp(ksljold,ksljnew));
            }
            else //新增
            {
                return _Json.GetResults(await _Repository.AddEntity(ksljnew));
            }
        }
        /// <summary>
        /// 删除单个科室路径
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> DelDeptCp(JsonObject data)
        {
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能保存！");
            }
            string ksbm = _Json.InString(data["ksbm"]);
            if (string.IsNullOrWhiteSpace(ksbm))
            {
                return _Json.GetErrMsg("科室编码为空，不能保存！");
            }

            CP_KSLJ_VM kslj = await _Repository.GetEntity<CP_KSLJ_VM>(a => a.ksbm == ksbm && a.ljbm == ljbm);
            if(kslj == null)
            {
                return _Json.GetErrMsg($"未查询到科室编码：{ksbm}，路径编码：{ljbm}的对应信息！");
            }
            return _Json.GetResults(await _Repository.DelDeptCp(kslj));
        }
        #endregion

        #region 路径病种
        /// <summary>
        /// 查询路径病种列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetListCpIcd(JsonObject data)
        {
            int page = _Json.InInt(data["page"]);
            int limit = _Json.InInt(data["limit"]);
            string filter = _Json.InString(data["filterstr"]);
            string ljbm = _Json.InString(data["ljbm"]);
            string jbbm = _Json.InString(data["jbbm"]);
            RefAsync<int> total = 0;
            var list = await _Repository.GetListCpIcd(ljbm, jbbm, page, limit, filter, total);
            return _Json.GetResults(page, limit, total, list);
        }
        /// <summary>
        /// 查询单个路径病种信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetSingleCpIcd(JsonObject data)
        {
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能执行！");
            }
            string jbbm = _Json.InString(data["jbbm"]);
            if (string.IsNullOrWhiteSpace(jbbm))
            {
                return _Json.GetErrMsg("疾病编码为空，不能执行！");
            }
            return _Json.GetResults(await _Repository.GetSingleCpIcd(ljbm,jbbm));
        }
        /// <summary>
        /// 保存单个路径病种信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> SaveCpIcd(JsonObject data)
        {
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能保存！");
            }
            string jbbm = _Json.InString(data["jbbm"]);
            if (string.IsNullOrWhiteSpace(jbbm))
            {
                return _Json.GetErrMsg("疾病编码为空，不能保存！");
            }
            string jbbm_old = _Json.InString(data["jbbm_old"]);
            CP_LJICD_VM ljicdnew = new CP_LJICD_VM();
            ljicdnew.ljbm = ljbm;
            ljicdnew.jbbm = jbbm;

            CP_LJICD_VM ljicd = await _Repository.GetEntity<CP_LJICD_VM>(a => a.ljbm == ljbm && a.jbbm == jbbm);
            if (ljicd != null)
            {
                return _Json.GetErrMsg($"路径编码：{ljbm}，疾病编码：{jbbm}对应关系已经存在！");
            }
            if (!string.IsNullOrWhiteSpace(jbbm_old)) //更新
            {                
                CP_LJICD_VM ljicdold = new CP_LJICD_VM();
                ljicdold.jbbm = jbbm_old;
                ljicdold.ljbm = ljbm;

                return _Json.GetResults(await _Repository.SaveCpIcd(ljicdold, ljicdnew));
            }
            else //新增
            {
                return _Json.GetResults(await _Repository.AddEntity(ljicdnew));
            }
        }
        /// <summary>
        /// 删除单个科室路径信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> DelCpIcd(JsonObject data)
        {
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能删除！");
            }
            string jbbm = _Json.InString(data["jbbm"]);
            if (string.IsNullOrWhiteSpace(jbbm))
            {
                return _Json.GetErrMsg("疾病编码为空，不能删除！");
            }

            CP_LJICD_VM ljicd = await _Repository.GetEntity<CP_LJICD_VM>(a => a.jbbm == jbbm && a.ljbm == ljbm);
            if (ljicd == null)
            {
                return _Json.GetErrMsg($"未查询到路径编码：{ljbm}，疾病编码：{jbbm}的对应信息！");
            }
            return _Json.GetResults(await _Repository.DelCpIcd(ljicd));
        }
        #endregion

        #region 执行大类
        /// <summary>
        /// 获取执行大类列表
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetListExecuteLargeClass(JsonObject data)
        {
            int page = _Json.InInt(data["page"]);
            int limit = _Json.InInt(data["limit"]);
            string filter = _Json.InString(data["filterstr"]);
            RefAsync<int> total = 0;
            var list = await _Repository.GetListExecuteLargeClass(page, limit, filter, total);
            return _Json.GetResults(page, limit, total, list);
        }
        /// <summary>
        /// 获取单个执行大类信息
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetSingleExecuteLargeClass(JsonObject data)
        {
            string dlbm = _Json.InString(data["dlbm"]);
            if (string.IsNullOrWhiteSpace(dlbm))
            {
                return _Json.GetErrMsg("执行大类编码为空，不能查询！");
            }
            return _Json.GetResults(await _Repository.GetSingleExecuteLargeClass(dlbm));
        }
        /// <summary>
        /// 保存执行大类
        /// </summary>
        /// <returns></returns>
        public async Task<string> SaveExecuteLargeClass(JsonObject data)
        {
            StringBuilder msg = new StringBuilder();
            CP_ZXDL_VM zxdl = _Json.InEntities<CP_ZXDL_VM>(data);
            if (string.IsNullOrWhiteSpace(zxdl.dlmc))
            {
                return _Json.GetErrMsg("执行大类名称为空，不能保存！");
            }
            if (string.IsNullOrWhiteSpace(zxdl.dlbm)) //新增
            {
                zxdl.dlbm = _Ywxh.GetNewCode(await _Repository.GetMax<CP_ZXDL_VM>(a => a.dlbm),4);
                if (string.IsNullOrWhiteSpace(zxdl.dlbm))
                {
                    return _Json.GetErrMsg("生成大类编码错误！");
                }
            }
            return _Json.GetResults(await _Repository.SaveExecuteLargeClass(zxdl, _Json.ToJsonKeyArr(data)));
        }
        /// <summary>
        /// 删除执行大类
        /// </summary>
        /// <returns></returns>
        public async Task<string> DelExecuteLargeClass(JsonObject data)
        {
            string dlbm = _Json.InString(data["dlbm"]);
            if (string.IsNullOrWhiteSpace(dlbm))
            {
                return _Json.GetErrMsg("大类编码为空，不能删除！");
            }

            CP_ZXDL_VM zxdl = await _Repository.GetEntity<CP_ZXDL_VM>(a => a.dlbm == dlbm);
            if (zxdl == null)
            {
                return _Json.GetErrMsg($"未查询到执行大类编码：{dlbm}的信息！");
            }
            return _Json.GetResults(await _Repository.DelExecuteLargeClass(zxdl));
        }
        #endregion

        #region 执行细类
        /// <summary>
        /// 获取执行细类列表
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetListExecuteSubclass(JsonObject data)
        {
            int page = _Json.InInt(data["page"]);
            int limit = _Json.InInt(data["limit"]);
            string filter = _Json.InString(data["filterstr"]);
            RefAsync<int> total = 0;
            var list = await _Repository.GetListExecuteSubclass(page, limit, filter, total);
            return _Json.GetResults(page, limit, total, list);
        }
        /// <summary>
        /// 获取单个执行细类信息
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetSingleExecuteSubclass(JsonObject data)
        {
            string xlbm = _Json.InString(data["xlbm"]);
            if (string.IsNullOrWhiteSpace(xlbm))
            {
                return _Json.GetErrMsg("执行细类编码为空，不能查询！");
            }
            return _Json.GetResults(await _Repository.GetSingleExecuteSubclass(xlbm));
        }
        /// <summary>
        /// 保存执行细类
        /// </summary>
        /// <returns></returns>
        public async Task<string> SaveExecuteSubclass(JsonObject data)
        {
            StringBuilder msg = new StringBuilder();
            CP_ZXXL_VM zxxl = _Json.InEntities<CP_ZXXL_VM>(data);
            if (string.IsNullOrWhiteSpace(zxxl.xlmc))
            {
                return _Json.GetErrMsg("执行细类名称为空，不能保存！");
            }
            if (string.IsNullOrWhiteSpace(zxxl.xlbm)) //新增
            {
                zxxl.xlbm = _Ywxh.GetNewCode(await _Repository.GetMax<CP_ZXXL_VM>(a => a.xlbm), 4);
                if (string.IsNullOrWhiteSpace(zxxl.xlbm))
                {
                    return _Json.GetErrMsg("生成细类编码错误！");
                }
            }
            return _Json.GetResults(await _Repository.SaveExecuteSubclass(zxxl, _Json.ToJsonKeyArr(data)));
        }
        /// <summary>
        /// 删除执行细类
        /// </summary>
        /// <returns></returns>
        public async Task<string> DelExecuteSubclass(JsonObject data)
        {
            string xlbm = _Json.InString(data["xlbm"]);
            if (string.IsNullOrWhiteSpace(xlbm))
            {
                return _Json.GetErrMsg("细类编码为空，不能删除！");
            }

            CP_ZXXL_VM zxxl = await _Repository.GetEntity<CP_ZXXL_VM>(a => a.xlbm == xlbm);
            if (zxxl == null)
            {
                return _Json.GetErrMsg($"未查询到执行细类编码：{xlbm}的信息！");
            }
            return _Json.GetResults(await _Repository.DelExecuteSubclass(zxxl));
        }
        #endregion
    }
}
