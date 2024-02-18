using His.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His.ClinicalPathway.Repository;
using System.Text.Json.Nodes;
using His.Entities;
using His.DAL;
using System.Diagnostics.CodeAnalysis;
using SqlSugar;

namespace His.ClinicalPathway.Services
{
    /// <summary>
    /// 医生路径管理
    /// </summary>
    public class CPManageServices : ICPManageServices
    {
        private readonly ICPManageRepository _Repository;
        private readonly IJsonMethod _Json;
        private readonly IPublicSystemRepository _Sys;
        private readonly IPublicKfyfRepository _Kfyf;
        private readonly IPublicYwxhRepository _Ywxh;
        private readonly ICommon _Com;
        private readonly ICPPublicRepository _Public;
        public CPManageServices(ICPManageRepository Repository, IJsonMethod Json, IPublicSystemRepository Sys, IPublicKfyfRepository Kfyf, IPublicYwxhRepository Ywxh, ICommon com, ICPPublicRepository Public)
        {
            _Repository = Repository;
            _Json = Json;
            _Sys = Sys;
            _Kfyf = Kfyf;
            _Com = com;
            _Ywxh = Ywxh;
            _Public = Public;
        }
        #region 入径登记
        /// <summary>
        /// 查询查询科室与疾病对应的路径名称
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetListCPNameForDeptIcd(JsonObject data)
        {
            string ksbm = _Json.InString(data["ksbm"]);
            if (string.IsNullOrWhiteSpace(ksbm))
            {
                return _Json.GetErrMsg("科室编码为空，不能查询！");
            }
            string jbbm = _Json.InString(data["jbbm"]);
            if (string.IsNullOrWhiteSpace(jbbm))
            {
                string zyh = _Json.InString(data["zyh"]);
                if (string.IsNullOrWhiteSpace(zyh))
                {
                    return _Json.GetErrMsg("住院号与疾病编码不能同时为空，不能查询！");
                }
                jbbm = await _Sys.GetSingle<INHOSD_BRZD_VM>(a => a.zdbm, a => a.zyh == zyh && a.zzd == "1");
            }
            if (string.IsNullOrWhiteSpace(jbbm))
            {
                return _Json.GetErrMsg("疾病编码为空，不能查询！");
            }
            int page = _Json.InInt(data["page"]);
            int limit = _Json.InInt(data["limit"]);
            string filter = _Json.InString(data["filterstr"]);
            RefAsync<int> total = 0;
            var list = await _Repository.GetListCPNameForDeptIcd(ksbm, jbbm, page, limit,filter,total);
            return _Json.GetResults(page, limit, total.Value, list);
        }
        /// <summary>
        /// 入径登记
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> SaveCPEntry(JsonObject data)
        {
            StringBuilder msg = new StringBuilder();
            CP_DJJL_VM djjl = _Json.InEntities<CP_DJJL_VM>(data, msg);
            if (djjl == null)
            {
                return _Json.GetErrMsg("获取入径信息错误！" + msg.ToString());
            }
            if (string.IsNullOrWhiteSpace(djjl.zyh))
            {
                return _Json.GetErrMsg("住院号为空，不能登记！");
            }

            INHOS_RYDJ_VM rydj = await _Sys.GetInhosInfo(djjl.zyh);
            if (rydj == null)
            {
                return _Json.GetErrMsg($"住院号：{djjl.zyh}无效！");
            }

            //判断出院
            msg.Clear();
            if (!await _Sys.JudgeZyzt(rydj, msg))
            {
                return _Json.GetErrMsg(msg.ToString());
            }

            //if (string.IsNullOrWhiteSpace(djjl.ryks))
            //{
            //    return _Json.GetErrMsg("住院科室为空，不能登记！");
            //}

            CP_DJJL_VM djjlold = await _Repository.GetEntity<CP_DJJL_VM>(a => a.zyh == djjl.zyh && (a.ztbz == "0" || a.ztbz == "3"));
            if (djjlold != null)
            {
                return _Json.GetErrMsg($"住院号：{djjl.zyh}已入径，不能重复入径！");
            }

            //路径判断
            if (string.IsNullOrWhiteSpace(djjl.ljbm))
            {
                return _Json.GetErrMsg("路径为空，不能登记！");
            }
            CP_LJMC_VM ljmc = await _Sys.GetEntity<CP_LJMC_VM>(a => a.ljbm == djjl.ljbm);
            if (ljmc == null)
            {
                return _Json.GetErrMsg($"路径编码：{djjl.ljbm}无效！");
            }

            //疾病判断
            var listjbbm = await _Repository.GetListCpIcd(djjl.ljbm);
            if(listjbbm == null || listjbbm.Count <= 0)
            {
                return _Json.GetErrMsg($"路径：{djjl.ljbm}没有对应疾病！");
            }
            var ryzd = await _Sys.GetEntity<INHOSD_BRZD_VM>(a => a.zyh == djjl.zyh && a.zzd == "1");
            if (ryzd == null)
            {
                return _Json.GetErrMsg($"当前病人未填写入院主诊断，不能入径！");
            }
            djjl.jbbm = ryzd.zdbm;


            //科室判断
            if (string.IsNullOrWhiteSpace(djjl.ryks))
            {
                djjl.ryks = rydj.zyks;
            }
            if (!await _Sys.GetIsExist<PU_KS_VM>(a => a.ksbm == djjl.ryks))
            {
                return _Json.GetErrMsg($"住院科室编码：{djjl.ryks}无效！");
            }
            if (djjl.ryks != rydj.zyks)
            {
                return _Json.GetErrMsg($"入径科室编码：{djjl.ryks} 与病人当前科室编码：{rydj.zyks} 不一致！");
            }

            //判断科室下是否有疾病对应的临床路径名称
            var listljmc = await _Repository.GetListCPNameForDeptIcd(djjl.ryks, djjl.jbbm);
            if (listljmc.Count <= 0)
            {
                return _Json.GetErrMsg($"当前科室无诊断：{ryzd.zdmc}对应的临床路径名称！");
            }                    

            //操作员判断
            djjl.djczy = _Com.GetUserIdByToken();
            if (string.IsNullOrWhiteSpace(djjl.djczy))
            {
                return _Json.GetErrMsg("当前操作员为空，不能登记！");
            }
            if (!await _Sys.GetIsExist<PU_CZY_VM>(a => a.czybm == djjl.djczy))
            {
                return _Json.GetErrMsg($"当前操作员编码：{djjl.djczy}无效！");
            }

            //科室、疾病、路径是否匹配
            if (await _Repository.JudgeCPNameForDeptIcd(djjl.ryks, djjl.jbbm, djjl.ljbm) <= 0)
            {
                return _Json.GetErrMsg($"科室：{djjl.ryks}、疾病：{djjl.jbbm}、路径：{djjl.ljbm}不匹配");
            }

            //入院病情判断
            if (!string.IsNullOrWhiteSpace(rydj.ryqk) && ljmc.sybq != "0" && ljmc.sybq != rydj.ryqk)
            {
                return _Json.GetErrMsg($"病人入院病情：{rydj.ryqk}与路径限制的入院病情：{ljmc.sybq}不一致，不能登记！");
            }

            //性别判断
            if (ljmc.syxb != "0" && ljmc.syxb != rydj.rybrxb)
            {
                return _Json.GetErrMsg($"病人性别：{rydj.rybrxb}与路径限制的性别：{ljmc.syxb}不一致，不能登记！");
            }

            DateTime xtrq = _Sys.GetXtrq();
            djjl.djrq = xtrq;
            //入径时间判断
            if (data["rjrq"] == null)
            {
                djjl.rjrq = xtrq;
            }
            if (djjl.rjrq < rydj.ryrq)
            {
                return _Json.GetErrMsg($"入径时间：{djjl.rjrq}不能晚于入院时间：{rydj.ryrq}");
            }
            if (djjl.rjrq > xtrq)
            {
                return _Json.GetErrMsg($"入径时间：{djjl.rjrq}不能大于当前时间");
            }

            //入径限定时间判断
            if (ljmc.rjxdsj != null && ljmc.rjxdsj.Value > 0)
            {
                if (_Com.DateDiff(rydj.ryrq, xtrq) > ljmc.rjxdsj.Value)
                {
                    return _Json.GetErrMsg("已超过路径限定入径时间，不能入径！");
                }
            }

            djjl.ztbz = "0";

            return _Json.GetResults(await _Repository.SaveCPEntry(djjl));
        }
        /// <summary>
        /// 取消入径
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> CancelCPEntry(JsonObject data)
        {
            string djid = _Json.InString(data["djid"]);
            if (string.IsNullOrWhiteSpace(djid))
            {
                return _Json.GetErrMsg("路径登记ID为空，不能取消入径！");
            }
            string czybm = _Com.GetUserIdByToken();
            if (string.IsNullOrWhiteSpace(czybm))
            {
                return _Json.GetErrMsg("操作员为空，不能取消入径！");
            }
            if (!await _Sys.GetIsExist<PU_CZY_VM>(a => a.czybm == czybm))
            {
                return _Json.GetErrMsg($"操作员编码：{czybm}无效！");
            }
            string qxyy = _Json.InString(data["qxyy"]);
            if (string.IsNullOrWhiteSpace(qxyy))
            {
                return _Json.GetErrMsg("取消原因为空，不能取消入径！");
            }

            CP_DJJL_VM djjl = await _Sys.GetEntity<CP_DJJL_VM>(a => a.id == djid); //路径登记记录
            if (djjl == null)
            {
                return _Json.GetErrMsg($"路径登记ID：{djid}无效！");
            }
            CP_QXSQJL_VM qxjl = await _Sys.GetEntity<CP_QXSQJL_VM>(it => it.djid == djid); //取消记录
            if (qxjl == null)
            {
                qxjl = new CP_QXSQJL_VM();
                qxjl.djid = djid;
                qxjl.sqr = czybm;
                qxjl.sqrq = _Sys.GetXtrq();
                qxjl.shbz = "0";
                qxjl.sqlx = "0";
                qxjl.sqyy = qxyy;

                return _Json.GetResults(await _Repository.SaveCPCancelApply(qxjl));
            }
            else
            {
                if (qxjl.shbz == "0")
                {
                    return _Json.GetErrMsg("该病员的取消入径申请还未审核，请先通知主管部门审核！");
                }
                djjl.tcczy = czybm;
                djjl.tcrq = _Sys.GetXtrq();
                djjl.ztbz = "1";
                djjl.byyy = qxyy;

                return _Json.GetResults(await _Repository.UpdateCPEntry(djjl, new string[] { "tcczy", "tcrq", "ztbz", "byyy" }));
            }
        }
        #endregion

