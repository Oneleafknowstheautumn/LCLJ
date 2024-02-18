using His.ClinicalPathway.Repository;
using His.Core;
using His.DAL;
using His.Entities;
using SQLitePCL;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace His.ClinicalPathway.Services
{
    public class CPFormMaintenanceServices : ICPFormMaintenanceServices
    {
        private readonly ICPFormMaintenanceRepository _Repository;
        private readonly IJsonMethod _Json;
        private readonly IPublicYwxhRepository _Ywxh;
        private readonly ICPPublicRepository _Public;
        private readonly IPublicDddwRepository _Dddw;
        private readonly ICommon _Com;
        public CPFormMaintenanceServices(ICPFormMaintenanceRepository IRepository, IJsonMethod IJson, IPublicYwxhRepository IYwxh, ICPPublicRepository @public, IPublicDddwRepository dddw, ICommon Com)
        {
            _Repository = IRepository;
            _Json = IJson;
            _Ywxh = IYwxh;
            _Public = @public;
            _Dddw = dddw;
            _Com = Com;
        }
        #region 路径阶段
        /// <summary>
        /// 查询路径阶段列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetListCpPhase(JsonObject data)
        {
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能查询！");
            }
            int page = _Json.InInt(data["page"]);
            int limit = _Json.InInt(data["limit"]);
            string filter = _Json.InString(data["filterstr"]);
            RefAsync<int> total = 0;
            var list = await _Repository.GetListCpPhase(ljbm, page, limit, filter, total);
            return _Json.GetResults(page, limit, total, list);
        }
        /// <summary>
        /// 查询单个路径阶段信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetSingleCpPhase(JsonObject data)
        {
            string jdbm = _Json.InString(data["jdbm"]);
            if (string.IsNullOrWhiteSpace(jdbm))
            {
                return _Json.GetErrMsg("路径阶段编码为空，不能查询！");
            }
            return _Json.GetResults(await _Repository.GetSingleCpPhase(jdbm));
        }
        /// <summary>
        /// 保存路径阶段信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> SaveCpPhase(JsonObject data)
        {
            StringBuilder msg = new StringBuilder();
            CP_LJJD_VM ljjd = _Json.InEntities<CP_LJJD_VM>(data);
            if (ljjd == null)
            {
                return _Json.GetErrMsg("未获取到要保存的数据！" + msg.ToString());
            }
            if (string.IsNullOrWhiteSpace(ljjd.jdbm))
            {
                return _Json.GetErrMsg("请输入阶段编码！");
            }

            CP_LJJD_VM ljmcold = await _Repository.GetEntity<CP_LJJD_VM>(a => a.ljbm == ljjd.ljbm);

            if (ljmcold == null) //新增
            {
                if (string.IsNullOrWhiteSpace(ljjd.ljbm))
                {
                    return _Json.GetErrMsg("请选择路径名称！");
                }
                if (ljjd.jdhf <= 0)
                {
                    return _Json.GetErrMsg("请录入阶段顺序！");
                }
                if (string.IsNullOrWhiteSpace(ljjd.jdmc))
                {
                    return _Json.GetErrMsg("请录入阶段名称！");
                }
                if (ljjd.jd_begin == null)
                {
                    return _Json.GetErrMsg("请录入开始天数！");
                }
                if (ljjd.jd_begin <= 0)
                {
                    return _Json.GetErrMsg("开始天数必须大于零！");
                }
                if (ljjd.jd_end == null)
                {
                    return _Json.GetErrMsg("请录入结束天数！");
                }
                if (ljjd.jd_end <= 0)
                {
                    return _Json.GetErrMsg("结束天数必须大于零！");
                }
                if(ljjd.jd_begin > ljjd.jd_end)
                {
                    return _Json.GetErrMsg($"开始天数：{ljjd.jd_begin}不能大于结束天数：{ljjd.jd_end}");
                }
                if (string.IsNullOrWhiteSpace(ljjd.jdbz))
                {
                    return _Json.GetErrMsg("请指定阶段标准类别！");
                }
            }
            else //修改
            {
                if (data["ljbm"] != null && string.IsNullOrWhiteSpace(ljjd.ljbm))
                {
                    return _Json.GetErrMsg("请选择路径名称！");
                }
                if (data["jdhf"] != null && ljjd.jdhf <= 0)
                {
                    return _Json.GetErrMsg("请录入阶段顺序！");
                }
                if (data["jdmc"] != null && string.IsNullOrWhiteSpace(ljjd.jdmc))
                {
                    return _Json.GetErrMsg("请录入阶段名称！");
                }
                if (data["jd_begin"] != null && ljjd.jd_begin == null)
                {
                    return _Json.GetErrMsg("请录入开始天数！");
                    if (ljjd.jd_begin <= 0)
                    {
                        return _Json.GetErrMsg("开始天数必须大于零！");
                    }
                }
                if (data["jd_end"] != null && ljjd.jd_end == null)
                {
                    return _Json.GetErrMsg("请录入结束天数！");
                    if (ljjd.jd_end <= 0)
                    {
                        return _Json.GetErrMsg("结束天数必须大于零！");
                    }
                }
                if (data["jd_begin"] != null && data["jd_end"] != null && ljjd.jd_begin > ljjd.jd_end)
                {
                    return _Json.GetErrMsg($"开始天数：{ljjd.jd_begin}不能大于结束天数：{ljjd.jd_end}");
                }
                if (data["jdbz"] != null && string.IsNullOrWhiteSpace(ljjd.jdbz))
                {
                    return _Json.GetErrMsg("请指定阶段标准类别！");
                }
            }

            DbResult<bool> result = await _Repository.SaveCpPhase(ljjd, _Json.ToJsonKeyArr(data));
            return _Json.GetResults(result);
        }
        /// <summary>
        /// 删除路径阶段
        /// </summary>
        /// <returns></returns>
        public async Task<string> DelCpPhase(JsonObject data)
        {
            string jdbm = _Json.InString(data["jdbm"]);
            if (string.IsNullOrWhiteSpace(jdbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能删除！");
            }
            CP_LJJD_VM ljjd = await _Repository.GetEntity<CP_LJJD_VM>(a => a.jdbm == jdbm);
            if (ljjd == null)
            {
                return _Json.GetErrMsg($"未查询到编码：{jdbm}的路径名称信息！");
            }

            return _Json.GetResults(await _Repository.DelCpPhase(ljjd));
        }
        #endregion

        #region 路径项目
        /// <summary>
        /// 查询路径项目列表
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetListCpPhaseProject(JsonObject data)
        {
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能查询！");
            }
            string jdbm = _Json.InString(data["jdbm"]);
            if (string.IsNullOrWhiteSpace(jdbm))
            {
                return _Json.GetErrMsg("阶段编码为空，不能查询！");
            }
            int page = _Json.InInt(data["page"]);
            int limit = _Json.InInt(data["limit"]);
            string filter = _Json.InString(data["filterstr"]);
            RefAsync<int> total = 0;
            var list = await _Public.GetListCpProject(ljbm, jdbm, page, limit, filter, total);
            return _Json.GetResults(page, limit, total, list);
        }
        /// <summary>
        /// 查询单个路径项目
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetSingleCpProject(JsonObject data)
        {
            string xmbm = _Json.InString(data["xmbm"]);
            if (string.IsNullOrWhiteSpace(xmbm))
            {
                return _Json.GetErrMsg("路径项目编码为空，不能查询！");
            }
            return _Json.GetResults(await _Repository.GetSingleCpProject(xmbm));
        }

        /// <summary>
        /// 保存路径项目
        /// </summary>
        /// <returns></returns>
        public async Task<string> SaveCpProject(JsonObject data)
        {
            StringBuilder msg = new StringBuilder();
            CP_LJXM_VM ljxm = _Json.InEntities<CP_LJXM_VM>(data,msg);
            if (ljxm == null)
            {
                return _Json.GetErrMsg("未获取到要保存的数据！" + msg.ToString());
            }

            CP_LJXM_VM ljxmold = await _Repository.GetEntity<CP_LJXM_VM>(a => a.ljbm == ljxm.xmbm);
            ljxm.cjczy = _Com.GetUserIdByToken();
            if (ljxmold == null) //新增
            {
                if (string.IsNullOrWhiteSpace(ljxm.ljbm))
                {
                    return _Json.GetErrMsg("请选择路径名称！");
                }
                if (string.IsNullOrWhiteSpace(ljxm.jdbm))
                {
                    return _Json.GetErrMsg("请选择路径阶段！");
                }
                if (string.IsNullOrWhiteSpace(ljxm.xmdl))
                {
                    return _Json.GetErrMsg("请指定项目大类！");
                }
                if (string.IsNullOrWhiteSpace(ljxm.xmlx))
                {
                    return _Json.GetErrMsg("请指定项目类型！");
                }
                if (string.IsNullOrWhiteSpace(ljxm.xmnr))
                {
                    return _Json.GetErrMsg("请输入项目内容！");
                }
                if (string.IsNullOrWhiteSpace(ljxm.xmxz))
                {
                    return _Json.GetErrMsg("请指定选择方式！");
                }
                if (string.IsNullOrWhiteSpace(ljxm.xmzxfs))
                {
                    return _Json.GetErrMsg("请指定执行方式！");
                }
                if (string.IsNullOrWhiteSpace(ljxm.zxdlbh))
                {
                    return _Json.GetErrMsg("请指定执行大类！");
                }
                if (string.IsNullOrWhiteSpace(ljxm.zxxlbh))
                {
                    return _Json.GetErrMsg("请指定执行细类！");
                }
                if (string.IsNullOrWhiteSpace(ljxm.xmzxr))
                {
                    return _Json.GetErrMsg("请指定执行人类别！");
                }
                if (string.IsNullOrWhiteSpace(ljxm.xmjdpg))
                {
                    return _Json.GetErrMsg("请输入项目评估内容！");
                }
                ljxm.cjrq = _Repository.GetSysDate();
            }
            else //修改
            {
                if (data["ljbm"] != null && string.IsNullOrWhiteSpace(ljxm.ljbm))
                {
                    return _Json.GetErrMsg("请选择路径名称！");
                }
                if (data["jdbm"] != null && string.IsNullOrWhiteSpace(ljxm.jdbm))
                {
                    return _Json.GetErrMsg("请选择路径阶段！");
                }
                if (data["xmdl"] != null && string.IsNullOrWhiteSpace(ljxm.xmdl))
                {
                    return _Json.GetErrMsg("请指定项目大类！");
                }
                if (data["xmlx"] != null && string.IsNullOrWhiteSpace(ljxm.xmlx))
                {
                    return _Json.GetErrMsg("请指定项目类型！");
                }
                if (data["xmnr"] != null && string.IsNullOrWhiteSpace(ljxm.xmnr))
                {
                    return _Json.GetErrMsg("请输入项目内容！");
                }
                if (data["xmxz"] != null && string.IsNullOrWhiteSpace(ljxm.xmxz))
                {
                    return _Json.GetErrMsg("请指定选择方式！");
                }
                if (data["xmzxfs"] != null && string.IsNullOrWhiteSpace(ljxm.xmzxfs))
                {
                    return _Json.GetErrMsg("请指定执行方式！");
                }
                if (data["zxdlbh"] != null && string.IsNullOrWhiteSpace(ljxm.zxdlbh))
                {
                    return _Json.GetErrMsg("请指定执行大类！");
                }
                if (data["zxxlbh"] != null && string.IsNullOrWhiteSpace(ljxm.zxxlbh))
                {
                    return _Json.GetErrMsg("请指定执行细类！");
                }
                if (data["xmzxr"] != null && string.IsNullOrWhiteSpace(ljxm.xmzxr))
                {
                    return _Json.GetErrMsg("请指定执行人类别！");
                }
                if (data["xmjdpg"] != null && string.IsNullOrWhiteSpace(ljxm.xmjdpg))
                {
                    return _Json.GetErrMsg("请输入项目评估内容！");
                }
            }

            DbResult<bool> result = await _Repository.SaveCpProject(ljxm, _Json.ToJsonKeyArr(data));
            return _Json.GetResults(result);
        }
        /// <summary>
        /// 删除路径项目
        /// </summary>
        /// <returns></returns>
        public async Task<string> DelCpProject(JsonObject data)
        {
            string xmbm = _Json.InString(data["xmbm"]);
            if (string.IsNullOrWhiteSpace(xmbm))
            {
                return _Json.GetErrMsg("路径项目编码为空，不能查询！");
            }
            CP_LJXM_VM ljxm = await _Repository.GetEntity<CP_LJXM_VM>(a => a.xmbm == xmbm);
            if (ljxm == null)
            {
                return _Json.GetErrMsg($"未查询到编码：{xmbm}的路径项目信息！");
            }

            return _Json.GetResults(await _Repository.DelCpProject(ljxm));
        }
        #endregion

        #region 治疗方案
        /// <summary>
        /// 查询治疗方案列表
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetListCPTherapeuticSchedule(JsonObject data)
        {
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能查询！");
            }
            string jdbm = _Json.InString(data["jdbm"]);
            if (string.IsNullOrWhiteSpace(jdbm))
            {
                return _Json.GetErrMsg("阶段编码为空，不能查询！");
            }
            int page = _Json.InInt(data["page"]);
            int limit = _Json.InInt(data["limit"]);
            string filter = _Json.InString(data["filterstr"]);
            RefAsync<int> total = 0;
            var list = await _Repository.GetListCPTherapeuticSchedule(ljbm, jdbm, page, limit, filter, total);
            return _Json.GetResults(page, limit, total, list);
        }
        /// <summary>
        /// 查询单个治疗方案信息
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetSingleCPTherapeuticSchedule(JsonObject data)
        {
            string fabm = _Json.InString(data["fabm"]);
            if (string.IsNullOrWhiteSpace(fabm))
            {
                return _Json.GetErrMsg("方案编码为空，不能查询！");
            }
            return _Json.GetResults(await _Repository.GetSingleCPTherapeuticSchedule(fabm));
        }
        /// <summary>
        /// 保存治疗方案
        /// </summary>
        /// <returns></returns>
        public async Task<string> SaveCPTherapeuticSchedule(JsonObject data)
        {
            StringBuilder msg = new StringBuilder();
            CP_ZLFA_VM zlfa = _Json.InEntities<CP_ZLFA_VM>(data,msg);
            if (zlfa == null)
            {
                return _Json.GetErrMsg("未获取到要保存的数据！" + msg.ToString());
            }

            CP_ZLFA_VM zlfaold = await _Repository.GetEntity<CP_ZLFA_VM>(a => a.ljbm == zlfa.fabm);

            if (zlfaold == null) //新增
            {
                if (string.IsNullOrWhiteSpace(zlfa.ljbm))
                {
                    return _Json.GetErrMsg("请选择路径名称！");
                }
                if (string.IsNullOrWhiteSpace(zlfa.jdbm))
                {
                    return _Json.GetErrMsg("请选择路径阶段！");
                }
                if (string.IsNullOrWhiteSpace(zlfa.famc))
                {
                    return _Json.GetErrMsg("请输入方案名称！");
                }
            }
            else //修改
            {
                if (data["ljbm"] != null && string.IsNullOrWhiteSpace(zlfa.ljbm))
                {
                    return _Json.GetErrMsg("请选择路径名称！");
                }
                if (data["jdbm"] != null && string.IsNullOrWhiteSpace(zlfa.jdbm))
                {
                    return _Json.GetErrMsg("请选择路径阶段！");
                }
                if (data["famc"] != null && string.IsNullOrWhiteSpace(zlfa.famc))
                {
                    return _Json.GetErrMsg("请输入方案名称！");
                }
            }

            DbResult<bool> result = await _Repository.SaveCPTherapeuticSchedule(zlfa, _Json.ToJsonKeyArr(data));
            return _Json.GetResults(result);
        }
        /// <summary>
        /// 删除治疗方案
        /// </summary>
        /// <returns></returns>
        public async Task<string> DelCPTherapeuticSchedule(JsonObject data)
        {
            string fabm = _Json.InString(data["fabm"]);
            if (string.IsNullOrWhiteSpace(fabm))
            {
                return _Json.GetErrMsg("方案编码为空，不能查询！");
            }
            CP_ZLFA_VM zlfa = await _Repository.GetEntity<CP_ZLFA_VM>(a => a.fabm == fabm);
            if (zlfa == null)
            {
                return _Json.GetErrMsg($"未查询到编码：{fabm}的治疗方案信息！");
            }

            return _Json.GetResults(await _Repository.DelCPTherapeuticSchedule(zlfa));
        }
        #endregion

        #region 路径医嘱
        /// <summary>
        /// 查询路径项目医嘱列表
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetListCpProjectMedicalAdvice(JsonObject data)
        {
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能查询！");
            }
            string jdbm = _Json.InString(data["jdbm"]);
            if (string.IsNullOrWhiteSpace(jdbm))
            {
                return _Json.GetErrMsg("阶段编码为空，不能查询！");
            }
            string xmbm = _Json.InString(data["xmbm"]);
            if (string.IsNullOrWhiteSpace(xmbm))
            {
                return _Json.GetErrMsg("路径项目编码为空，不能查询！");
            }
            int page = _Json.InInt(data["page"]);
            int limit = _Json.InInt(data["limit"]);
            string filter = _Json.InString(data["filterstr"]);
            RefAsync<int> total = 0;
            var list = await _Public.GetListCpMedicalAdvice(ljbm, jdbm, xmbm, page, limit, filter, total);
            return _Json.GetResults(page, limit, total, list);
        }
        /// <summary>
        /// 查询中药医嘱明细列表
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetListCpChineseMedicalAdvice(JsonObject data)
        {
            string yzbm = _Json.InString(data["yzbm"]);
            if (string.IsNullOrWhiteSpace(yzbm))
            {
                return _Json.GetErrMsg("医嘱编码为空，不能查询！");
            }
            
            return _Json.GetResults(await _Public.GetListCpChineseMedicalAdvice(yzbm));
        }
        /// <summary>
        /// 查询单个医嘱
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetSingleCpMedicalAdvice(JsonObject data)
        {
            string yzbm = _Json.InString(data["yzbm"]);
            if (string.IsNullOrWhiteSpace(yzbm))
            {
                return _Json.GetErrMsg("方案编码为空，不能查询！");
            }
            return _Json.GetResults(await _Repository.GetSingleCpMedicalAdvice(yzbm));
        }
        /// <summary>
        /// 保存医嘱项目 单条
        /// </summary>
        /// <returns></returns>
        public async Task<string> SaveCpMedicalAdvice(JsonObject data)
        {
            StringBuilder msg = new StringBuilder();
            var yzxm = _Json.InEntities<CP_YZXM_VM>(data,msg);
            if(yzxm == null)
            {
                return _Json.GetErrMsg("获取医嘱信息时出错！" + msg.ToString());
            }
            if(string.IsNullOrWhiteSpace(yzxm.ljbm))
            {
                return _Json.GetErrMsg("路径不能为空！");
            }
            if (string.IsNullOrWhiteSpace(yzxm.jdbm))
            {
                return _Json.GetErrMsg("路径阶段不能为空！");
            }
            if (string.IsNullOrWhiteSpace(yzxm.ljxmbm))
            {
                return _Json.GetErrMsg("路径阶段项目不能为空！");
            }

            string czybm = _Com.GetUserIdByToken();          
            yzxm.cjczy = czybm;
            yzxm.cjrq = _Repository.GetSysDate();
            if (string.IsNullOrWhiteSpace(yzxm.yzbm)) //新增
            {
                string bm = await _Repository.GetMax<CP_YZXM_VM>(a => a.yzbm); //当前最大编码
                bm = _Ywxh.GetNewCode(bm, 10);

                var vmtem = await _Repository.GetEntityList<CP_YZXM_VM>(it => it.yzbm == bm); //查询新增编码是否已存在
                if (vmtem != null)
                {
                    return _Json.GetErrMsg($"新生成的医嘱编码：{bm}已经被使用！");
                }
                yzxm.yzbm = bm;
            }
            if (string.IsNullOrWhiteSpace(yzxm.zlfa))
            {
                return _Json.GetErrMsg("请选择方案名称！");
            }
            yzxm.zlfamc = await _Repository.GetSingle<CP_ZLFA_VM>(a => a.famc, a => a.fabm == yzxm.zlfa);
            if (string.IsNullOrWhiteSpace(yzxm.zlfamc))
            {
                return _Json.GetErrMsg($"治疗方案：{yzxm.zlfa}无效！");
            }
            if (string.IsNullOrWhiteSpace(yzxm.yzlx))
            {
                return _Json.GetErrMsg("请指定医嘱类型！");
            }
            if (string.IsNullOrWhiteSpace(yzxm.yzzl))
            {
                return _Json.GetErrMsg("请指定医嘱种类！");
            }
            if (string.IsNullOrWhiteSpace(yzxm.yzpd))
            {
                return _Json.GetErrMsg("请指定医嘱类别！");
            }
            if (string.IsNullOrWhiteSpace(yzxm.yzmxxmbm))
            {
                return _Json.GetErrMsg("请录入医嘱项目名称！");
            }
            if (yzxm.yzpd == "0") //诊疗项目判断
            {
                //项目有效性
                if (!await _Repository.GetIsExist<PU_MXZLXM_VM>(a => a.mxzlxmbm == yzxm.yzmxxmbm))
                {
                    return _Json.GetErrMsg($"诊疗医嘱项目：{yzxm.yzmxxmbm}无效！");
                }
            }
            else if (yzxm.yzpd == "1") //药品判断
            {
                //项目有效性
                var ypzd = await _Repository.GetEntity<YK_YPZD_VM>(a => a.ypbm == yzxm.yzmxxmbm);
                if (ypzd == null)
                {
                    return _Json.GetErrMsg($"药品医嘱项目：{yzxm.yzmxxmbm}无效！");
                }

                if (yzxm.dcjl <= 0)
                {
                    return _Json.GetErrMsg("请录入单次剂量！");
                }
                if (string.IsNullOrWhiteSpace(yzxm.jldw))
                {
                    return _Json.GetErrMsg("剂量单位不能为空！");
                }
                if (!await _Repository.GetIsExist<YK_JLDWBM_VM>(a => a.jldwid == yzxm.yzmxxmbm))
                {
                    return _Json.GetErrMsg($"剂量单位编码：{yzxm.jldw}无效！");
                }

                if (string.IsNullOrWhiteSpace(yzxm.pcbm))
                {
                    return _Json.GetErrMsg("频次不能为空！");
                }
                if (!await _Repository.GetIsExist<PU_PC_VM>(a => a.pcbm == yzxm.yzmxxmbm))
                {
                    return _Json.GetErrMsg($"频次编码：{yzxm.pcbm}无效！");
                }

                if (string.IsNullOrWhiteSpace(yzxm.yyff))
                {
                    return _Json.GetErrMsg("用药方法不能为空！");
                }
                if (!await _Repository.GetIsExist<PU_GYTJ_VM>(a => a.tjbm == yzxm.yzmxxmbm))
                {
                    return _Json.GetErrMsg($"用药方法编码：{yzxm.yyff}无效！");
                }

                //皮试
                if(yzxm.psypbz == "1" && ypzd.psypbz != "1")
                {
                    return _Json.GetErrMsg("医嘱设定为皮试医嘱，但当前药品非皮试药品！");
                }
            }            

            return _Json.GetResults(await _Repository.SaveCpMedicalAdvice(yzxm, _Json.ToJsonKeyArr(data)));
        }
        /// <summary>
        /// 保存医嘱项目 批量
        /// </summary>
        /// <returns></returns>
        public async Task<string> SaveSomeCpMedicalAdvice(JsonObject data)
        {
            StringBuilder msg = new StringBuilder();
            var listyzxm = _Json.InList<CP_YZXM_VM>(data, msg);
            if (listyzxm == null || listyzxm.Count <= 0)
            {
                return _Json.GetErrMsg("获取医嘱信息时出错！" + msg.ToString());
            }
            var listljbm = listyzxm.Where(it => !string.IsNullOrWhiteSpace(it.ljbm)).Select(it => it.ljbm).ToList();
            if (listljbm.Count <= 0)
            {
                return _Json.GetErrMsg("医嘱明细中未找到[libm]值");
            }
            var listjdbm = listyzxm.Where(it => !string.IsNullOrWhiteSpace(it.ljbm)).Select(it => it.jdbm).ToList();
            if (listljbm.Count <= 0)
            {
                return _Json.GetErrMsg("医嘱明细中未找到[jdbm]值");
            }
            var listljxmbm = listyzxm.Where(it => !string.IsNullOrWhiteSpace(it.ljbm)).Select(it => it.ljxmbm).ToList();
            if (listljbm.Count <= 0)
            {
                return _Json.GetErrMsg("医嘱明细中未找到[ljxmbm]值");
            }
            var listyzxmbm = listyzxm.Select(it => it.yzmxxmbm).ToList(); //医嘱中的项目列表
            var listzlxm = await _Repository.GetEntityList<PU_MXZLXM_VM>(it => listyzxmbm.Contains(it.mxzlxmbm)); //明细诊疗项目
            var listypbm = await _Repository.GetEntityList<YK_YPZD_VM>(it => listyzxmbm.Contains(it.ypbm)); //药品字典
            var listjldw = await _Repository.GetEntityList<YK_JLDWBM_VM>(); //单位
            var listpc = await _Repository.GetEntityList<PU_PC_VM>(); //频次
            var listyyff = await _Repository.GetEntityList<PU_GYTJ_VM>(); //用药方法

            string ljbm = listljbm[0]; //路径编码
            string jdbm = listjdbm[0]; //阶段编码
            string ljxmbm = listljxmbm[0]; //项目编码
            string czybm = _Com.GetUserIdByToken();
            DateTime xtrq = _Repository.GetSysDate();
            string bm = await _Repository.GetMax<CP_YZXM_VM>(a => a.yzbm); //当前最大编码
            List<string> listbm = new List<string>();
            if (string.IsNullOrWhiteSpace(bm))
            {
                bm = "0";
            }
            int index = -1;
            for (int i = 0; i < listyzxm.Count; i++)
            {
                var item = listyzxm[i];
                item.ljbm = ljbm;
                item.jdbm = jdbm;
                item.ljxmbm = ljxmbm;
                item.cjczy = czybm;
                item.cjrq = xtrq;
                if (string.IsNullOrWhiteSpace(item.yzbm)) //新增
                {
                    bm = _Ywxh.GetNewCode(bm, 10);
                    item.yzbm = bm;
                    listbm.Add(bm);
                }
                if (string.IsNullOrWhiteSpace(item.zlfamc))
                {
                    return _Json.GetErrMsg("请选择方案名称！");
                }
                if (string.IsNullOrWhiteSpace(item.yzlx))
                {
                    return _Json.GetErrMsg("请指定医嘱类型！");
                }
                if (string.IsNullOrWhiteSpace(item.yzzl))
                {
                    return _Json.GetErrMsg("请指定医嘱种类！");
                }
                if (string.IsNullOrWhiteSpace(item.yzpd))
                {
                    return _Json.GetErrMsg("请指定医嘱类别！");
                }
                if (string.IsNullOrWhiteSpace(item.yzmxxmbm))
                {
                    return _Json.GetErrMsg("请录入医嘱项目名称！");
                }
                if (item.yzpd == "0") //诊疗项目判断
                {
                    //项目有效性
                    index = listzlxm.FindIndex(it => it.mxzlxmbm == item.yzmxxmbm);
                    if (index < 0)
                    {
                        return _Json.GetErrMsg($"诊疗医嘱项目：{item.yzmxxmbm}无效！");
                    }
                }
                else if (item.yzpd == "1") //药品判断
                {
                    //项目有效性
                    index = listypbm.FindIndex(it => it.ypbm == item.yzmxxmbm);
                    if (index < 0)
                    {
                        return _Json.GetErrMsg($"药品医嘱项目：{item.yzmxxmbm}无效！");
                    }
                    if(item.psypbz == "1" && listypbm[index].psypbz != "1")
                    {
                        return _Json.GetErrMsg($"药品设定为皮试医嘱，但{listypbm[index].ypmc}非皮试药品！");
                    }

                    if (item.dcjl <= 0)
                    {
                        return _Json.GetErrMsg("请录入单次剂量！");
                    }
                    if (string.IsNullOrWhiteSpace(item.jldw))
                    {
                        return _Json.GetErrMsg("剂量单位不能为空！");
                    }
                    index = listjldw.FindIndex(it => it.jldwid == item.jldw);
                    if (index < 0)
                    {
                        return _Json.GetErrMsg($"剂量单位编码：{item.jldw}无效！");
                    }

                    if (string.IsNullOrWhiteSpace(item.pcbm))
                    {
                        return _Json.GetErrMsg("频次不能为空！");
                    }
                    index = listpc.FindIndex(it => it.pcbm == item.pcbm);
                    if (index < 0)
                    {
                        return _Json.GetErrMsg($"频次编码：{item.pcbm}无效！");
                    }

                    if (string.IsNullOrWhiteSpace(item.yyff))
                    {
                        return _Json.GetErrMsg("用药方法不能为空！");
                    }
                    index = listyyff.FindIndex(it => it.tjbm == item.yyff);
                    if (index < 0)
                    {
                        return _Json.GetErrMsg($"用药方法编码：{item.yyff}无效！");
                    }
                }
            }
            var listtem = await _Repository.GetEntityList<CP_YZXM_VM>(yzxm => listbm.Contains(yzxm.yzbm)); //查询新增编码是否已存在
            if (listtem.Count > 0)
            {
                return _Json.GetErrMsg($"新生成的医嘱编码：{_Com.ListToStr(listbm, ',')}已经被使用！");
            }

            return _Json.GetResults(await _Repository.SaveCpMedicalAdvice(listyzxm, _Json.ToJsonKeyArr(data[0])));
        }
        /// <summary>
        /// 保存中药医嘱
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> SaveCpChineseMedicalAdvice(JsonObject data)
        {
            StringBuilder msg = new StringBuilder();
            CP_YZXM_VM yzxm = _Json.InEntities<CP_YZXM_VM>(data["yzxm"], msg);
            if (yzxm == null)
            {
                return _Json.GetErrMsg("获取医嘱信息时出错！" + msg.ToString());
            }

            if (string.IsNullOrWhiteSpace(yzxm.ljbm))
            {
                return _Json.GetErrMsg("请选择路径！");
            }
            if (string.IsNullOrWhiteSpace(yzxm.jdbm))
            {
                return _Json.GetErrMsg("请选择路径阶段！");
            }
            if (string.IsNullOrWhiteSpace(yzxm.ljxmbm))
            {
                return _Json.GetErrMsg("请选择项目名称！");
            }
            if (string.IsNullOrWhiteSpace(yzxm.zlfa))
            {
                return _Json.GetErrMsg("请选择方案名称！");
            }
            yzxm.zlfamc = await _Repository.GetSingle<CP_ZLFA_VM>(a => a.famc, a => a.fabm == yzxm.zlfa);
            if (string.IsNullOrWhiteSpace(yzxm.zlfamc))
            {
                return _Json.GetErrMsg($"治疗方案：{yzxm.zlfa}无效！");
            }
            if (string.IsNullOrWhiteSpace(yzxm.yzlx))
            {
                yzxm.yzlx = "0";
            }
            //if (string.IsNullOrWhiteSpace(yzxm.yzzl))
            //{
            //    return _Json.GetErrMsg("请指定医嘱种类！");
            //}

            if (yzxm.fjsl <= 0)
            {
                return _Json.GetErrMsg("请输入剂数！");
            }
            if (string.IsNullOrWhiteSpace(yzxm.fjmc))
            {
                return _Json.GetErrMsg("请输入方剂名称！");
            }
            yzxm.yzpd = "1"; //医嘱类别 0=医疗医嘱 1=药品医嘱 2=嘱托医嘱
            yzxm.sfcy = "1";

            msg.Clear();
            List<CP_YZXM_ZYMX_VM> listzymx = _Json.InList<CP_YZXM_ZYMX_VM>(data["zymx"], msg);
            if (listzymx == null || listzymx.Count <= 0)
            {
                return _Json.GetErrMsg("获取中药明细错误！" + msg.ToString());
            }

            //药典信息
            List<string> listypbm = listzymx.Select(it => it.ypbm).ToList(); //药品编码列表
            List<YK_YPZD_VM> listypzd = await _Repository.GetEntityList<YK_YPZD_VM>(a => listypbm.Contains(a.ypbm));

            //医嘱编码
            if (string.IsNullOrWhiteSpace(yzxm.yzbm))
            {
                string bm = await _Repository.GetMax<CP_YZXM_VM>(a => a.yzbm);
                if (string.IsNullOrWhiteSpace(bm))
                {
                    bm = "0000000001";
                }
                else
                {
                    bm = _Ywxh.GetNewCode(bm, 10);
                }
                
                if (await _Repository.GetIsExist<CP_YZXM_VM>(a => a.yzbm == bm))
                {
                    return _Json.GetErrMsg($"生成的医嘱编码：{bm}已经被使用！");
                }
                yzxm.yzbm = bm;
            }
            int maxxh = listzymx.Max(it => it.mxxh);
            for (int i = 0; i < listzymx.Count; i++)
            {
                var zymx = listzymx[i];
                if (string.IsNullOrWhiteSpace(zymx.ypbm))
                {
                    return _Json.GetErrMsg($"第{i + 1}味药物编码为空，不能保存！");
                }
                var ypzd = listypzd.Find(it => it.ypbm == zymx.ypbm);
                if (ypzd == null)
                {
                    return _Json.GetErrMsg($"第{i + 1}味药物编码无效，不能保存！");
                }
                if (zymx.dcsl <= 0)
                {
                    return _Json.GetErrMsg($"第{i + 1}味药物：{ypzd.ypmc}单次数量不正确，不能保存！");
                }
                zymx.jldw = ypzd.jldw;
                zymx.yzbm = yzxm.yzbm;
                maxxh++;
                zymx.mxxh = zymx.mxxh <= 0 ? maxxh : zymx.mxxh;
            }

            DbResult<bool> result = await _Repository.SaveCpChineseMedicalAdvice(yzxm, _Json.ToJsonKeyArr(data["yzxm"]), listzymx, _Json.ToJsonKeyArr(data["zymx"]));
            return _Json.GetResults(result);
        }
        /// <summary>
        /// 删除医嘱项目
        /// </summary>
        /// <returns></returns>
        public async Task<string> DelCpMedicalAdvice(JsonObject data)
        {
            string yzbm = _Json.InString(data["yzbm"]);
            if (string.IsNullOrWhiteSpace(yzbm))
            {
                return _Json.GetErrMsg("方案编码为空，不能删除！");
            }
            CP_YZXM_VM yzxm = await _Repository.GetEntity<CP_YZXM_VM>(a => a.yzbm == yzbm);
            if (yzxm == null)
            {
                return _Json.GetErrMsg($"未查询到编码：{yzbm}的医嘱信息！");
            }
         
            if (yzxm.sfcy != "1")
            {
                return _Json.GetResults(await _Repository.DelCpMedicalAdvice(yzxm));
            }
            else
            {
                var listzymx = await _Repository.GetEntityList<CP_YZXM_ZYMX_VM>(a => a.yzbm == yzbm);
                return _Json.GetResults(await _Repository.DelCpMedicalAdvice(yzxm, listzymx));
            }
        }
        /// <summary>
        /// 删除中药医嘱项目
        /// </summary>
        /// <returns></returns>
        public async Task<string> DelCpChineseMedicalAdvice(JsonObject data)
        {
            string yzbm = _Json.InString(data["yzbm"]);
            if (string.IsNullOrWhiteSpace(yzbm))
            {
                return _Json.GetErrMsg("方案编码为空，不能删除！");
            }
            int mxxh = _Json.InInt32(data["mxxh"]);
            if (mxxh <= 0)
            {
                return _Json.GetErrMsg("明细序号为空，不能删除！");
            }
            var yzxm = await _Repository.GetEntity<CP_YZXM_ZYMX_VM>(a => a.yzbm == yzbm && a.mxxh == mxxh);
            if (yzxm == null)
            {
                return _Json.GetErrMsg($"未查询到编码：{yzbm}，明细序号：{mxxh}的中药医嘱信息！");
            }

            return _Json.GetResults(await _Repository.DelCpChineseMedicalAdvice(yzxm));
        }

        #endregion

        #region 路径查询
        /// <summary>
        /// 查询科室路径名称列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetListCPNameByDept(JsonObject data)
        {
            string ksbm = _Json.InString(data["ksbm"]);
            if (string.IsNullOrWhiteSpace(ksbm))
            {
                return _Json.GetErrMsg("科室为空，不能查询！");
            }
            int page = _Json.InInt(data["page"]);
            int limit = _Json.InInt(data["limit"]);
            string filter = _Json.InString(data["filterstr"]);
            RefAsync<int> total = 0;
            var list = await _Public.GetListCPNameByDept(ksbm,page,limit,filter,total);
            return _Json.GetResults(page,limit,total.Value,list);  
        }
        /// <summary>
        /// 查询路径下的阶段列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetCpPhase(JsonObject data)
        {
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径为空，不能查询！");
            }
            int page = _Json.InInt(data["page"]);
            int limit = _Json.InInt(data["limit"]);
            string filter = _Json.InString(data["filterstr"]);
            RefAsync<int> total = 0;
            var list = await _Public.GetCpPhase(ljbm, page, limit, filter, total);
            return _Json.GetResults(page, limit, total.Value, list);
        }

        /// <summary>
        /// 查询路径下的项目列表
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetListCpProject(JsonObject data)
        {
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能查询！");
            }
            int page = _Json.InInt(data["page"]);
            int limit = _Json.InInt(data["limit"]);
            string filter = _Json.InString(data["filterstr"]);
            RefAsync<int> total = 0;
            var list = await _Public.GetListCpProject(ljbm, "", page, limit, filter, total);
            return _Json.GetResults(page, limit, total, list);
        }
        /// <summary>
        /// 路径下的医嘱项目
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetListCpMedicalAdvice(JsonObject data)
        {
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能查询！");
            }
            int page = _Json.InInt(data["page"]);
            int limit = _Json.InInt(data["limit"]);
            string filter = _Json.InString(data["filterstr"]);
            RefAsync<int> total = 0;
            var list = await _Public.GetListCpMedicalAdvice(ljbm, "", "", page, limit, filter, total);
            return _Json.GetResults(page, limit, total, list);
        }
        #endregion

        #region 路径表单
        // 查询科室路径名称列表 GetListCPNameByDept


        /// <summary>
        /// 根据路径编码查询路径阶段项目信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetCpMedicalAdvice(JsonObject data)
        {
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能查询！");
            }
            
            CP_LJMC_VM ljmc = await _Repository.GetEntity<CP_LJMC_VM>(a => a.ljbm == ljbm);
            if(ljmc == null)
            {
                return _Json.GetErrMsg($"路径编码：{ljbm}无效！");
            }
            
            var listljjdxm = await _Public.GetListCpProject(ljbm,"");
            return _Json.GetResults(listljjdxm);

            //if (dw_ljmc4.CurrentRow > 0)
            //{
            //    try
            //    {
            //        dw_ljmc4.SelectRow(0, false);
            //        dw_ljmc4.SelectRow(dw_ljmc4.CurrentRow, true);
            //        for (int i = 1; i <= dw_ljmc4.rowcount(); i++)
            //        {
            //            dw_ljmc4.SetItem(i, "xz", "0");
            //        }
            //        dw_ljmc4.SetItem(dw_ljmc4.CurrentRow, "xz", "1");
            //        string ls_ljbm = dw_ljmc4.GetItemStringEx(dw_ljmc4.CurrentRow, "ljbm", "");
            //        string ls_ljmc = dw_ljmc4.GetItemStringEx(dw_ljmc4.CurrentRow, "ljmc", "");
            //        string ls_sydx = dw_ljmc4.GetItemStringEx(dw_ljmc4.CurrentRow, "sydx", "");
            //        string ls_bzzyr = dw_ljmc4.GetItemStringEx(dw_ljmc4.CurrentRow, "bzzyr", "");
            //        dw_bd.Modify("to_bt.Text='" + ls_ljmc + "'");
            //        dw_bd.Modify("to_sydx.Text='" + ls_sydx + "'");
            //        dw_bd.Modify("to_bzzyr.Text='" + ls_bzzyr + "'");

            //        DC_DataStore lds_jdmc = new DC_DataStore();
            //        lds_jdmc.LibraryList = SystemInfo.AppDir + @"\pbl\lclj.pbl";
            //        lds_jdmc.DataWindowObject = "lclj_wh_ljbd_jdmc";
            //        lds_jdmc.Retrieve(ls_ljbm);

            //        DC_DataStore lds_xmmc = new DC_DataStore();
            //        lds_xmmc.LibraryList = SystemInfo.AppDir + @"\pbl\lclj.pbl";
            //        lds_xmmc.DataWindowObject = "lclj_wh_ljbd_xmmc";

            //        dw_bd.Reset();
            //        int groupno = 1, j = 0;
            //        int ll_bddyls = dw_ljmc4.GetItemIntEx(dw_ljmc4.CurrentRow, "bddyls", 0);
            //        if (ll_bddyls <= 0) { ll_bddyls = 3; }
            //        if (ll_bddyls > 10) { ll_bddyls = 10; }
            //        string ls_headername = "", ls_jdbm = "";
            //        int ll_xh1 = 0, ll_xh2 = 0, ll_find = 0, ll_ins;
            //        for (int i = 1; i <= lds_jdmc.RowCount; i++)
            //        {
            //            if (j == ll_bddyls) { groupno++; }
            //            j = i % ll_bddyls;
            //            if (j == 0) { j = ll_bddyls; }
            //            ls_headername = lds_jdmc.GetItemStringEx(i, "jdmc", "").Trim();
            //            ls_jdbm = lds_jdmc.GetItemStringEx(i, "jdbm", "").Trim();
            //            lds_xmmc.Retrieve(ls_ljbm, ls_jdbm, "");
            //            for (int k = 1; k <= lds_xmmc.RowCount; k++)
            //            {
            //                if (ll_xh1 != lds_xmmc.GetItemIntEx(k, "xh", 0))
            //                {
            //                    ll_xh2 = 1;
            //                }
            //                else
            //                {
            //                    ll_xh2++;
            //                }
            //                ll_xh1 = lds_xmmc.GetItemIntEx(k, "xh", 0);
            //                ll_find = dw_bd.FindRow("(groupno=" + groupno.ToString() + ") and (no1=" + ll_xh1.ToString() + ") and (no2=" + ll_xh2.ToString() + ")", 1, dw_bd.rowcount());
            //                if (ll_find <= 0)
            //                {
            //                    ll_ins = dw_bd.InsertRow(0);
            //                    dw_bd.SetItem(ll_ins, "groupno", groupno);
            //                    dw_bd.SetItem(ll_ins, "mc", lds_xmmc.GetItemStringEx(k, "dlmc", ""));
            //                    dw_bd.SetItem(ll_ins, "no1", ll_xh1);
            //                    dw_bd.SetItem(ll_ins, "no2", ll_xh2);
            //                    dw_bd.SetItem(ll_ins, "c" + j.ToString(), lds_xmmc.GetItemStringEx(k, "xmnr", ""));
            //                    dw_bd.SetItem(ll_ins, "xmxz" + j.ToString(), lds_xmmc.GetItemStringEx(k, "xmxz", ""));
            //                    dw_bd.SetItem(ll_ins, "xz" + j.ToString(), lds_xmmc.GetItemStringEx(k, "kxx", ""));
            //                    dw_bd.SetItem(ll_ins, "bt" + j.ToString(), ls_headername);
            //                }
            //                else
            //                {
            //                    dw_bd.SetItem(ll_find, "c" + j.ToString(), lds_xmmc.GetItemStringEx(k, "xmnr", ""));
            //                    dw_bd.SetItem(ll_find, "xmxz" + j.ToString(), lds_xmmc.GetItemStringEx(k, "xmxz", ""));
            //                    dw_bd.SetItem(ll_find, "xz" + j.ToString(), lds_xmmc.GetItemStringEx(k, "kxx", ""));
            //                    dw_bd.SetItem(ll_find, "bt" + j.ToString(), ls_headername);
            //                }
            //            }
            //        }
            //        dw_bd.SetSort("groupno A no1 A no2 A");
            //        dw_bd.Sort();
            //        dw_bd.CalculateGroups();
            //    }
            //    catch (Exception ex)
            //    {
            //        UMessageBox.Show(ex.Message, "提示", BoxButtons.OK, BoxIcons.Stop);
            //    }
            //}
        }
        #endregion

        #region 树型列表
        /// <summary>
        /// 查询路径、阶段、项目三级列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetCpPhaseProjectTree(JsonObject data)
        {
            string ksbm = _Json.InString(data["ksbm"]);
            if (string.IsNullOrWhiteSpace(ksbm))
            {
                return _Json.GetErrMsg("路径为空，不能查询！");
            }
            //var listljmc = await _Public.GetListCPNameByDept(ksbm);
            //var listljjd = await _Public.GetCpPhase(ljbm);
            //var listjdxm = await _Public.GetListCpProject(ljbm);
            List<Dictionary<string, object>> listdic = new List<Dictionary<string, object>>();
            //foreach (var vm in listljjd)
            //{
            //    var listxm = listjdxm.Where(it => it.jdbm == vm.jdbm).ToList();
            //    var dic = new Dictionary<string, object>();
            //    dic.Add("jdbm", vm.jdbm);
            //    dic.Add("jdmc", vm.jdmc);
            //    dic.Add("children", listxm);
            //    listdic.Add(dic);
            //}
            return _Json.GetResults(listdic);
        }
        /// <summary>
        /// 查询项目列表树
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetCpProjectTree(JsonObject data)
        {
            string jdbm = _Json.InString(data["jdbm"]);
            if (string.IsNullOrWhiteSpace(jdbm))
            {
                return _Json.GetErrMsg("阶段为空，不能查询！");
            }
            var listjdxm = await _Public.GetEntityList<CP_LJXM_VM>(a => a.jdbm == jdbm);
            List<DddwEntity> listxmdl = await _Dddw.GetList("CP_xmdl", "");
            List<Dictionary<string, object>> listdic = new List<Dictionary<string, object>>();
            foreach(var vm in listxmdl)
            {
                var dic = new Dictionary<string, object>();
                dic.Add("id", vm.bm);
                dic.Add("mc", vm.mc);

                var list = listjdxm.Where(it => it.xmdl == vm.bm).ToList();
                var listdic_1 = new List<Dictionary<string, object>>();
                foreach (var item in list)
                {
                    var dic_1 = new Dictionary<string, object>();
                    dic_1.Add("id", item.xmbm);
                    dic_1.Add("mc", item.xmnr);
                    dic_1.Add("pid", vm.bm);
                    listdic_1.Add(dic_1);
                }

                dic.Add("children", listdic_1);
                listdic.Add(dic);
            }
            return _Json.GetResults(listdic);
        }
        #endregion
    }
}