        #region 路径执行
        /// <summary>
        /// 查询科室入径病人
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetListCPPatient(JsonObject data)
        {
            string ksbm = _Json.InString(data["ksbm"]);
            if (string.IsNullOrWhiteSpace(ksbm))
            {
                return _Json.GetErrMsg("科室为空，不能查询！");
            }
            string filter = _Json.InString(data["filterstr"]);
            int page = _Json.InInt16(data["page"]);
            int limit = _Json.InInt16(data["limit"]);

            DateTime? rq1 = _Json.InDateNull(data["rq1"]);
            DateTime? rq2 = _Json.InDateNull(data["rq2"]);

            string cybz = _Json.InString(data["cybz"]); //出院标志 0-在院 1-出院
            string ztbz = _Json.InString(data["ztbz"]); //路径状态标志 0=正常在院 1=取消 2=变异 3=正常结束

            string gcbr = _Json.InString(data["gcbr"]); //管床病人 1-只查询管床病人
            string czybm = _Com.GetUserIdByToken();
            RefAsync<int> total = 0;

            var list = await _Repository.GetListCPPatient(ksbm, cybz, ztbz, gcbr, czybm, rq1, rq2, page, limit, filter, total);
            return _Json.GetResults(page, limit, total, list);
        }

        //取单个路径阶段

        /// <summary>
        /// 病人执行路径阶段记录
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetListPatientCPPhase(JsonObject data)
        {
            string zyh = _Json.InString(data["zyh"]);
            if (string.IsNullOrWhiteSpace(zyh))
            {
                return _Json.GetErrMsg("住院号为空，不能查询！");
            }
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能查询！");
            }
            return _Json.GetResults(await _Repository.GetListPatientCPPhase(zyh, ljbm));
        }
        /// <summary>
        /// 查询病人路径执行记录
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetCPExecutionRecordDetails(JsonObject data)
        {
            string zyh = _Json.InString(data["zyh"]);
            if (string.IsNullOrWhiteSpace(zyh))
            {
                return _Json.GetErrMsg("住院号为空，不能查询！");
            }
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
            string filter = _Json.InString(data["filterstr"]);
            int page = _Json.InInt16(data["page"]);
            int limit = _Json.InInt16(data["limit"]);

            RefAsync<int> total = 0;

            var list = await _Repository.GetCPExecutionRecordDetails(zyh, ljbm, jdbm, page, limit, filter, total);
            return _Json.GetResults(page, limit, total, list);
        }
        /// <summary>
        /// 获取病人路径阶段评估记录
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetCPEvaluationRecord(JsonObject data)
        {
            string zyh = _Json.InString(data["zyh"]);
            if (string.IsNullOrWhiteSpace(zyh))
            {
                return _Json.GetErrMsg("住院号为空，不能查询！");
            }
            string jdbm = _Json.InString(data["jdbm"]);
            if (string.IsNullOrWhiteSpace(jdbm))
            {
                return _Json.GetErrMsg("阶段编码为空，不能查询！");
            }
            return _Json.GetResults(await _Repository.GetCPEvaluationRecord(zyh, jdbm));
        }
        /// <summary>
        /// 保存评估
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> SaveCPEvaluation(JsonObject data)
        {
            string ksbm = _Json.InString(data["ksbm"]);
            if (string.IsNullOrWhiteSpace(ksbm))
            {
                return _Json.GetErrMsg("科室编码为空，不能保存！");
            }
            StringBuilder msg = new StringBuilder();
            CP_PGJL_VM pgjl = _Json.InEntities<CP_PGJL_VM>(data);
            if(pgjl == null)
            {
                return _Json.GetErrMsg("获取评估信息错误！" + msg.ToString());
            }
            if (string.IsNullOrWhiteSpace(pgjl.zyh))
            {
                return _Json.GetErrMsg("住院号为空，不能保存！");
            }
            pgjl.pgczy = _Com.GetUserIdByToken();
            if (string.IsNullOrWhiteSpace(pgjl.pgczy))
            {
                return _Json.GetErrMsg("评估人员为空，不能保存！");
            }

            CP_DJJL_VM djjl = await _Sys.GetEntity<CP_DJJL_VM>(a => a.zyh == pgjl.zyh && a.ryks == ksbm && a.ztbz != "1");
            if (djjl == null)
            {
                return _Json.GetErrMsg($"未找到住院号：{pgjl.zyh}的有效路径登记信息");
            }

            if (string.IsNullOrWhiteSpace(pgjl.jdbm))
            {
                return _Json.GetErrMsg("路径阶段为空，不能保存！");
            }
            if (string.IsNullOrWhiteSpace(pgjl.ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能保存！");
            }
            if(await _Sys.GetIsExist<CP_PGJL_VM>(a => a.zyh == pgjl.zyh && a.jdbm == pgjl.jdbm && a.ljbm == pgjl.ljbm))
            {
                return _Json.GetErrMsg("当前阶段已经评估，不能重复评估！");
            }
            if (string.IsNullOrWhiteSpace(pgjl.pgzt))
            {
                return _Json.GetErrMsg("请指定评估情况！");
            }
            if (string.IsNullOrWhiteSpace(pgjl.pgjg))
            {
                return _Json.GetErrMsg("请填写评估结果！");
            }
            //执行结果判断
            string zxjg = await _Sys.GetSingle<CP_LJXM_ZXJL_VM>(a => a.zxjg, a => a.zyh == pgjl.zyh && a.ljbm == pgjl.ljbm && a.jdbm == pgjl.jdbm && a.zxjg == "1");
            if(zxjg != "1")
            {
                return _Json.GetErrMsg("路径阶段项目必须执行后才能进行评估！");
            }

            DateTime xtrq = _Sys.GetSysDate();
            //判断变异退出审核
            string issctcsq = _Json.InString(data["sctcsq"]); //是否自动生成退出申请
            if (pgjl.pgzt == "2") //退出
            {
                CP_QXSQJL_VM qxsq = await _Sys.GetEntity<CP_QXSQJL_VM>(a => a.djid == djjl.id && a.sqlx == "1");
                if(qxsq == null)
                {
                    if(issctcsq != "1")
                    {
                        return _Json.GetErrMsg("变异退出必须先申请，请申请后再试！");
                    }
                    qxsq = new CP_QXSQJL_VM();
                    qxsq.djid = djjl.id;
                    qxsq.sqr = pgjl.pgczy;
                    qxsq.sqrq = xtrq;
                    qxsq.shbz = "0";
                    qxsq.sqlx = "1";
                    qxsq.sqyy = pgjl.pgjg;

                    DbResult<bool> res = await _Repository.SaveCPCancelApply(qxsq);
                    if(res.IsSuccess)
                    {
                        return _Json.GetErrMsg("变异退出申请保存成功，请通知主管部门审核！");
                    }
                    else
                    {
                        return _Json.GetErrMsg("变异退出申请保存失败！" + res.ErrorMessage);
                    }
                }
                else
                {
                    if(qxsq.shbz == "0")
                    {
                        return _Json.GetErrMsg("请先通知主管部门审核变异退出申请！");
                    }
                }
                djjl.ztbz = "2";
            }
            else if(pgjl.pgzt == "3")
            {
                djjl.ztbz = "3";
            }
            else
            {
                djjl.ztbz = "0";
            }
            djjl.byyy = pgjl.pgjg;
            djjl.tcczy = pgjl.pgczy;
            djjl.tcrq = xtrq;

            pgjl.pgrq = xtrq;
            return _Json.GetResults(await _Repository.SaveCPEvaluation(pgjl, djjl, new string[] { "ztbz", "byyy", "tcczy", "tcrq" }));
        }
        /// <summary>
        /// 取消评估
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> CancelCPEvaluation(JsonObject data)
        {
            string id = _Json.InString(data["id"]);
            string zyh = _Json.InString(data["zyh"]);
            string jdbm = _Json.InString(data["jdbm"]);

            CP_PGJL_VM pgjl;
            if (string.IsNullOrWhiteSpace(id))
            {
                if (string.IsNullOrWhiteSpace(zyh) || string.IsNullOrWhiteSpace(jdbm))
                {
                    return _Json.GetErrMsg("评估ID为空时，住院号与阶段不能为空！");
                }
                pgjl = await _Sys.GetEntity<CP_PGJL_VM>(a => a.zyh == zyh && a.jdbm == jdbm);
                if (pgjl == null)
                {
                    return _Json.GetErrMsg($"未查询到住院号：{zyh}，阶段编码：{jdbm} 的评估记录！");
                }
            }
            else
            {
                pgjl = await _Sys.GetEntity<CP_PGJL_VM>(a => a.id == id);
                if (pgjl == null)
                {
                    return _Json.GetErrMsg($"未查询到ID：{id}的评估记录！");
                }
            }

            if(await _Sys.GetCount<CP_PGJL_VM>(a => a.pgrq > pgjl.pgrq) > 0)
            {
                return _Json.GetErrMsg("取消评估必须从最后一次评估取消");
            }
            return _Json.GetResults(_Repository.DelCPEvaluation(pgjl));
        }
        /// <summary>
        /// 路径执行前判断
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> CPPreExecutionJudgment(JsonObject data)
        {
            string zyh = _Json.InString(data["zyh"]);
            string ljbm = _Json.InString(data["ljbm"]);
            string jdbm = _Json.InString(data["jdbm"]);
            string pgzt = _Json.InString(data["pgzt"]);
            StringBuilder msg = new StringBuilder();
            if (await CPPreExecutionJudgment(zyh, ljbm, jdbm, pgzt, msg))
            {
                return _Json.GetResults();
            }
            else
            {
                return _Json.GetErrMsg(msg.ToString());
            }
        }
        /// <summary>
        /// 判断路径是否可执行
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="ljbm"></param>
        /// <param name="jdbm"></param>
        /// <param name="pgzt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private async Task<bool> CPPreExecutionJudgment(string zyh, string ljbm, string jdbm, string pgzt, StringBuilder msg)
        {
            if (string.IsNullOrWhiteSpace(zyh))
            {
                msg.Append("住院号为空，不能执行！");
                return false;
            }
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                msg.Append("路径编码为空，不能执行！");
                return false;
            }
            if (string.IsNullOrWhiteSpace(jdbm))
            {
                msg.Append("阶段编码为空，不能执行！");
                return false;
            }


            //路径名称判断
            CP_LJMC_VM ljxx = await _Sys.GetEntity<CP_LJMC_VM>(a => a.ljbm == ljbm);
            if (ljxx == null)
            {
                msg.Append($"路径编码：{ljbm}无效！");
                return false;
            }

            //住院状态判断
            if (!await _Sys.JudgeZyzt(zyh, msg))
            {
                msg.Append(msg.ToString());
                return false;
            }

            //路径登记判断
            CP_DJJL_VM djjl = await _Sys.GetEntity<CP_DJJL_VM>(a => a.zyh == zyh && a.ljbm == ljbm && a.ztbz == "0");
            if (djjl == null)
            {
                msg.Append($"住院号：{zyh}已取消或退出路径，不能再执行路径！");
                return false;
            }

            //评估记录
            var listpgjl = await _Repository.GetListPatientCPEvaluation(zyh);

            //判断上一阶段或下一阶段是否已评估
            var listjd = await _Public.GetListPhaseForCPCode(ljbm); //路径阶段
            if (listjd.Count <= 0)
            {
                msg.Append($"路径：{ljxx.ljmc}没有对应的路径阶段！");
            }
            var ljjd = listjd.Find(it => it.jdbm == jdbm); //当前阶段
            if (ljjd == null)
            {
                msg.Append($"阶段编码：{jdbm}无效！");
                return false;
            }
            int find = -1;
            foreach (var jd in listjd)
            {
                if (jd.jdhf < ljjd.jdhf) //之前阶段
                {
                    find = listpgjl.FindIndex(it => it.jdbm == jd.jdbm && it.zyh == zyh);
                    if (find < 0)
                    {
                        msg.Append($"请评估：{jd.jdmc}后才能执行此阶段！");
                        return false;
                    }
                }
                else if (jd.jdhf > ljjd.jdhf) //之后阶段
                {
                    find = listpgjl.FindIndex(it => it.jdbm == jd.jdbm && it.zyh == zyh);
                    if (find >= 0)
                    {
                        msg.Append($"阶段：{jd.jdmc}已评估，不能执行此阶段！");
                        return false;
                    }
                }
            }

            //判断出院日期
            if (pgzt == "3" && ljjd.jdbz != "3")
            {
                msg.Append($"阶段：{ljjd.jdmc}非出院日，评估情况不能为正常出院！");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 查询路径、阶段、治疗方案下的医嘱列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetListCPMedicalAdvice(JsonObject data)
        {
            string zyh = _Json.InString(data["zyh"]);
            if (string.IsNullOrWhiteSpace(zyh))
            {
                return _Json.GetErrMsg("住院号为空，不能执行！");
            }
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径编码为空，不能执行！");
            }
            string jdbm = _Json.InString(data["jdbm"]);
            if (string.IsNullOrWhiteSpace(jdbm))
            {
                return _Json.GetErrMsg("阶段编码为空，不能执行！");
            }
            string zlfabm = _Json.InString(data["zlfabm"]);
            if (string.IsNullOrWhiteSpace(zlfabm))
            {
                return _Json.GetErrMsg("治疗方案编码为空，不能执行！");
            }
            string filter = _Json.InString(data["filterstr"]);
            int page = _Json.InInt16(data["page"]);
            int limit = _Json.InInt16(data["limit"]);

            string strljxmbm = _Json.InString(data["ljxmbm"]);
            if (string.IsNullOrWhiteSpace(strljxmbm))
            {
                return _Json.GetErrMsg("请选择要执行的路径项目！");
            }
            List<string> listljxmbm = _Com.StrToList(strljxmbm,',');

            RefAsync<int> total = 0;
            var list = await _Repository.GetListCpMedicalAdvice(ljbm, jdbm, zlfabm, zyh, listljxmbm, page, limit, filter, total);
            return _Json.GetResults(page,limit,total.Value,list);
        }
        /// <summary>
        /// 路径执行
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> CPPreExecution(JsonObject data)
        {
            string ksbm = _Json.InString(data["ksbm"]);
            if (string.IsNullOrWhiteSpace(ksbm))
            {
                return _Json.GetErrMsg("科室为空，不能执行！");
            }
            if(!await _Sys.GetIsExist<PU_KS_VM>(a => a.ksbm == ksbm))
            {
                return _Json.GetErrMsg($"科室编码：{ksbm}无效！");
            }
            string czybm = _Com.GetUserIdByToken();
            if (string.IsNullOrWhiteSpace(czybm))
            {
                return _Json.GetErrMsg("当前操作人员为空，不能执行！");
            }
            string czyks = await _Sys.GetUserDept(czybm);
            if (string.IsNullOrWhiteSpace(czyks))
            {
                return _Json.GetErrMsg($"操作人员编码：{czybm}无效！");
            }
            string zyh = _Json.InString(data["zyh"]);
            string ljbm = _Json.InString(data["ljbm"]);
            string jdbm = _Json.InString(data["jdbm"]);
            string pgzt = _Json.InString(data["pgzt"]);
            StringBuilder msg = new StringBuilder();
            if (!await CPPreExecutionJudgment(zyh, ljbm, jdbm, pgzt, msg))
            {
                return _Json.GetErrMsg(msg.ToString());
            }

            msg.Clear();
            List<CPAdviceDetail_Model> listyzmx = _Json.InList<CPAdviceDetail_Model>(data["yzxm"],msg); //医嘱信息
            if(listyzmx == null || listyzmx.Count <= 0)
            {
                return _Json.GetErrMsg("获取路径医嘱出错！" + msg.ToString());
            }
            DateTime? yzrq = _Json.InDateNull(data["yzrq"]);
            if (yzrq == null)
            {
                return _Json.GetErrMsg("医嘱开始时间为空，不能执行！");
            }
            string yebh = _Json.InString(data["yebh"]); //婴儿编号

            DateTime xtrq = _Sys.GetXtrq();

            //根据医嘱编码获取医嘱项目信息
            List<CPAdviceDetail_Model> listyzxm = await _Repository.GetListCpMedicalAdviceExecution(listyzmx.Select(x => x.yzbm).ToList()); 
            //List<CP_YZXM_VM> listyzxm = await _Sys.GetEntityList<CP_YZXM_VM>(a => listyzmx.Select(x => x.yzbm).ToList().Contains(a.yzbm));
            if (listyzxm.Count <= 0)
            {
                return _Json.GetErrMsg($"医嘱项目无效！");
            }
            //根据医嘱编码获取路径项目
            List<CP_LJXM_VM> listljxm = await _Public.GetListCPProjectForCodeList(listyzxm.Select(it => it.ljxmbm).ToList());

            //已执行记录 保存用
            List<CP_ZXJL_VM> listyzzxjl = await _Sys.GetEntityList<CP_ZXJL_VM>(a => a.zyh == zyh && a.ljbm == ljbm && a.jdbm == jdbm);
            //已执行项目执行记录 保存用
            List<CP_LJXM_ZXJL_VM> listljxmzjjl = await _Sys.GetEntityList<CP_LJXM_ZXJL_VM>(a => a.zyh == zyh && a.jdbm == jdbm);
            
            //系统参数
            List<Csqx_vm> listcs = await _Sys.GetCs(czybm, "00800100101,00800100103,00800100108,00800100109");
            string isxyyfbm = _Com.FindCsz(listcs, "00800100101"); //默认西药房
            string iszyyfbm = _Com.FindCsz(listcs, "00800100103"); //默认中药房
            string iscfscyz = _Com.FindCsz(listcs, "00800100108"); //医嘱是否重复生成
            string iszdtycqyz = _Com.FindCsz(listcs, "00800100109"); //自动停用长期医嘱
            if (string.IsNullOrWhiteSpace(iscfscyz)) { iscfscyz = "0"; }
            if (string.IsNullOrWhiteSpace(iszdtycqyz)) { iszdtycqyz = "0"; }

            //中药医嘱
            List<CP_YZXM_ZYMX_VM> listzyyzmx = await _Sys.GetEntityList<CP_YZXM_ZYMX_VM>(a => listyzxm.Select(it => it.ljxmbm).ToList().Contains(a.yzbm));

            List<YF_PCKC_VM> listpckc = await _Kfyf.GetYfPckcNoZeroYxq(isxyyfbm); //药房批次库存
            if(listpckc.Count <= 0)
            {
                return _Json.GetErrMsg($"默认药房：{isxyyfbm}无库存！");
            }
            List<AdviceDetailSimple_Model> listyzxxold = await _Repository.GetListPatientAdvice(zyh,jdbm); //病人原有的阶段医嘱信息
            List<INHOSD_YLYZ_VM> listylyz = new List<INHOSD_YLYZ_VM>(); //保存医疗医嘱
            List<INHOSD_YLYZMX_VM> listylyzmx = new List<INHOSD_YLYZMX_VM>(); //保存医疗医嘱明细
            List<INHOSD_YPYZ_VM> listypyz = new List<INHOSD_YPYZ_VM>(); //保存药品医嘱
            List<INHOSD_YPYZMX_VM> listypyzmx = new List<INHOSD_YPYZMX_VM>(); //保存药品医嘱明细
            List<INHOSD_YPYZMX_VM> listypyzmx_tz = new List<INHOSD_YPYZMX_VM>(); //保存药品医嘱停嘱
            List<INHOSD_YLYZMX_VM> listylyzmx_tz = new List<INHOSD_YLYZMX_VM>(); //保存医疗医嘱停嘱
            List<Error_Model> listerr = new List<Error_Model>(); //错误信息
            List<YK_YPZD_VM> listypzd = await _Sys.GetEntityList<YK_YPZD_VM>(a => a.tybz == "0"); //药品字典

            //写路径项目执行记录
            foreach(var ljxm in listljxm)
            {
                int find = listljxmzjjl.FindIndex(it => it.xmbm == ljxm.xmbm);
                if(find < 0) //没有记录，新增
                {
                    listljxmzjjl.Add(new CP_LJXM_ZXJL_VM
                    {
                        zyh = zyh,
                        xmbm = ljxm.xmbm,
                        ljbm = ljxm.ljbm,
                        jdbm = ljxm.jdbm,
                        xmnr = ljxm.xmnr,
                        zxjg = "1",
                        jdpg = ljxm.xmjdpg,
                        zxr = czybm,
                        zxsj = xtrq
                    });
                }
                else //有记录，修改
                {
                    if (listljxmzjjl[find].zxjg != "0")
                    {
                        listljxmzjjl[find].zxr = czybm;
                        listljxmzjjl[find].zxsj = xtrq;
                    }
                    else
                    {
                        listljxmzjjl[find].zxjg = "1";
                        listljxmzjjl[find].jdpg = ljxm.xmjdpg;
                    }
                }
            }

            #region 生成医嘱明细
            int xyxh = 0;
            int ylxh = 0;
            for (int i = 0; i < listyzmx.Count; i++)
            {
                var yzmx = listyzmx[i];
                if (string.IsNullOrWhiteSpace(yzmx.yzbm))
                {
                    listerr.Add(new Error_Model
                    {
                        msg = $"第{i + 1}行医嘱项目为空！"
                    });
                    continue;
                }
                var yzxm = listyzxm.Find(it => it.yzbm == yzmx.yzbm);
                if (yzxm == null)
                {
                    listerr.Add(new Error_Model
                    {
                        msg = $"第{i + 1}行医嘱项目编码：{yzmx.yzbm}无效！"
                    });
                    continue;
                }

                #region 医嘱执行记录处理
                int find = listyzzxjl.FindIndex(it => it.yzbm == yzmx.yzbm);
                if (find < 0) //未执行过，添加执行记录
                {
                    listyzzxjl.Add(new CP_ZXJL_VM
                    {
                        zyh = zyh,
                        ljbm = ljbm,
                        jdbm = jdbm,
                        zyts = 0,
                        ypbz = yzxm.yzpd,
                        xmbm = yzxm.ljxmbm,
                        yzmxxmbm = yzxm.yzmxxmbm,
                        zxzt = "1",
                        zxrq = xtrq,
                        zxczy = czybm,
                        yzbm = yzmx.yzbm,
                    });
                }
                else //已执行过，修改执行时间
                {
                    listyzzxjl[find].zxzt = "1";
                    listyzzxjl[find].zxrq = xtrq;
                    listyzzxjl[find].zxczy = czybm;
                }
                #endregion

                #region 药品库存判断
                if (yzxm.yzpd == "1") //医嘱类别 0=医疗医嘱 1=药品医嘱 2=嘱托医嘱
                {
                    var ypzd = listypzd.Find(it => it.ypbm == yzxm.yzmxxmbm);
                    if(ypzd == null)
                    {
                        listerr.Add(new Error_Model
                        {
                            msg = $"第{i + 1}行药品：{yzxm.yzmxxmmc}[编码{yzmx.yzmxxmbm}]在字典中并不存在！"
                        });
                    }
                    decimal kcsl = listpckc.Where(it => it.ryypbm == yzxm.yzmxxmbm).Sum(it => it.kcsl); //库存数量
                    decimal yzyl = listyzmx.Where(it => it.yzpd == "1" && it.yzmxxmbm == yzmx.yzmxxmbm).Sum(it => it.zl); //药品总用量
                    if (kcsl < yzyl)
                    {
                        //换药，同名同规格药品
                        var listpckctm = listpckc.Where(it => it.ypmc == yzxm.yzmxxmbm && it.ypgg == ypzd.ypgg).ToList();
                        if (listpckctm == null || listpckctm.Count() <= 0)
                        {
                            listerr.Add(new Error_Model
                            {
                                msg = $"第{i + 1}行药品：{yzxm.yzmxxmmc}库存：{kcsl}，小于药品总用量：{yzyl}！"
                            });
                            continue;
                        }
                        else
                        {
                            string hybz = "0"; //换药标志
                            for(int k = 0; k < listpckctm.Count(); k++)
                            {
                                var item = listpckctm[k];
                                var hykcsl = listpckctm.Where(it => it.ryypbm == item.ryypbm).Sum(it => it.kcsl); //库存数量
                                if (hykcsl >= yzyl)
                                {
                                    yzxm.yzmxxmbm = item.ryypbm;
                                    yzxm.yzmxxmmc = item.ypmc;
                                    hybz = "1";
                                    listerr.Add(new Error_Model
                                    {
                                        msg = $"第{i + 1}行药品：{yzxm.yzmxxmmc}[编码{yzmx.yzmxxmbm}]库存：{kcsl}，小于药品总用量：{yzyl}，已将药品更换为编码：{item.ryypbm}的同名同规格药品！"
                                    });
                                }
                            }
                            if(hybz == "0")
                            {
                                listerr.Add(new Error_Model
                                {
                                    msg = $"第{i + 1}行药品：{yzxm.yzmxxmmc}库存：{kcsl}，小于药品总用量：{yzyl}！"
                                });
                                continue;
                            }
                        }
                    }
                }
                #endregion

                #region 生成医嘱
                //医嘱是否已执行 是就跳过
                find = listyzxxold.FindIndex(it => it.lclj_yzbm == yzmx.yzbm);
                if (find >= 0)
                {
                    listerr.Add(new Error_Model
                    {
                        msg = $"第{i + 1}行项目：{yzxm.yzmxxmmc}已有执行记录！"
                    });
                    continue;
                }

                //中药医嘱
                if(yzxm.sfcy == "1")
                {
                    List<CP_YZXM_ZYMX_VM> listzyyzmxtem = listzyyzmx.Where(it => it.yzbm == yzxm.yzbm).ToList();
                    if(listzyyzmxtem.Count > 0)
                    {
                        string ypyzxh = await _Ywxh.GetAdviceSn("药品", xtrq);
                        if (string.IsNullOrWhiteSpace(ypyzxh))
                        {
                            return _Json.GetErrMsg("生成中药药品医嘱序号失败");
                        }
                        int zyxh = 0;
                        listypyz.Add(new INHOSD_YPYZ_VM
                        {
                            ypyzxh = ypyzxh,
                            ksbm = ksbm,
                            zyh = zyh,
                            yzlx = "0",
                            ypyzfl = "0",
                            sfye = string.IsNullOrWhiteSpace(yebh) ? "0" : "1",
                            yebh = yebh,
                            ksrq = yzrq.Value,
                            djry = czybm,
                            djsj = xtrq,
                            xdys = czybm,
                            ysks = czyks,
                            sfcy = "1",
                            fjmc = yzxm.fjmc,
                            fjsl = yzxm.fjsl == null ? 0 : yzxm.fjsl.Value,
                            zcyyf = yzxm.zcyyf,
                            yfbm = iszyyfbm
                        }) ;
                        foreach(var vm in listzyyzmxtem)
                        {
                            zyxh++;
                            listypyzmx.Add(new INHOSD_YPYZMX_VM
                            {
                                ypyzxh = ypyzxh,
                                ypmxxh = zyxh,
                                tjbm = vm.tjbm,
                                ryypbm = vm.ypbm,
                                fzh = 0,
                                dcjl = vm.dcsl,
                                jldw = vm.jldw,
                                yyts = yzxm.fjsl == null ? 0 : yzxm.fjsl.Value,
                                yyzl = vm.dcsl * (yzxm.fjsl == null ? 0 : yzxm.fjsl.Value),
                                yysm = yzxm.yssm,
                                shbz = "0",
                                ystzbz = "0",
                                hstzbz = "0",
                                tsyz = "0",
                                psypbz = "0",
                                qmsj = xtrq,
                                lclj_jdbm = jdbm,
                                lclj_yzbm = yzxm.yzbm,
                                yzzl = "zy",
                            });
                        }
                    }
                }
                else if (yzxm.yzpd == "1") //药品
                {
                    #region 药品医嘱明细
                    xyxh++;
                    listypyzmx.Add(new INHOSD_YPYZMX_VM
                    {
                        tjbm = yzxm.yyff,
                        pcbm = yzxm.pcbm,
                        ryypbm = yzxm.yzmxxmbm,
                        fzh = yzxm.yzfzh == null ? 0 : yzxm.yzfzh.Value,
                        dcjl = yzxm.dcjl == null ? 0 : yzxm.dcjl.Value,
                        jldw = yzxm.jldw,
                        yyts = 1,
                        yyzl = yzxm.zl,
                        sysd = string.IsNullOrWhiteSpace(yzxm.sysd) ? null : Convert.ToDecimal(yzxm.sysd),
                        sydw = yzxm.sydw,
                        yysm = yzxm.yssm,
                        shbz = "0",
                        ystzbz = "0",
                        hstzbz = "0",
                        tsyz = "0",
                        psypbz = yzxm.psypbz,
                        qmsj = yzrq,
                        yzlx = string.IsNullOrWhiteSpace(yzxm.yzlx) ? "0" : yzxm.yzlx,
                        yzzl = string.IsNullOrWhiteSpace(yzxm.yzzl) ? "0" : yzxm.yzzl,
                        lclj_jdbm = jdbm,
                        lclj_yzbm = yzxm.yzbm,
                        ypmxxh = xyxh,
                        ryypmc = yzxm.yzmxxmmc,
                        ryypgg = yzxm.gg
                    });
                    #endregion
                }
                else //医疗医嘱
                {
                    #region 医疗医嘱明细
                    ylxh++;
                    listylyzmx.Add(new INHOSD_YLYZMX_VM
                    {
                        tsyz = yzxm.yzpd == "2" ? "1" : "0",
                        mxzlxmbm = yzxm.yzmxxmbm,
                        fzh = yzxm.yzfzh == null ? 0 : yzxm.yzfzh.Value,
                        sl = yzxm.zl,
                        pcbm = yzxm.pcbm,
                        gllb = yzxm.ylyzfl,
                        hldj = yzxm.hldj,
                        yzms = yzxm.yssm,
                        shbz = "0",
                        ystzbz = "0",
                        hstzbz = "0",
                        qmsj = yzrq,
                        yzlx = string.IsNullOrWhiteSpace(yzxm.yzlx) ? "0" : yzxm.yzlx,
                        yzzl = string.IsNullOrWhiteSpace(yzxm.yzzl) ? "0" : yzxm.yzzl,
                        lclj_jdbm = jdbm,
                        lclj_yzbm = yzxm.yzbm,
                        ylmxxh = ylxh,
                        rymxzlxmmc = yzxm.yzmxxmmc
                    });
                    #endregion
                }
                #endregion
            }
            #endregion

            #region 药品医嘱主表及明细序号 yzzl = "999"
            List<string> listyzzl = new List<string>(); //医嘱种类
            List<string> listyzlx = new List<string>(); //医嘱类型
            foreach(var item in listypyzmx)
            {
                if (!listyzzl.Contains(item.yzzl)) { listyzzl.Add(item.yzzl); }
                if (!listyzlx.Contains(item.yzlx)) { listyzlx.Add(item.yzlx); }
            }

            for(int i = 0; i < listyzlx.Count; i++)
            {
                string yzlx = listyzlx[i];
                for (int j = 0; j < listyzzl.Count; j++)
                {
                    string yzzl = listyzzl[j];
                    if(yzzl == "zy") //中药
                    {
                        continue;
                    }
                    string ypyzxh = await _Ywxh.GetAdviceSn("药品", xtrq);
                    if (string.IsNullOrWhiteSpace(ypyzxh))
                    {
                        return _Json.GetErrMsg("生成药品医嘱序号失败");
                    }
                    listypyz.Add(new INHOSD_YPYZ_VM
                    {
                        ypyzxh = ypyzxh,
                        ksbm = ksbm,
                        zyh = zyh,
                        yzlx = yzlx,
                        ypyzfl = yzzl,
                        sfye = string.IsNullOrWhiteSpace(yebh) ? "0" : "1",
                        yebh = yebh,
                        xdys = czybm,
                        ysks = czyks,
                        ksrq = yzrq.Value,
                        sfcy = "0",
                        djsj = xtrq,
                        djry = czybm,
                        yfbm = isxyyfbm
                    });
                    List<INHOSD_YPYZMX_VM> listyzmxtem = listypyzmx.Where(it => it.yzlx == yzlx && it.yzzl == yzzl).OrderBy(it => it.ypmxxh).ToList();
                    int xh = 0;
                    foreach(var mx in listyzmxtem)
                    {
                        xh++;
                        mx.ypyzxh = ypyzxh;
                        mx.ypmxxh = xh;
                        mx.xssx = xh;
                        mx.fzxh = "yp" + ypyzxh + xh.ToString();
                    }
                }
            }
            #endregion

            #region 医疗医嘱主表及明细序号
            listyzzl = new List<string>(); //医嘱种类
            listyzlx = new List<string>(); //医嘱类型
            foreach(var item in listylyzmx)
            {
                if (!listyzzl.Contains(item.yzzl)) { listyzzl.Add(item.yzzl); }
                if (!listyzlx.Contains(item.yzlx)) { listyzlx.Add(item.yzlx); }
            }
            for (int i = 0; i < listyzlx.Count; i++)
            {
                string yzlx = listyzlx[i];
                for (int j = 0; j < listyzzl.Count; j++)
                {
                    string yzzl = listyzzl[j];
                    string ylyzxh = await _Ywxh.GetAdviceSn("医疗", xtrq);
                    if (string.IsNullOrWhiteSpace(ylyzxh))
                    {
                        return _Json.GetErrMsg("生成医疗医嘱序号失败");
                    }
                    listylyz.Add(new INHOSD_YLYZ_VM
                    {
                        ylyzxh = ylyzxh,
                        ksbm = ksbm,
                        zyh = zyh,
                        yzlx = yzlx,
                        ylyzfl = yzzl,
                        sfye = string.IsNullOrWhiteSpace(yebh) ? "0" : "1",
                        yebh = yebh,
                        xdys = czybm,
                        ysks = czyks,
                        ksrq = yzrq.Value,
                        djrq = xtrq,
                        djry = czybm
                    });
                    List<INHOSD_YLYZMX_VM> listyzmxtem = listylyzmx.Where(it => it.yzlx == yzlx && it.yzzl == yzzl).OrderBy(it => it.ylmxxh).ToList();
                    int xh = 0;
                    foreach (var mx in listyzmxtem)
                    {
                        xh++;
                        mx.ylyzxh = ylyzxh;
                        mx.ylmxxh = xh;
                        mx.xssx = xh;
                        mx.fzxh = "yl" + ylyzxh + xh.ToString();
                    }
                }
            }
            #endregion

            #region 皮试医嘱
            for(int i = 0; i < listypyzmx.Count; i++)
            {
                var mx = listypyzmx[i];
                if(mx.psypbz != "1")
                {
                    continue;
                }
                string ylyzxh = await _Ywxh.GetAdviceSn("医疗", xtrq);
                if (string.IsNullOrWhiteSpace(ylyzxh))
                {
                    return _Json.GetErrMsg("生成医疗医嘱序号失败");
                }
                listylyz.Add(new INHOSD_YLYZ_VM
                {
                    ylyzxh = ylyzxh,
                    ksbm = ksbm,
                    zyh = zyh,
                    yzlx = "0",
                    ylyzfl = "0",
                    sfye = string.IsNullOrWhiteSpace(yebh) ? "0" : "1",
                    yebh = yebh,
                    xdys = czybm,
                    ysks = czyks,
                    ksrq = yzrq.Value,
                    djrq = xtrq,
                    djry = czybm
                });
                listylyzmx.Add(new INHOSD_YLYZMX_VM
                {
                    ylyzxh = ylyzxh,
                    ylmxxh = 1,
                    mxzlxmbm = mx.ryypmc + "皮试",
                    ypyzxh = mx.ypyzxh,
                    ypmxxh = mx.ypmxxh,
                    fzh = 0,
                    sl = 1,
                    shbz = "0",
                    ystzbz = "0",
                    hstzbz = "0",
                    tsyz = "1",
                    xssx = 1,
                    qmsj = xtrq,
                    fzxh = "yl" + ylyzxh + "1"
                });
            }
            #endregion

            #region 自动停用长期医嘱
            if(iszdtycqyz == "1")
            {
                listypyzmx_tz = await _Repository.GetListPatientDrugAdvice(zyh);
                foreach(var vm in listypyzmx_tz)
                {
                    vm.ystzbz = vm.shbz == "1" ? "1" : "2";
                    vm.tzsm = vm.shbz == "1" ? "执行路径自动停嘱" : "执行路径自动作废";
                    vm.tzys = czybm;
                    vm.ystzsj = xtrq;
                }
                listylyzmx_tz = await _Repository.GetListPatientMedicalAdvice(zyh);
                foreach (var vm in listylyzmx_tz)
                {
                    vm.ystzbz = vm.shbz == "1" ? "1" : "2";
                    vm.tzsm = vm.shbz == "1" ? "执行路径自动停嘱" : "执行路径自动作废";
                    vm.tzys = czybm;
                    vm.ystzsj = xtrq;
                }
            }
            #endregion

            DbResult<bool> result = await _Repository.CPPreExecution(listljxmzjjl, listyzzxjl, listylyz, listylyzmx, listypyz, listypyzmx, listypyzmx_tz, listylyzmx_tz);
            if(result.IsSuccess )
            {
                return _Json.GetResults("路径执行成功！" + _Com.ListToStr(listerr.Select(it => it.msg).ToList(), ','));
            }
            else
            {
                return _Json.GetErrMsg("路径执行失败！" + result.ErrorMessage);
            }
        }
        #region 病人表单查询
        /// <summary>
        /// 查询病人表单信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetPatientForm(JsonObject data)
        {
            string zyh = _Json.InString(data["zyh"]);
            if (string.IsNullOrWhiteSpace(zyh))
            {
                return _Json.GetErrMsg("住院号为空，不能查询！");
            }
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径为空，不能查询！");
            }
            //阶段列表
            var listljjd = await _Public.GetListPhaseForCPCode(ljbm);
            //项目列表
            var listljxm = await _Public.GetListCpProject(ljbm, zyh);
            //执行记录
            var listzxjl = await _Public.GetListPatientCpExecutionRecord(ljbm, zyh);
            //评估结果
            var listpgjl = await _Public.GetPatientCPEvaluationRecord(ljbm, zyh);

            var res = new
            {
                ljjd = listljjd,
                ljxm = listljxm,
                zxjl = listzxjl,
                pgjl = listpgjl
            };
            return _Json.GetResults(res);
        }
        #endregion
        #endregion

        #region 护士执行

        //病人列表 同GetListCPPatient

        //路径阶段 同GetListPhaseForCPCode

        /// <summary>
        /// 根据住院号和路径编码获取路径阶段
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetListCpProjectForPatientAndCpCode(JsonObject data)
        {
            string zyh = _Json.InString(data["zyh"]);
            if(string.IsNullOrWhiteSpace(zyh))
            {
                return _Json.GetErrMsg("住院号为空，不能查询！");
            }
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径为空，不能查询！");
            }

            return _Json.GetResults(await _Repository.GetListPhaseForCPCode(zyh, ljbm));
        }
        /// <summary>
        /// 查询病人路径护理执行信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetNurseCPExecutionRecordDetails(JsonObject data)
        {
            string zyh = _Json.InString(data["zyh"]);
            if (string.IsNullOrWhiteSpace(zyh))
            {
                return _Json.GetErrMsg("住院号为空，不能查询！");
            }
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径为空，不能查询！");
            }
            string jdbm = _Json.InString(data["jdbm"]);
            if (string.IsNullOrWhiteSpace(jdbm))
            {
                return _Json.GetErrMsg("阶段为空，不能查询！");
            }

            string filter = _Json.InString(data["filterstr"]);
            int page = _Json.InInt16(data["page"]);
            int limit = _Json.InInt16(data["limit"]);

            RefAsync<int> total = 0;
            var list = await _Repository.GetNurseCPExecutionRecordDetails(zyh,ljbm,jdbm,page,limit,filter,total);

            return _Json.GetResults(list);
        }
        /// <summary>
        /// 获取护士阶段评估记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetSingleNurseCpEvaluate(JsonObject data)
        {
            string zyh = _Json.InString(data["zyh"]);
            if (string.IsNullOrWhiteSpace(zyh))
            {
                return _Json.GetErrMsg("住院号为空，不能查询！");
            }
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径为空，不能查询！");
            }
            string jdbm = _Json.InString(data["jdbm"]);
            if (string.IsNullOrWhiteSpace(jdbm))
            {
                return _Json.GetErrMsg("阶段为空，不能查询！");
            }
            return _Json.GetResults(await _Repository.GetSingleNurseCpEvaluate(zyh,ljbm,jdbm));
        }
        /// <summary>
        /// 护理路径执行
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> NurseCPPreExecution(JsonObject data)
        {
            string czybm = _Com.GetUserIdByToken();
            if (string.IsNullOrWhiteSpace(czybm))
            {
                return _Json.GetErrMsg("当前操作员为空，不能查询！");
            }
            string zyh = _Json.InString(data["zyh"]);
            if (string.IsNullOrWhiteSpace(zyh))
            {
                return _Json.GetErrMsg("住院号为空，不能查询！");
            }
            string ljbm = _Json.InString(data["ljbm"]);
            if (string.IsNullOrWhiteSpace(ljbm))
            {
                return _Json.GetErrMsg("路径为空，不能查询！");
            }
            string jdbm = _Json.InString(data["jdbm"]);
            if (string.IsNullOrWhiteSpace(jdbm))
            {
                return _Json.GetErrMsg("阶段为空，不能查询！");
            }

            StringBuilder msg = new StringBuilder();
            CP_PGJL_HL_VM pgjl = _Json.InEntities<CP_PGJL_HL_VM>(data["pgjl"],msg);
            if(pgjl == null)
            {
                return _Json.GetErrMsg("护理评估信息为空，不能保存！" + msg.ToString());
            }

            msg.Clear();
            List<CP_ZXJL_HL_VM> listzxjl = _Json.InList<CP_ZXJL_HL_VM>(data["zxxm"],msg);
            if (listzxjl == null || listzxjl.Count <= 0)
            {
                return _Json.GetErrMsg("护理执行记录信息为空，不能保存！" + msg.ToString());
            }

            if (string.IsNullOrWhiteSpace(pgjl.pgzt))
            {
                return _Json.GetErrMsg("请选择评估状态");
            }

            DateTime xtrq = _Sys.GetXtrq();
            pgjl.zyh = zyh;
            pgjl.ljbm = ljbm;
            pgjl.jdbm = jdbm;
            pgjl.djrq = xtrq;
            pgjl.czybm = czybm;

            var listljxm = await _Sys.GetEntityList<CP_LJXM_VM>();
            var listzxjlsave = new List<CP_ZXJL_HL_VM>();
            for (int i = 0; i < listzxjl.Count; i++)
            {
                var vm = listzxjl[i];
                if (!string.IsNullOrWhiteSpace(vm.id))
                {
                    continue;
                }
                if (string.IsNullOrWhiteSpace(vm.xmbm))
                {
                    return _Json.GetErrMsg($"第{i+1}行项目编码为空");
                }
                var ljxm = listljxm.Find(it => it.xmbm == vm.xmbm);
                if(ljxm == null)
                {
                    return _Json.GetErrMsg($"路径项目编码：{vm.xmbm}无效！");
                }
                vm.xmnr = ljxm.xmnr;
                vm.zxzt = "1";
                vm.zxry = czybm;
                vm.xmjdpg = "1";
                vm.zxjg = "1";
                vm.zxqm = czybm;
                vm.zxrq = xtrq;
                listzxjlsave.Add(vm);
            }

            return _Json.GetResults(await _Repository.NurseCPPreExecution(pgjl, _Json.ToJsonKeyArr(data["pgjl"]), listzxjlsave));
        }
        /// <summary>
        /// 查询病人表单信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetPatientNurseForm(JsonObject data)
        {
            string zyh = _Json.InString(data["zyh"]);
            if (string.IsNullOrWhiteSpace(zyh))
            {
                return _Json.GetErrMsg("住院号为空，不能查询！");
            }
            CP_DJJL_VM djjl = await _Repository.GetEntity<CP_DJJL_VM>(a => a.zyh == zyh && a.ztbz != "1");
            if (djjl == null)
            {
                return _Json.GetErrMsg($"未查询到住院号：{zyh}的路径登记信息！");
            }

            //阶段列表
            var listljjd = await _Public.GetListPhaseForCPCode(djjl.ljbm);
            //项目列表
            var listljxm = await _Public.GetListCpProject(djjl.ljbm, zyh);
            //执行记录
            var listzxjl = await _Public.GetListNurseCpProject(djjl.ljbm, zyh);
            //评估结果
            var listpgjl = await _Public.GetPatientNurseCPEvaluationRecord(djjl.ljbm, zyh);

            var res = new
            {
                ljjd = listljjd,
                ljxm = listljxm,
                zxjl = listzxjl,
                pgjl = listpgjl
            };
            return _Json.GetResults(res);
        }
        #endregion
    }
}
