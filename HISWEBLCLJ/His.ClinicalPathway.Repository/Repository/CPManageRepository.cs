using His.Core;
using His.DAL;
using His.Entities;
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace His.ClinicalPathway.Repository
{
    /// <summary>
    /// 医生路径管理
    /// </summary>
    public class CPManageRepository : HisBaseService, ICPManageRepository
    {
        private readonly IPublicDddwRepository _Dddw;
        public CPManageRepository(IPublicDddwRepository IDddw)
        {
            _Dddw = IDddw;
        }
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
        public async Task<object> GetListCPNameForDeptIcd(string ksbm, string jbbm, int page, int limit, string filter, RefAsync<int> total)
        {
            var q = Db.Queryable<CP_LJMC_VM>()
                        .OrderBy(mc => mc.xssx)
                        .WhereIF(!string.IsNullOrWhiteSpace(filter), mc => mc.ljbm.Contains(filter) || mc.ljmc.Contains(filter) || mc.pydm.ToUpper().Contains(filter.ToUpper()))
                        .Where(mc => mc.tybz == "0")
                        .Where(mc => SqlFunc.Subqueryable<CP_KSLJ_VM>().Where(it => it.ksbm == ksbm).Where(it => it.ljbm == mc.ljbm).Any())
                        .Where(mc => SqlFunc.Subqueryable<CP_LJICD_VM>().Where(it => it.jbbm == jbbm).Where(it => it.ljbm == mc.ljbm).Any())
                        .Select(mc => new
                        {
                            ljbm = mc.ljbm,
                            ljmc = mc.ljmc,
                            pydm = mc.pydm,
                            bzms = mc.bzms,
                            fl = mc.fl,
                            flmc = SqlFunc.IIF(mc.fl == "0", "卫生部", SqlFunc.IIF(mc.fl == "1", "院内", "")),
                            blfx = mc.blfx,
                            blfxmc = SqlFunc.IIF(mc.blfx == "1", "单纯普通型", SqlFunc.IIF(mc.blfx == "2", "单纯急症型", SqlFunc.IIF(mc.blfx == "3", "复杂疑难型", SqlFunc.IIF(mc.blfx == "4", "复杂危重型", "")))),
                            sybq = mc.sybq,
                            sybqmc = SqlFunc.IIF(mc.blfx == "0", "不区分", SqlFunc.IIF(mc.blfx == "1", "危", SqlFunc.IIF(mc.blfx == "2", "重", SqlFunc.IIF(mc.blfx == "3", "一般", "")))),
                            syxb = mc.syxb,
                            syxbmc = SqlFunc.IIF(mc.blfx == "0", "不区分", SqlFunc.IIF(mc.blfx == "1", "男", SqlFunc.IIF(mc.blfx == "2", "女", ""))),
                            syks = mc.syks,
                            syksmc = SqlFunc.IIF(mc.fl == "1", "是", "否"),
                            bzts = mc.bzts,
                            tybz = mc.tybz,
                            cjczy = mc.cjczy,
                            cjczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == mc.cjczy).Select(it => it.czyxm),
                            cjrq = mc.cjrq,
                            shbz = mc.shbz,
                            shczy = mc.shczy,
                            shczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == mc.shczy).Select(it => it.czyxm),
                            shrq = mc.shrq,
                            sydx = mc.sydx,
                            bzzyr = mc.bzzyr,
                            ckfy_from = mc.ckfy_from,
                            ckfy_to = mc.ckfy_to,
                            bddyls = mc.bddyls,
                            rjxdsj = mc.rjxdsj,
                            xssx = mc.xssx,
                            qysj = mc.qysj,
                            ls_ljbm = mc.ls_ljbm,
                            ls_yljbm = mc.ls_yljbm,
                            yyzbxtptbm = mc.yyzbxtptbm,
                            yyzbxtptmc = mc.yyzbxtptmc
                        });
            return await base.GetListForQueryable(q, page, limit, total);
        }
        /// <summary>
        /// 判断科室、疾病是否有对应的路径名称
        /// </summary>
        /// <param name="ksbm"></param>
        /// <param name="jbbm"></param>
        /// <returns></returns>
        public async Task<int> JudgeCPNameForDeptIcd(string ksbm, string jbbm)
        {
            return await Db.Queryable<CP_LJMC_VM>()
                        .Where(mc => mc.tybz == "0")
                        .Where(mc => SqlFunc.Subqueryable<CP_KSLJ_VM>().Where(it => it.ksbm == ksbm).Where(it => it.ljbm == mc.ljbm).Any())
                        .Where(mc => SqlFunc.Subqueryable<CP_LJICD_VM>().Where(it => it.jbbm == jbbm).Where(it => it.ljbm == mc.ljbm).Any())
                        .CountAsync();
        }
        /// <summary>
        /// 判断科室、疾病与路径是否匹配
        /// </summary>
        /// <param name="ksbm"></param>
        /// <param name="jbbm"></param>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        public async Task<int> JudgeCPNameForDeptIcd(string ksbm, string jbbm, string ljbm)
        {
            return await Db.Queryable<CP_LJMC_VM>()
                        .Where(mc => mc.tybz == "0")
                        .Where(mc => mc.ljbm == ljbm)
                        .Where(mc => SqlFunc.Subqueryable<CP_KSLJ_VM>().Where(it => it.ksbm == ksbm).Where(it => it.ljbm == mc.ljbm).Any())
                        .Where(mc => SqlFunc.Subqueryable<CP_LJICD_VM>().Where(it => it.jbbm == jbbm).Where(it => it.ljbm == mc.ljbm).Any())
                        .CountAsync();
        }
        /// <summary>
        /// 获取科室、疾病对应的路径名称
        /// </summary>
        /// <param name="ksbm"></param>
        /// <param name="jbbm"></param>
        /// <returns></returns>
        public async Task<List<CP_LJMC_VM>> GetListCPNameForDeptIcd(string ksbm, string jbbm)
        {
            return await Db.Queryable<CP_LJMC_VM>()
                .Where(mc => mc.tybz == "0")
                .Where(mc => SqlFunc.Subqueryable<CP_KSLJ_VM>().Where(it => it.ksbm == ksbm).Where(it => it.ljbm == mc.ljbm).Any())
                .Where(mc => SqlFunc.Subqueryable<CP_LJICD_VM>().Where(it => it.jbbm == jbbm).Where(it => it.ljbm == mc.ljbm).Any())
                .ToListAsync();

        }
        /// <summary>
        /// 查询路径对应的ICD编码列表
        /// </summary>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        public async Task<List<string>> GetListCpIcd(string ljbm)
        {
            return await Db.Queryable<CP_LJICD_VM>()
                .Where(ljicd => ljicd.ljbm == ljbm)
                .Select(ljicd => ljicd.jbbm).ToListAsync();
        }
        /// <summary>
        /// 入径登记保存
        /// </summary>
        /// <param name="djjl"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> SaveCPEntry(CP_DJJL_VM djjl)
        {
            return await base.AddEntity(djjl);
        }
        /// <summary>
        /// 更新登记信息
        /// </summary>
        /// <param name="djjl"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> UpdateCPEntry(CP_DJJL_VM djjl, string[] col)
        {
            return await base.UpdateEntity(djjl, col);
        }
        /// <summary>
        /// 保存退出路径申请
        /// </summary>
        /// <param name="qxjl"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> SaveCPCancelApply(CP_QXSQJL_VM qxjl)
        {
            return await base.AddEntity(qxjl);
        }
        #endregion

        #region 路径执行
        /// <summary>
        /// 查询科室入径病人
        /// </summary>
        /// <param name="ksbm"></param>
        /// <param name="cybz">出院标志 0-在院 1-出院</param>
        /// <param name="ztbz">状态标志 0=正常在院 1=取消 2=变异 3=正常结束</param>
        /// <param name="rq1">查询开始日期</param>
        /// <param name="rq2">查询结束日期</param>
        /// <param name="gcbr">管床病人 1-只查管床病人</param>
        /// <param name="ysbm">医生编码</param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public async Task<object> GetListCPPatient(string ksbm, string cybz, string ztbz, string gcbr, string ysbm, DateTime? rq1, DateTime? rq2, int page, int limit, string filter, RefAsync<int> total)
        {
            var q = Db.Queryable<CP_DJJL_VM, INHOS_RYDJ_VM, PATIENT_ZCXX_VM, INHOS_CWH_VM>((djjl, rydj, zcxx, cwh) => new JoinQueryInfos(
                JoinType.Inner, djjl.zyh == rydj.zyh,
                JoinType.Inner, rydj.brid == zcxx.brid,
                JoinType.Inner, rydj.rycwid == cwh.cwid))
                .Where((djjl, rydj, zcxx, cwh) => djjl.ryks == ksbm)
                .WhereIF(!string.IsNullOrWhiteSpace(ztbz), (djjl, rydj, zcxx, cwh) => rydj.zyzt == ztbz)
                .WhereIF(!string.IsNullOrWhiteSpace(cybz), (djjl, rydj, zcxx, cwh) => rydj.bqcybz == cybz)
                .WhereIF(gcbr == "1", (djjl, rydj, zcxx, cwh) => rydj.zyys == ysbm)
                .WhereIF(cybz == "1", (djjl, rydj, zcxx, cwh) => rydj.bqcybz == "1" && rydj.bqcyrq >= rq1.Value && rydj.bqcyrq < rq2.Value)
                .WhereIF(cybz != "1", (djjl, rydj, zcxx, cwh) => rydj.bqcybz == "0")
                .Where((djjl, rydj, zcxx, cwh) => rydj.ifzf == "0")
                .WhereIF(!string.IsNullOrWhiteSpace(filter), (djjl, rydj, zcxx, cwh) => rydj.zyh.Contains(filter) || rydj.rybrxm.Contains(filter) || zcxx.pydm.ToUpper().Contains(filter.ToUpper()))
                .OrderBy((djjl, rydj, zcxx, cwh) => new
                {
                    cwh.xssx
                })
                .Select((djjl, rydj, zcxx, cwh) => new
                {
                    rydj.zyh,
                    rydj.rybrxm,
                    rydj.bxlbbm,
                    rydj.rybxlbmc,
                    rydj.fbbm,
                    rydj.ryfbmc,
                    rydj.brid,
                    rydj.ryrq,
                    rydj.rycwid,
                    rydj.cwh,
                    rydj.zyys,
                    rydj.zyysxm,
                    rydj.brnl,
                    rydj.brnldw,
                    brnldwmc = SqlFunc.IIF(rydj.brnldw == "1", "岁", SqlFunc.IIF(rydj.brnldw == "2", "月", SqlFunc.IIF(rydj.brnldw == "3", "天", SqlFunc.IIF(rydj.brnldw == "4", "小时", SqlFunc.IIF(rydj.brnldw == "5", "分钟", ""))))),
                    rydj.gchs,
                    rydj.gchsxm,
                    rydj.ryhyzkbm,
                    hyzkmc = SqlFunc.Subqueryable<PU_SJZD_VM>().Where(it => it.xmlb == "婚姻状况" && it.xmbm == rydj.ryhyzkbm).Select(it => it.xmmc),
                    rydj.ryzybm,
                    zymc = SqlFunc.Subqueryable<PU_SJZD_VM>().Where(it => it.xmlb == "职业编码" && it.xmbm == rydj.ryzybm).Select(it => it.xmmc),
                    rydj.rybrxb,
                    xbmc = SqlFunc.Subqueryable<PU_SJZD_VM>().Where(it => it.xmlb == "性别代码" && it.xmbm == rydj.rybrxb).Select(it => it.xmmc),
                    rydj.rysj,
                    rydj.zyks,
                    rydj.zyksmc,
                    rydj.bqcybz,
                    djjl.ljbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == djjl.ljbm).Select(it => it.ljmc),
                    djjl.ztbz,
                    djjl.djczy,
                    djczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == djjl.djczy).Select(it => it.czyxm),
                    djjl.djrq,
                    djjl.tcczy,
                    tcczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == djjl.tcczy).Select(it => it.czyxm),
                    djjl.tcrq,
                    djjl.byyy,
                    djjl.id,
                    djjl.jbbm,
                    jbmc = SqlFunc.Subqueryable<PU_JBBM_VM>().Where(it => it.jbbm == djjl.jbbm).Select(it => it.jbmc),
                    djjl.rjrq
                });
            return await base.GetListForQueryable(q, page, limit, total);
        }

        /// <summary>
        /// 病人执行路径阶段记录
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        public async Task<object> GetListPatientCPPhase(string zyh, string ljbm)
        {
            return await Db.Queryable<CP_LJJD_VM, CP_PGJL_VM>((ljjd, pgjl) => new JoinQueryInfos(
                JoinType.Left, ljjd.jdbm == pgjl.jdbm && pgjl.zyh == zyh && pgjl.ljbm == ljbm))
                .Where(ljjd => ljjd.ljbm == ljbm)
                .Select((ljjd, pgjl) => new
                {
                    jdbm = ljjd.jdbm,
                    jdmc = SqlFunc.IIF(string.IsNullOrWhiteSpace(pgjl.id), "", "(√)") + ljjd.jdmc,
                    zxbz = SqlFunc.IIF(string.IsNullOrWhiteSpace(pgjl.id), "0", "1")
                }).ToListAsync();
        }
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
        public async Task<object> GetCPExecutionRecordDetails(string zyh, string ljbm, string jdbm, int page, int limit, string filter, RefAsync<int> total)
        {
            var q = Db.Queryable<CP_LJXM_VM, CP_LJXM_ZXJL_VM>((ljxm, zxjl) => new JoinQueryInfos(
                JoinType.Left, ljxm.xmbm == zxjl.xmbm && zxjl.jdbm == ljxm.jdbm && zxjl.ljbm == ljxm.ljbm && zxjl.zyh == zyh))
                .Where((ljxm, zxjl) => ljxm.ljbm == ljbm)
                .Where((ljxm, zxjl) => ljxm.jdbm == jdbm)
                .Select((ljxm, zxjl) => new
                {
                    ljxm.xmbm,
                    ljxm.ljbm,
                    ljxm.jdbm,
                    ljxm.xmdl,
                    ljxm.xmlx,
                    ljxm.xsxh,
                    ljxm.xmnr,
                    ljxm.xmxz,
                    ljxm.xmzxfs,
                    ljxm.xmzxr,
                    ljxm.xmjdpg,
                    ljxm.zxdlbh,
                    ljxm.zxxlbh,
                    ljxm.kxx,
                    zxjl.zxjg,
                    zxdlmc = SqlFunc.Subqueryable<CP_ZXDL_VM>().Where(it => it.dlbm == ljxm.zxdlbh).Select(it => it.dlmc),
                    xmzxfsmc = SqlFunc.IIF(ljxm.xmzxfs == "1", "无需执行", SqlFunc.IIF(ljxm.xmzxfs == "2", "每天执行", SqlFunc.IIF(ljxm.xmzxfs == "3", "至少执行一次", SqlFunc.IIF(ljxm.xmzxfs == "4", "必要时执行", "")))) 
                });
            return await base.GetListForQueryable(q, page, limit, total);
        }
        /// <summary>
        /// 获取病人路径阶段评估记录
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="jdbm"></param>
        /// <returns></returns>
        public async Task<object> GetCPEvaluationRecord(string zyh, string jdbm)
        {
            return await Db.Queryable<CP_PGJL_VM>()
                .Where(pgjl => pgjl.zyh == zyh)
                .Where(pgjl => pgjl.jdbm == jdbm)
                .Select(pgjl => new
                {
                    pgjl.id,
                    pgjl.zyh,
                    pgjl.zyts,
                    pgjl.pgzt,
                    ztsm = SqlFunc.IIF(pgjl.pgzt == "1", "变异继续", SqlFunc.IIF(pgjl.pgzt == "2", "变异退出", SqlFunc.IIF(pgjl.pgzt == "3", "正常出院", "正常"))),
                    pgjl.pgczy,
                    czyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == pgjl.pgczy).Select(it => it.czyxm),
                    pgjl.pgjg,
                    pgjl.jdbm,
                    jdmc = SqlFunc.Subqueryable<CP_LJJD_VM>().Where(it => it.jdbm == pgjl.jdbm).Select(it => it.jdmc),
                    pgjl.ljbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == pgjl.ljbm).Select(it => it.ljmc)
                }).FirstAsync();
        }
        /// <summary>
        /// 查询病人评估记录
        /// </summary>
        /// <param name="zyh"></param>
        /// <returns></returns>
        public async Task<List<CP_PGJL_VM>> GetListPatientCPEvaluation(string zyh)
        {
            return await Db.Queryable<CP_PGJL_VM>()
                .Where(pgjl => pgjl.zyh == zyh)
                .ToListAsync();
        }
        /// <summary>
        /// 保存评估
        /// </summary>
        /// <param name="pgjl"></param>
        /// <param name="djjl"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> SaveCPEvaluation(CP_PGJL_VM pgjl, CP_DJJL_VM djjl, string[] col)
        {
            return await Db.UseTranAsync(async () =>
            {
                await Db.Insertable(pgjl).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
                await Db.Updateable(djjl).UpdateColumns(col).ExecuteCommandAsync();
            });
        }
        /// <summary>
        /// 删除评估
        /// </summary>
        /// <param name="pgjl"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> DelCPEvaluation(CP_PGJL_VM pgjl)
        {
            return await base.DeleteEntity(pgjl);
        }
        /// <summary>
        /// 查询路径和阶段对应的治疗方案列表
        /// </summary>
        /// <param name="ljbm"></param>
        /// <param name="jdbm"></param>
        /// <returns></returns>
        public async Task<object> GetListCPTherapeuticScheduleForCpcodePhase(string ljbm, string jdbm)
        {
            return await Db.Queryable<CP_ZLFA_VM>()
                .Where(zlfa => zlfa.ljbm == ljbm)
                .Where(zlfa => zlfa.jdbm == jdbm)
                .ToListAsync();
        }
        /// <summary>
        /// 查询指定的医嘱列表 路径执行时用
        /// </summary>
        /// <param name="listyzbm">医嘱编码列表</param>
        /// <returns></returns>
        public async Task<List<CPAdviceDetail_Model>> GetListCpMedicalAdviceExecution(List<string> listyzbm)
        {
            var q_yp = Db.Queryable<CP_YZXM_VM, YK_YPZD_VM, CP_LJXM_VM>((yzxm, ypzd, ljxm) => new JoinQueryInfos(
                JoinType.Inner, yzxm.yzmxxmbm == ypzd.ypbm,
                JoinType.Inner, yzxm.ljxmbm == ljxm.xmbm))
                .Where((yzxm, ypzd, ljxm) => yzxm.yzpd == "1")
                .Where((yzxm, ypzd, ljxm) => listyzbm.Contains(yzxm.yzbm))
                .Select((yzxm, ypzd, ljxm) => new CPAdviceDetail_Model
                {
                    yzbm = yzxm.yzbm,
                    yzlx = yzxm.yzlx,
                    yzzl = yzxm.yzzl,
                    yzpd = yzxm.yzpd,
                    xsxh = yzxm.xsxh,
                    yzmxxmbm = yzxm.yzmxxmbm,
                    yzfzh = yzxm.yzfzh,
                    dcsl = yzxm.dcsl,
                    yyff = yzxm.yyff,
                    dcjl = yzxm.dcjl,
                    jldw = yzxm.jldw,
                    pcbm = yzxm.pcbm,
                    zl = yzxm.zl,
                    yssm = yzxm.yssm,
                    sysd = yzxm.sysd,
                    sydw = yzxm.sydw,
                    zxks = yzxm.zxks,
                    yfbm = yzxm.yfbm,
                    ljxmbm = yzxm.ljxmbm,
                    cjczy = yzxm.cjczy,
                    cjrq = yzxm.cjrq,
                    ljbm = yzxm.ljbm,
                    jdbm = yzxm.jdbm,
                    bxxm = yzxm.bxxm,
                    zlfa = yzxm.zlfa,
                    sfcy = yzxm.sfcy,
                    fjsl = yzxm.fjsl,
                    fjmc = yzxm.fjmc,
                    zcyyf = yzxm.zcyyf,
                    cflx = yzxm.cflx,
                    ls_ljbm = yzxm.ls_ljbm,
                    ls_jdbm = yzxm.ls_jdbm,
                    ls_xmbm = yzxm.ls_xmbm,
                    ls_yzbm = yzxm.ls_yzbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljbm).Select(it => it.ljmc),
                    jdmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.jdbm).Select(it => it.ljmc),
                    ljxmmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljxmbm).Select(it => it.ljmc),
                    zlfamc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.zlfa).Select(it => it.ljmc),
                    yzlxmc = "",
                    yzzlmc = "",
                    yzpdmc = "",
                    yzmxxmmc = ypzd.ypmc,
                    bxxmmc = "",
                    jldwmc = SqlFunc.Subqueryable<YK_JLDWBM_VM>().Where(it => it.jldwid == yzxm.jldw).Select(it => it.jldwmc),
                    pcmc = SqlFunc.Subqueryable<PU_PC_VM>().Where(it => it.pcbm == yzxm.pcbm).Select(it => it.pcmc),
                    yyffmc = SqlFunc.Subqueryable<PU_GYTJ_VM>().Where(it => it.tjbm == yzxm.yyff).Select(it => it.tjmc),
                    sydwmc = "",
                    cjczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == yzxm.cjczy).Select(it => it.czyxm),
                    cflxmc = SqlFunc.Subqueryable<YF_CFLX_VM>().Where(it => it.cflxbm == yzxm.cflx).Select(it => it.cflxmc),
                    gg = ypzd.ypgg,
                    jbjl = ypzd.jbjl,
                    jldw_ypzd = ypzd.jldw,
                    psypbz = yzxm.psypbz,
                    tclbid = ypzd.tclbid,
                    hldj = "",
                    hldjmc = "",
                    xsxh_ljxm = ljxm.xsxh,
                    ylyzfl = "",
                    ylyzflmc = "",
                    xmxz = ljxm.xmxz,
                    xmnr = ljxm.xmnr,
                    xmzxfs = ljxm.xmzxfs,
                    xmzxfsmc = "",
                    ypzlbm = ypzd.ypzlbm,
                    ypzlmc = SqlFunc.Subqueryable<YK_YPZL_VM>().Where(it => it.ypzlbm == ypzd.ypzlbm).Select(it => it.ypzlmc),
                    ypjxid = ypzd.ypjxid,
                    ypjxmc = SqlFunc.Subqueryable<YK_JXBM_VM>().Where(it => it.ypjxid == ypzd.ypjxid).Select(it => it.jxmc),
                    lsxh = 0
                })
                .Mapper(async yzxm =>
                {
                    yzxm.yzlxmc = await _Dddw.GetNameForCode("zyyzlx", yzxm.yzlx);
                    yzxm.yzzlmc = await _Dddw.GetNameForCode("yzzl_cq", yzxm.yzzl);
                    yzxm.yzpdmc = await _Dddw.GetNameForCode("CP_Yzpd", yzxm.yzpd);
                    yzxm.bxxmmc = await _Dddw.GetNameForCode("CP_Bxxm", yzxm.bxxm);
                    yzxm.sydwmc = await _Dddw.GetNameForCode("sydw", yzxm.sydw);
                    yzxm.xmzxfsmc = await _Dddw.GetNameForCode("CP_Xmxzfs", yzxm.xmzxfs);
                });
            var q_yl = Db.Queryable<CP_YZXM_VM, PU_MXZLXM_VM, PU_ZLXL_VM, PU_ZLDL_VM, CP_LJXM_VM>((yzxm, zlxm, zlxl, zldl, ljxm) => new JoinQueryInfos(
                JoinType.Inner, yzxm.yzmxxmbm == zlxm.mxzlxmbm,
                JoinType.Inner, zlxm.zlxlbm == zlxl.zlxlbm,
                JoinType.Inner, zlxl.zldlbm == zldl.zldlbm,
                JoinType.Inner, yzxm.ljxmbm == ljxm.xmbm))
                .Where((yzxm, zlxm, zlxl, zldl, ljxm) => yzxm.yzpd == "0")
                .Where((yzxm, zlxm, zlxl, zldl, ljxm) => listyzbm.Contains(yzxm.yzbm))
                .Select((yzxm, zlxm, zlxl, zldl, ljxm) => new CPAdviceDetail_Model
                {
                    yzbm = yzxm.yzbm,
                    yzlx = yzxm.yzlx,
                    yzzl = yzxm.yzzl,
                    yzpd = yzxm.yzpd,
                    xsxh = yzxm.xsxh,
                    yzmxxmbm = yzxm.yzmxxmbm,
                    yzfzh = yzxm.yzfzh,
                    dcsl = yzxm.dcsl,
                    yyff = yzxm.yyff,
                    dcjl = yzxm.dcjl,
                    jldw = yzxm.jldw,
                    pcbm = yzxm.pcbm,
                    zl = yzxm.zl,
                    yssm = yzxm.yssm,
                    sysd = yzxm.sysd,
                    sydw = yzxm.sydw,
                    zxks = yzxm.zxks,
                    yfbm = yzxm.yfbm,
                    ljxmbm = yzxm.ljxmbm,
                    cjczy = yzxm.cjczy,
                    cjrq = yzxm.cjrq,
                    ljbm = yzxm.ljbm,
                    jdbm = yzxm.jdbm,
                    bxxm = yzxm.bxxm,
                    zlfa = yzxm.zlfa,
                    sfcy = yzxm.sfcy,
                    fjsl = yzxm.fjsl,
                    fjmc = yzxm.fjmc,
                    zcyyf = yzxm.zcyyf,
                    cflx = yzxm.cflx,
                    ls_ljbm = yzxm.ls_ljbm,
                    ls_jdbm = yzxm.ls_jdbm,
                    ls_xmbm = yzxm.ls_xmbm,
                    ls_yzbm = yzxm.ls_yzbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljbm).Select(it => it.ljmc),
                    jdmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.jdbm).Select(it => it.ljmc),
                    ljxmmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljxmbm).Select(it => it.ljmc),
                    zlfamc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.zlfa).Select(it => it.ljmc),
                    yzlxmc = "",
                    yzzlmc = "",
                    yzpdmc = "",
                    yzmxxmmc = zlxm.mxzlxmmc,
                    bxxmmc = "",
                    jldwmc = SqlFunc.Subqueryable<YK_JLDWBM_VM>().Where(it => it.jldwid == yzxm.jldw).Select(it => it.jldwmc),
                    pcmc = SqlFunc.Subqueryable<PU_PC_VM>().Where(it => it.pcbm == yzxm.pcbm).Select(it => it.pcmc),
                    yyffmc = SqlFunc.Subqueryable<PU_GYTJ_VM>().Where(it => it.tjbm == yzxm.yyff).Select(it => it.tjmc),
                    sydwmc = "",
                    cjczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == yzxm.cjczy).Select(it => it.czyxm),
                    cflxmc = SqlFunc.Subqueryable<YF_CFLX_VM>().Where(it => it.cflxbm == yzxm.cflx).Select(it => it.cflxmc),
                    gg = "",
                    jbjl = 0,
                    jldw_ypzd = "",
                    psypbz = "",
                    tclbid = "",
                    hldj = zlxm.hldj,
                    hldjmc = "",
                    xsxh_ljxm = ljxm.xsxh,
                    ylyzfl = "",
                    ylyzflmc = "",
                    xmxz = ljxm.xmxz,
                    xmnr = ljxm.xmnr,
                    xmzxfs = ljxm.xmzxfs,
                    xmzxfsmc = "",
                    ypzlbm = "",
                    ypzlmc = "",
                    ypjxid = "",
                    ypjxmc = "",
                    lsxh = 0
                })
                .Mapper(async yzxm =>
                {
                    yzxm.yzlxmc = await _Dddw.GetNameForCode("zyyzlx", yzxm.yzlx);
                    yzxm.yzzlmc = await _Dddw.GetNameForCode("yzzl_cq", yzxm.yzzl);
                    yzxm.yzpdmc = await _Dddw.GetNameForCode("CP_Yzpd", yzxm.yzpd);
                    yzxm.bxxmmc = await _Dddw.GetNameForCode("CP_Bxxm", yzxm.bxxm);
                    yzxm.hldjmc = await _Dddw.GetNameForCode("hldj", yzxm.hldj);
                    yzxm.ylyzflmc = await _Dddw.GetNameForCode("zlyzfl", yzxm.ylyzfl);
                    yzxm.xmzxfsmc = await _Dddw.GetNameForCode("CP_Xmxzfs", yzxm.xmzxfs);
                });
            var q = Db.Queryable<CP_YZXM_VM, CP_LJXM_VM>((yzxm, ljxm) => new JoinQueryInfos(
                JoinType.Inner, yzxm.ljxmbm == ljxm.xmbm))
                .Where((yzxm, ljxm) => yzxm.yzpd == "2")
                .Where((yzxm, ljxm) => listyzbm.Contains(yzxm.yzbm))
                .Select((yzxm, ljxm) => new CPAdviceDetail_Model
                {
                    yzbm = yzxm.yzbm,
                    yzlx = yzxm.yzlx,
                    yzzl = yzxm.yzzl,
                    yzpd = yzxm.yzpd,
                    xsxh = yzxm.xsxh,
                    yzmxxmbm = yzxm.yzmxxmbm,
                    yzfzh = yzxm.yzfzh,
                    dcsl = yzxm.dcsl,
                    yyff = yzxm.yyff,
                    dcjl = yzxm.dcjl,
                    jldw = yzxm.jldw,
                    pcbm = yzxm.pcbm,
                    zl = yzxm.zl,
                    yssm = yzxm.yssm,
                    sysd = yzxm.sysd,
                    sydw = yzxm.sydw,
                    zxks = yzxm.zxks,
                    yfbm = yzxm.yfbm,
                    ljxmbm = yzxm.ljxmbm,
                    cjczy = yzxm.cjczy,
                    cjrq = yzxm.cjrq,
                    ljbm = yzxm.ljbm,
                    jdbm = yzxm.jdbm,
                    bxxm = yzxm.bxxm,
                    zlfa = yzxm.zlfa,
                    sfcy = yzxm.sfcy,
                    fjsl = yzxm.fjsl,
                    fjmc = yzxm.fjmc,
                    zcyyf = yzxm.zcyyf,
                    cflx = yzxm.cflx,
                    ls_ljbm = yzxm.ls_ljbm,
                    ls_jdbm = yzxm.ls_jdbm,
                    ls_xmbm = yzxm.ls_xmbm,
                    ls_yzbm = yzxm.ls_yzbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljbm).Select(it => it.ljmc),
                    jdmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.jdbm).Select(it => it.ljmc),
                    ljxmmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljxmbm).Select(it => it.ljmc),
                    zlfamc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.zlfa).Select(it => it.ljmc),
                    yzlxmc = "",
                    yzzlmc = "",
                    yzpdmc = "",
                    yzmxxmmc = yzxm.yzmxxmbm,
                    bxxmmc = "",
                    jldwmc = SqlFunc.Subqueryable<YK_JLDWBM_VM>().Where(it => it.jldwid == yzxm.jldw).Select(it => it.jldwmc),
                    pcmc = SqlFunc.Subqueryable<PU_PC_VM>().Where(it => it.pcbm == yzxm.pcbm).Select(it => it.pcmc),
                    yyffmc = SqlFunc.Subqueryable<PU_GYTJ_VM>().Where(it => it.tjbm == yzxm.yyff).Select(it => it.tjmc),
                    sydwmc = "",
                    cjczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == yzxm.cjczy).Select(it => it.czyxm),
                    cflxmc = SqlFunc.Subqueryable<YF_CFLX_VM>().Where(it => it.cflxbm == yzxm.cflx).Select(it => it.cflxmc),
                    gg = "",
                    jbjl = 0,
                    jldw_ypzd = "",
                    psypbz = "",
                    tclbid = "",
                    hldj = "",
                    hldjmc = "",
                    xsxh_ljxm = ljxm.xsxh,
                    ylyzfl = "",
                    ylyzflmc = "",
                    xmxz = ljxm.xmxz,
                    xmnr = ljxm.xmnr,
                    xmzxfs = ljxm.xmzxfs,
                    xmzxfsmc = "",
                    ypzlbm = "",
                    ypzlmc = "",
                    ypjxid = "",
                    ypjxmc = "",
                    lsxh = 0
                })
                .Mapper(async yzxm =>
                {
                    yzxm.yzlxmc = await _Dddw.GetNameForCode("zyyzlx", yzxm.yzlx);
                    yzxm.yzzlmc = await _Dddw.GetNameForCode("yzzl_cq", yzxm.yzzl);
                    yzxm.yzpdmc = await _Dddw.GetNameForCode("CP_Yzpd", yzxm.yzpd);
                    yzxm.bxxmmc = await _Dddw.GetNameForCode("CP_Bxxm", yzxm.bxxm);                    
                    yzxm.xmzxfsmc = await _Dddw.GetNameForCode("CP_Xmxzfs", yzxm.xmzxfs);
                });
            return await Db.UnionAll(q,q_yl,q_yp).ToListAsync();
        }
        /// <summary>
        /// 查询路径、阶段、治疗方案下的医嘱列表 执行路径时查询使用
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
        public async Task<List<CPAdviceDetail_Model>> GetListCpMedicalAdvice(string ljbm, string jdbm, string zlfa, string zyh, List<string> listljxmbm, int page, int limit, string filter, RefAsync<int> total)
        {
            var q_yp = Db.Queryable<CP_YZXM_VM, YK_YPZD_VM, CP_LJXM_VM>((yzxm, ypzd, ljxm) => new JoinQueryInfos(
                JoinType.Inner, yzxm.yzmxxmbm == ypzd.ypbm,
                JoinType.Inner, yzxm.ljxmbm == ljxm.xmbm))
                .WhereIF(!string.IsNullOrWhiteSpace(filter), (yzxm, ypzd, ljxm) => yzxm.yzmxxmbm.Contains(filter) || ypzd.ypmc.Contains(filter) || ypzd.pyjm.ToUpper().Contains(filter.ToUpper()))
                .Where((yzxm, ypzd, ljxm) => yzxm.ljbm == ljbm)
                .Where((yzxm, ypzd, ljxm) => yzxm.jdbm == jdbm)
                .Where((yzxm, ypzd, ljxm) => yzxm.zlfa == zlfa || (yzxm.zlfa == null || yzxm.zlfa == ""))
                .Where((yzxm, ypzd, ljxm) => yzxm.yzpd == "1")
                .Where((yzxm, ypzd, ljxm) => listljxmbm.Contains(yzxm.ljxmbm))
                .Select((yzxm, ypzd, ljxm) => new CPAdviceDetail_Model
                {
                    yzbm = yzxm.yzbm,
                    yzlx = yzxm.yzlx,
                    yzzl = yzxm.yzzl,
                    yzpd = yzxm.yzpd,
                    xsxh = yzxm.xsxh,
                    yzmxxmbm = yzxm.yzmxxmbm,
                    yzfzh = yzxm.yzfzh,
                    dcsl = yzxm.dcsl,
                    yyff = yzxm.yyff,
                    dcjl = yzxm.dcjl,
                    jldw = yzxm.jldw,
                    pcbm = yzxm.pcbm,
                    zl = yzxm.zl,
                    yssm = yzxm.yssm,
                    sysd = yzxm.sysd,
                    sydw = yzxm.sydw,
                    zxks = yzxm.zxks,
                    yfbm = yzxm.yfbm,
                    ljxmbm = yzxm.ljxmbm,
                    cjczy = yzxm.cjczy,
                    cjrq = yzxm.cjrq,
                    ljbm = yzxm.ljbm,
                    jdbm = yzxm.jdbm,
                    bxxm = yzxm.bxxm,
                    zlfa = yzxm.zlfa,
                    sfcy = yzxm.sfcy,
                    fjsl = yzxm.fjsl,
                    fjmc = yzxm.fjmc,
                    zcyyf = yzxm.zcyyf,
                    cflx = yzxm.cflx,
                    ls_ljbm = yzxm.ls_ljbm,
                    ls_jdbm = yzxm.ls_jdbm,
                    ls_xmbm = yzxm.ls_xmbm,
                    ls_yzbm = yzxm.ls_yzbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljbm).Select(it => it.ljmc),
                    jdmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.jdbm).Select(it => it.ljmc),
                    ljxmmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljxmbm).Select(it => it.ljmc),
                    zlfamc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.zlfa).Select(it => it.ljmc),
                    yzlxmc = "",
                    yzzlmc = "",
                    yzpdmc = "",
                    yzmxxmmc = ypzd.ypmc,
                    bxxmmc = "",
                    jldwmc = SqlFunc.Subqueryable<YK_JLDWBM_VM>().Where(it => it.jldwid == yzxm.jldw).Select(it => it.jldwmc),
                    pcmc = SqlFunc.Subqueryable<PU_PC_VM>().Where(it => it.pcbm == yzxm.pcbm).Select(it => it.pcmc),
                    yyffmc = SqlFunc.Subqueryable<PU_GYTJ_VM>().Where(it => it.tjbm == yzxm.yyff).Select(it => it.tjmc),
                    sydwmc = "",
                    cjczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == yzxm.cjczy).Select(it => it.czyxm),
                    cflxmc = SqlFunc.Subqueryable<YF_CFLX_VM>().Where(it => it.cflxbm == yzxm.cflx).Select(it => it.cflxmc),
                    gg = ypzd.ypgg,
                    jbjl = ypzd.jbjl,
                    jldw_ypzd = ypzd.jldw,
                    psypbz = ypzd.psypbz,
                    tclbid = ypzd.tclbid,
                    hldj = "",
                    hldjmc = "",
                    xsxh_ljxm = ljxm.xsxh,
                    ylyzfl = "",
                    ylyzflmc = "",
                    xmxz = ljxm.xmxz,
                    xmnr = ljxm.xmnr,
                    xmzxfs = ljxm.xmzxfs,
                    xmzxfsmc = "",
                    ypzlbm = ypzd.ypzlbm,
                    ypzlmc = SqlFunc.Subqueryable<YK_YPZL_VM>().Where(it => it.ypzlbm == ypzd.ypzlbm).Select(it => it.ypzlmc),
                    ypjxid = ypzd.ypjxid,
                    ypjxmc = SqlFunc.Subqueryable<YK_JXBM_VM>().Where(it => it.ypjxid == ypzd.ypjxid).Select(it => it.jxmc),
                    lsxh = 0,
                    wtyz = SqlFunc.Subqueryable<INHOSD_YPYZMX_VM>().InnerJoin<INHOSD_YPYZ_VM>((ypyzmx, ypyz) => ypyzmx.ypyzxh == ypyz.ypyzxh)
                                                                   .Where((ypyzmx, ypyz) => ypyz.zyh == zyh)
                                                                   .Where((ypyzmx, ypyz) => ypyz.yzlx == "1")
                                                                   .Where((ypyzmx, ypyz) => ypyzmx.ystzbz == "0")
                                                                   .Where((ypyzmx, ypyz) => ypyzmx.lclj_yzbm == "" || ypyzmx.lclj_yzbm != null)
                                                                   .Where((ypyzmx, ypyz) => ypyzmx.ryypbm == yzxm.yzmxxmbm)
                                                                   .Select((ypyzmx, ypyz) => SqlFunc.IIF(ypyzmx.ypyzxh != "" || ypyzmx.ypyzxh != null, "1", "0"))
                })
                .Mapper(async yzxm =>
                {
                    yzxm.yzlxmc = await _Dddw.GetNameForCode("zyyzlx", yzxm.yzlx);
                    yzxm.yzzlmc = await _Dddw.GetNameForCode("yzzl_cq", yzxm.yzzl);
                    yzxm.yzpdmc = await _Dddw.GetNameForCode("CP_Yzpd", yzxm.yzpd);
                    yzxm.bxxmmc = await _Dddw.GetNameForCode("CP_Bxxm", yzxm.bxxm);
                    yzxm.sydwmc = await _Dddw.GetNameForCode("sydw", yzxm.sydw);
                    yzxm.xmzxfsmc = await _Dddw.GetNameForCode("CP_Xmxzfs", yzxm.xmzxfs);
                });
            var q_yl = Db.Queryable<CP_YZXM_VM, PU_MXZLXM_VM, PU_ZLXL_VM, PU_ZLDL_VM, CP_LJXM_VM>((yzxm, zlxm, zlxl, zldl, ljxm) => new JoinQueryInfos(
                JoinType.Inner, yzxm.yzmxxmbm == zlxm.mxzlxmbm,
                JoinType.Inner, zlxm.zlxlbm == zlxl.zlxlbm,
                JoinType.Inner, zlxl.zldlbm == zldl.zldlbm,
                JoinType.Inner, yzxm.ljxmbm == ljxm.xmbm))
                .WhereIF(!string.IsNullOrWhiteSpace(filter), (yzxm, zlxm, zlxl, zldl, ljxm) => yzxm.yzmxxmbm.Contains(filter) || zlxm.mxzlxmmc.Contains(filter) || zlxm.mxzlxmdm.ToUpper().Contains(filter.ToUpper()))
                .Where((yzxm, zlxm, zlxl, zldl, ljxm) => yzxm.ljbm == ljbm)
                .Where((yzxm, zlxm, zlxl, zldl, ljxm) => yzxm.jdbm == jdbm)
                .Where((yzxm, zlxm, zlxl, zldl, ljxm) => yzxm.zlfa == zlfa || (yzxm.zlfa == null || yzxm.zlfa == ""))
                .Where((yzxm, zlxm, zlxl, zldl, ljxm) => yzxm.yzpd == "0")
                .Where((yzxm, zlxm, zlxl, zldl, ljxm) => listljxmbm.Contains(yzxm.ljxmbm))
                .Select((yzxm, zlxm, zlxl, zldl, ljxm) => new CPAdviceDetail_Model
                {
                    yzbm = yzxm.yzbm,
                    yzlx = yzxm.yzlx,
                    yzzl = yzxm.yzzl,
                    yzpd = yzxm.yzpd,
                    xsxh = yzxm.xsxh,
                    yzmxxmbm = yzxm.yzmxxmbm,
                    yzfzh = yzxm.yzfzh,
                    dcsl = yzxm.dcsl,
                    yyff = yzxm.yyff,
                    dcjl = yzxm.dcjl,
                    jldw = yzxm.jldw,
                    pcbm = yzxm.pcbm,
                    zl = yzxm.zl,
                    yssm = yzxm.yssm,
                    sysd = yzxm.sysd,
                    sydw = yzxm.sydw,
                    zxks = yzxm.zxks,
                    yfbm = yzxm.yfbm,
                    ljxmbm = yzxm.ljxmbm,
                    cjczy = yzxm.cjczy,
                    cjrq = yzxm.cjrq,
                    ljbm = yzxm.ljbm,
                    jdbm = yzxm.jdbm,
                    bxxm = yzxm.bxxm,
                    zlfa = yzxm.zlfa,
                    sfcy = yzxm.sfcy,
                    fjsl = yzxm.fjsl,
                    fjmc = yzxm.fjmc,
                    zcyyf = yzxm.zcyyf,
                    cflx = yzxm.cflx,
                    ls_ljbm = yzxm.ls_ljbm,
                    ls_jdbm = yzxm.ls_jdbm,
                    ls_xmbm = yzxm.ls_xmbm,
                    ls_yzbm = yzxm.ls_yzbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljbm).Select(it => it.ljmc),
                    jdmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.jdbm).Select(it => it.ljmc),
                    ljxmmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljxmbm).Select(it => it.ljmc),
                    zlfamc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.zlfa).Select(it => it.ljmc),
                    yzlxmc = "",
                    yzzlmc = "",
                    yzpdmc = "",
                    yzmxxmmc = zlxm.mxzlxmmc,
                    bxxmmc = "",
                    jldwmc = SqlFunc.Subqueryable<YK_JLDWBM_VM>().Where(it => it.jldwid == yzxm.jldw).Select(it => it.jldwmc),
                    pcmc = SqlFunc.Subqueryable<PU_PC_VM>().Where(it => it.pcbm == yzxm.pcbm).Select(it => it.pcmc),
                    yyffmc = SqlFunc.Subqueryable<PU_GYTJ_VM>().Where(it => it.tjbm == yzxm.yyff).Select(it => it.tjmc),
                    sydwmc = "",
                    cjczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == yzxm.cjczy).Select(it => it.czyxm),
                    cflxmc = SqlFunc.Subqueryable<YF_CFLX_VM>().Where(it => it.cflxbm == yzxm.cflx).Select(it => it.cflxmc),
                    gg = "",
                    jbjl = 0,
                    jldw_ypzd = "",
                    psypbz = "",
                    tclbid = "",
                    hldj = zlxm.hldj,
                    hldjmc = "",
                    xsxh_ljxm = ljxm.xsxh,
                    ylyzfl = "",
                    ylyzflmc = "",
                    xmxz = ljxm.xmxz,
                    xmnr = ljxm.xmnr,
                    xmzxfs = ljxm.xmzxfs,
                    xmzxfsmc = "",
                    ypzlbm = "",
                    ypzlmc = "",
                    ypjxid = "",
                    ypjxmc = "",
                    lsxh = 0,
                    wtyz = SqlFunc.Subqueryable<INHOSD_YLYZMX_VM>().InnerJoin<INHOSD_YLYZ_VM>((ylyzmx, ylyz) => ylyzmx.ylyzxh == ylyz.ylyzxh)
                                                                   .Where((ylyzmx, ylyz) => ylyz.zyh == zyh)
                                                                   .Where((ylyzmx, ylyz) => ylyz.yzlx == "1")
                                                                   .Where((ylyzmx, ylyz) => ylyzmx.ystzbz == "0")
                                                                   .Where((ylyzmx, ylyz) => ylyzmx.lclj_yzbm == "" || ylyzmx.lclj_yzbm != null)
                                                                   .Where((ylyzmx, ylyz) => ylyzmx.mxzlxmbm == yzxm.yzmxxmbm)
                                                                   .Select((ylyzmx, ylyz) => SqlFunc.IIF(ylyzmx.ypyzxh != "" || ylyzmx.ylyzxh != null, "1", "0"))
                })
                .Mapper(async yzxm =>
                {
                    yzxm.yzlxmc = await _Dddw.GetNameForCode("zyyzlx", yzxm.yzlx);
                    yzxm.yzzlmc = await _Dddw.GetNameForCode("yzzl_cq", yzxm.yzzl);
                    yzxm.yzpdmc = await _Dddw.GetNameForCode("CP_Yzpd", yzxm.yzpd);
                    yzxm.bxxmmc = await _Dddw.GetNameForCode("CP_Bxxm", yzxm.bxxm);
                    yzxm.hldjmc = await _Dddw.GetNameForCode("hldj", yzxm.hldj);
                    yzxm.ylyzflmc = await _Dddw.GetNameForCode("zlyzfl", yzxm.ylyzfl);
                    yzxm.xmzxfsmc = await _Dddw.GetNameForCode("CP_Xmxzfs", yzxm.xmzxfs);
                });
            var q = Db.Queryable<CP_YZXM_VM, CP_LJXM_VM>((yzxm, ljxm) => new JoinQueryInfos(
                JoinType.Inner, yzxm.ljxmbm == ljxm.xmbm))
                .WhereIF(!string.IsNullOrWhiteSpace(filter), yzxm => yzxm.yzmxxmbm.Contains(filter))
                .Where((yzxm, ljxm) => yzxm.ljbm == ljbm)
                .Where((yzxm, ljxm) => yzxm.jdbm == jdbm)
                .Where((yzxm, ljxm) => yzxm.zlfa == zlfa || (yzxm.zlfa == null || yzxm.zlfa == ""))
                .Where((yzxm, ljxm) => yzxm.yzpd == "2")
                .Where((yzxm, ljxm) => listljxmbm.Contains(yzxm.ljxmbm))
                .Select((yzxm, ljxm) => new CPAdviceDetail_Model
                {
                    yzbm = yzxm.yzbm,
                    yzlx = yzxm.yzlx,
                    yzzl = yzxm.yzzl,
                    yzpd = yzxm.yzpd,
                    xsxh = yzxm.xsxh,
                    yzmxxmbm = yzxm.yzmxxmbm,
                    yzfzh = yzxm.yzfzh,
                    dcsl = yzxm.dcsl,
                    yyff = yzxm.yyff,
                    dcjl = yzxm.dcjl,
                    jldw = yzxm.jldw,
                    pcbm = yzxm.pcbm,
                    zl = yzxm.zl,
                    yssm = yzxm.yssm,
                    sysd = yzxm.sysd,
                    sydw = yzxm.sydw,
                    zxks = yzxm.zxks,
                    yfbm = yzxm.yfbm,
                    ljxmbm = yzxm.ljxmbm,
                    cjczy = yzxm.cjczy,
                    cjrq = yzxm.cjrq,
                    ljbm = yzxm.ljbm,
                    jdbm = yzxm.jdbm,
                    bxxm = yzxm.bxxm,
                    zlfa = yzxm.zlfa,
                    sfcy = yzxm.sfcy,
                    fjsl = yzxm.fjsl,
                    fjmc = yzxm.fjmc,
                    zcyyf = yzxm.zcyyf,
                    cflx = yzxm.cflx,
                    ls_ljbm = yzxm.ls_ljbm,
                    ls_jdbm = yzxm.ls_jdbm,
                    ls_xmbm = yzxm.ls_xmbm,
                    ls_yzbm = yzxm.ls_yzbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljbm).Select(it => it.ljmc),
                    jdmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.jdbm).Select(it => it.ljmc),
                    ljxmmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljxmbm).Select(it => it.ljmc),
                    zlfamc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.zlfa).Select(it => it.ljmc),
                    yzlxmc = "",
                    yzzlmc = "",
                    yzpdmc = "",
                    yzmxxmmc = yzxm.yzmxxmbm,
                    bxxmmc = "",
                    jldwmc = SqlFunc.Subqueryable<YK_JLDWBM_VM>().Where(it => it.jldwid == yzxm.jldw).Select(it => it.jldwmc),
                    pcmc = SqlFunc.Subqueryable<PU_PC_VM>().Where(it => it.pcbm == yzxm.pcbm).Select(it => it.pcmc),
                    yyffmc = SqlFunc.Subqueryable<PU_GYTJ_VM>().Where(it => it.tjbm == yzxm.yyff).Select(it => it.tjmc),
                    sydwmc = "",
                    cjczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == yzxm.cjczy).Select(it => it.czyxm),
                    cflxmc = SqlFunc.Subqueryable<YF_CFLX_VM>().Where(it => it.cflxbm == yzxm.cflx).Select(it => it.cflxmc),
                    gg = "",
                    jbjl = 0,
                    jldw_ypzd = "",
                    psypbz = "",
                    tclbid = "",
                    hldj = "",
                    hldjmc = "",
                    xsxh_ljxm = ljxm.xsxh,
                    ylyzfl = "",
                    ylyzflmc = "",
                    xmxz = ljxm.xmxz,
                    xmnr = ljxm.xmnr,
                    xmzxfs = ljxm.xmzxfs,
                    xmzxfsmc = "",
                    ypzlbm = "",
                    ypzlmc = "",
                    ypjxid = "",
                    ypjxmc = "",
                    lsxh = 0,
                    wtyz = SqlFunc.Subqueryable<INHOSD_YLYZMX_VM>().InnerJoin<INHOSD_YLYZ_VM>((ylyzmx, ylyz) => ylyzmx.ylyzxh == ylyz.ylyzxh)
                                                                   .Where((ylyzmx, ylyz) => ylyz.zyh == zyh)
                                                                   .Where((ylyzmx, ylyz) => ylyz.yzlx == "1")
                                                                   .Where((ylyzmx, ylyz) => ylyzmx.ystzbz == "0")
                                                                   .Where((ylyzmx, ylyz) => ylyzmx.lclj_yzbm == "" || ylyzmx.lclj_yzbm != null)
                                                                   .Where((ylyzmx, ylyz) => ylyzmx.mxzlxmbm == yzxm.yzmxxmbm)
                                                                   .Select((ylyzmx, ylyz) => SqlFunc.IIF(ylyzmx.ypyzxh != "" || ylyzmx.ylyzxh != null, "1", "0"))
                })
                .Mapper(async yzxm =>
                {
                    yzxm.yzlxmc = await _Dddw.GetNameForCode("zyyzlx", yzxm.yzlx);
                    yzxm.yzzlmc = await _Dddw.GetNameForCode("yzzl_cq", yzxm.yzzl);
                    yzxm.yzpdmc = await _Dddw.GetNameForCode("CP_Yzpd", yzxm.yzpd);
                    yzxm.bxxmmc = await _Dddw.GetNameForCode("CP_Bxxm", yzxm.bxxm);
                    yzxm.xmzxfsmc = await _Dddw.GetNameForCode("CP_Xmxzfs", yzxm.xmzxfs);
                });
            return await base.GetListForQueryable(Db.UnionAll(q, q_yl, q_yp), page, limit, total);
        }
        /// <summary>
        /// 查询指定的医嘱列表 
        /// </summary>
        /// <param name="listyzbm">医嘱编码列表</param>
        /// <returns></returns>
        public async Task<List<CP_YZXM_VM>> GetListCpMedicalAdvice(List<string> listyzbm)
        {
            return await Db.Queryable<CP_YZXM_VM>()
                .Where(yzxm => listyzbm.Contains(yzxm.ljxmbm))
                .Select(yzxm => new CP_YZXM_VM
                {
                    ljbm = yzxm.ljbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljbm).Select(it => it.ljmc),
                    jdbm = yzxm.jdbm,
                    jdmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.jdbm).Select(it => it.ljmc),
                    ljxmbm = yzxm.ljxmbm,
                    ljxmmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljxmbm).Select(it => it.ljmc),
                    zlfamc = yzxm.zlfamc,
                    zlfa = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.zlfa).Select(it => it.ljmc),
                    yzmxxmbm = yzxm.yzmxxmbm,
                    yzmxxmmc = SqlFunc.IIF(yzxm.yzpd == "0", SqlFunc.Subqueryable<PU_MXZLXM_VM>().Where(it => it.mxzlxmbm == yzxm.yzmxxmbm).Select(it => it.mxzlxmmc), SqlFunc.IIF(yzxm.yzpd == "1", SqlFunc.Subqueryable<YK_YPZD_VM>().Where(it => it.ypbm == yzxm.yzmxxmbm).Select(it => it.ypmc), yzxm.yzmxxmbm)),
                    jldw = yzxm.jldw,
                    jldwmc = SqlFunc.Subqueryable<YK_JLDWBM_VM>().Where(it => it.jldwid == yzxm.jldw).Select(it => it.jldwmc),
                    pcbm = yzxm.pcbm,
                    pcmc = SqlFunc.Subqueryable<PU_PC_VM>().Where(it => it.pcbm == yzxm.pcbm).Select(it => it.pcmc),
                    yyff = yzxm.yyff,
                    yyffmc = SqlFunc.Subqueryable<PU_GYTJ_VM>().Where(it => it.tjbm == yzxm.yyff).Select(it => it.tjmc),
                    cjczy = yzxm.cjczy,
                    cjczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == yzxm.cjczy).Select(it => it.czyxm),
                    cflx = yzxm.cflx,
                    cflxmc = SqlFunc.Subqueryable<YF_CFLX_VM>().Where(it => it.cflxbm == yzxm.cflx).Select(it => it.cflxmc),
                }, true)
                .Mapper(async yzxm =>
                {
                    yzxm.yzlxmc = await _Dddw.GetNameForCode("zyyzlx", yzxm.yzlx);
                    yzxm.yzzlmc = await _Dddw.GetNameForCode("yzzl_cq", yzxm.yzzl);
                    yzxm.yzpdmc = await _Dddw.GetNameForCode("CP_Yzpd", yzxm.yzpd);
                    yzxm.bxxmmc = await _Dddw.GetNameForCode("CP_Bxxm", yzxm.bxxm);
                    yzxm.sydwmc = await _Dddw.GetNameForCode("sydw", yzxm.sydw);
                }).ToListAsync();
        }
        /// <summary>
        /// 查询病人已保存的阶段医嘱列表
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="jdbm"></param>
        /// <returns></returns>
        public async Task<List<AdviceDetailSimple_Model>> GetListPatientAdvice(string zyh, string jdbm)
        {
            var q1 = Db.Queryable<INHOSD_YPYZMX_VM, INHOSD_YPYZ_VM>((yzmx, yz) => new JoinQueryInfos(
                JoinType.Inner, yzmx.ypyzxh == yz.ypyzxh))
                .Where((yzmx, yz) => yz.zyh == zyh)
                .Where((yzmx, yz) => yzmx.lclj_jdbm == jdbm)
                .Select((yzmx, yz) => new AdviceDetailSimple_Model
                {
                    lclj_yzbm = yzmx.lclj_yzbm,
                    xmbm = yzmx.ryypbm,
                    fzh = yzmx.fzh,
                    yzlx = yz.yzlx
                });
            var q2 = Db.Queryable<INHOSD_YLYZMX_VM, INHOSD_YLYZ_VM>((yzmx, yz) => new JoinQueryInfos(
                JoinType.Inner, yzmx.ylyzxh == yz.ylyzxh))
                .Where((yzmx, yz) => yz.zyh == zyh)
                .Where((yzmx, yz) => yzmx.lclj_jdbm == jdbm)
                .Select((yzmx, yz) => new AdviceDetailSimple_Model
                {
                    lclj_yzbm = yzmx.lclj_yzbm,
                    xmbm = yzmx.mxzlxmbm,
                    fzh = yzmx.fzh,
                    yzlx = yz.yzlx
                });
            return await Db.UnionAll(q1, q2).ToListAsync();
        }
        /// <summary>
        /// 查询病人未停长期医嘱
        /// </summary>
        /// <param name="zyh"></param>
        /// <returns></returns>
        public async Task<List<INHOSD_YPYZMX_VM>> GetListPatientDrugAdvice(string zyh)
        {
            return await Db.Queryable<INHOSD_YPYZMX_VM, INHOSD_YPYZ_VM>((yzmx, yz) => new JoinQueryInfos(
                JoinType.Inner, yzmx.ypyzxh == yz.ypyzxh))
                .Where((yzmx, yz) => yz.zyh == zyh)
                .Where((yzmx, yz) => yz.yzlx == "1")
                .Where((yzmx, yz) => yzmx.ystzbz == "0")
                //.Select((yzmx, yz) => new INHOSD_YPYZMX_VM
                //{

                //},true)
                .ToListAsync();
        }
        /// <summary>
        /// 查询病人未停临期医嘱
        /// </summary>
        /// <param name="zyh"></param>
        /// <returns></returns>
        public async Task<List<INHOSD_YLYZMX_VM>> GetListPatientMedicalAdvice(string zyh)
        {
            return await Db.Queryable<INHOSD_YLYZMX_VM, INHOSD_YLYZ_VM>((yzmx, yz) => new JoinQueryInfos(
                JoinType.Inner, yzmx.ylyzxh == yz.ylyzxh))
                .Where((yzmx, yz) => yz.zyh == zyh)
                .Where((yzmx, yz) => yz.yzlx == "1")
                .Where((yzmx, yz) => yzmx.ystzbz == "0")
                //.Select((yzmx, yz) => new INHOSD_YLYZMX_VM
                //{

                //}, true)
                .ToListAsync();
        }
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
        public async Task<DbResult<bool>> CPPreExecution(List<CP_LJXM_ZXJL_VM> listljxmzjjl, List<CP_ZXJL_VM> listyzzxjl, List<INHOSD_YLYZ_VM> listylyz ,List<INHOSD_YLYZMX_VM> listylyzmx ,List<INHOSD_YPYZ_VM> listypyz ,List<INHOSD_YPYZMX_VM> listypyzmx ,List<INHOSD_YPYZMX_VM> listypyzmx_tz ,List<INHOSD_YLYZMX_VM> listylyzmx_tz)
        {
            return await Db.UseTranAsync(async () =>
            {
                foreach(var vm in listljxmzjjl)
                {
                    var x = base.Db.Storageable(vm).ToStorage();
                    await x.AsInsertable.IgnoreColumns(ignoreNullColumn: true).IgnoreColumns(new string[] { "timestamp" }).ExecuteCommandAsync();
                    await x.AsUpdateable.UpdateColumns(new string[] {}).ExecuteCommandAsync();
                }
                foreach (var vm in listyzzxjl)
                {
                    var x = base.Db.Storageable(vm).ToStorage();
                    await x.AsInsertable.IgnoreColumns(ignoreNullColumn: true).IgnoreColumns(new string[] { "timestamp" }).ExecuteCommandAsync();
                    await x.AsUpdateable.UpdateColumns(new string[] { }).ExecuteCommandAsync();
                }

                foreach (var vm in listylyz)
                {
                    await Db.Insertable(vm).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
                }
                foreach (var vm in listylyzmx)
                {
                    await Db.Insertable(vm).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
                }
                foreach (var vm in listypyz)
                {
                    await Db.Insertable(vm).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
                }
                foreach (var vm in listypyzmx)
                {
                    await Db.Insertable(vm).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
                }

                await Db.Updateable(listypyzmx_tz).UpdateColumns().ExecuteCommandAsync();
                await Db.Updateable(listylyzmx_tz).UpdateColumns().ExecuteCommandAsync();
            });            
        }
        #endregion
        
        #region 护士执行
        //病人列表 同GetListCPPatient

        //路径阶段 同GetListPhaseForCPCode

        /// <summary>
        /// 根据住院号和路径编码获取护理评估记录
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        public async Task<List<CP_PGJL_HL_VM>> GetListNurseEvaluation(string zyh, string ljbm)
        {
            return await base.GetEntityList<CP_PGJL_HL_VM>(a => a.zyh == zyh && a.ljbm == ljbm);
        }
        /// <summary>
        /// 查询路径下的阶段以及是否有病人的评估
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        public async Task<object> GetListPhaseForCPCode(string zyh, string ljbm)
        {
            return await Db.Queryable<CP_LJJD_VM>()
                .Where(ljjd => ljjd.ljbm == ljbm)
                .Select(ljjd => new
                {
                    jdbm = ljjd.jdbm,
                    jdmc = ljjd.jdmc,
                    hlpg = SqlFunc.IIF(SqlFunc.Subqueryable<CP_PGJL_HL_VM>().Where(it => it.zyh == zyh && it.ljbm == ljbm).Count() > 0,"1","0")
                })
                .ToListAsync();
        }

        /// <summary>
        /// 根据路径编码和阶段编码获取医生或护士路径项目
        /// </summary>
        /// <param name="ljbm"></param>
        /// <param name="jdbm"></param>
        /// <param name="xmzxr">执行人类别 1=医生 2=护士</param>
        /// <returns></returns>
        public async Task<List<CP_LJXM_VM>> GetListCPProject(string ljbm, string jdbm, string xmzxr)
        {
            return await base.GetEntityList<CP_LJXM_VM>(a => a.ljbm == ljbm && a.jdbm == jdbm && a.xmzxr == xmzxr);                
        }
        /// <summary>
        /// 查询病人路径护理执行信息
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="ljbm"></param>
        /// <param name="jdbm"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public async Task<object> GetNurseCPExecutionRecordDetails(string zyh, string ljbm, string jdbm, int page, int limit, string filter, RefAsync<int> total)
        {
            var q = Db.Queryable<CP_LJXM_VM, CP_ZXJL_HL_VM>((ljxm, zxjl) => new JoinQueryInfos(
                JoinType.Left, ljxm.xmbm == zxjl.xmbm && zxjl.zyh == zyh && ljxm.jdbm == zxjl.jdbm && ljxm.ljbm == zxjl.ljbm))
                .WhereIF(!string.IsNullOrWhiteSpace(filter), (ljxm, zxjl) => ljxm.xmbm.Contains(filter) || ljxm.xmnr.Contains(filter))
                .Where((ljxm, zxjl) => ljxm.ljbm == ljbm)
                .Where((ljxm, zxjl) => ljxm.jdbm == jdbm)
                .OrderBy((ljxm, zxjl) => ljxm.xsxh)
                .Select((ljxm, zxjl) => new
                {
                    zxjl.id,
                    zyh = zyh,
                    ljxm.jdbm,
                    jdmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == zxjl.jdbm).Select(it => it.ljmc),
                    ljxm.xmbm,
                    zxjl.zxzt,
                    zxjl.zxry,
                    zxryxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == zxjl.zxry).Select(it => it.czyxm),
                    ljxm.xmnr,
                    ljxm.xmjdpg,
                    zxjl.jdrq,
                    zxjl.ljbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == zxjl.ljbm).Select(it => it.ljmc),
                    zxjl.zxqm,
                    zxjl.zxjg,
                    zxjl.zxsj_bb,
                    zxjl.zxsj_wb,
                    zxjl.zxsj_yb,
                    ljxm.zxdlbh,
                    zxdlmc = SqlFunc.Subqueryable<CP_ZXDL_VM>().Where(it => it.dlbm == ljxm.zxdlbh).Select(it => it.dlmc),
                    ljxm.xsxh,
                    ljxm.xmxz
                });
            return await base.GetListForQueryable(q, page, limit, total);
        }
        /// <summary>
        /// 获取护士阶段评估记录
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="ljbm"></param>
        /// <param name="jddm"></param>
        /// <returns></returns>
        public async Task<object> GetSingleNurseCpEvaluate(string zyh, string ljbm, string jdbm)
        {
            return await Db.Queryable<CP_PGJL_HL_VM>()
                .Where(pgjl => pgjl.zyh == zyh)
                .Where(pgjl => pgjl.ljbm == ljbm)
                .Where(pgjl => pgjl.jdbm == jdbm)
                .Select(pgjl => new
                {
                    id = pgjl.id,
                    zyh  = pgjl.zyh,
                    ljbm = pgjl.ljbm,
                    jdbm = pgjl.jdbm,
                    bbbz = pgjl.bbbz,
                    bbrq = pgjl.bbrq,
                    bbqm = pgjl.bbqm,
                    bbsm = pgjl.bbsm,
                    wbbz = pgjl.wbbz,
                    wbrq = pgjl.wbrq,
                    wbqm = pgjl.wbqm,
                    wbsm = pgjl.wbsm,
                    ybbz = pgjl.ybbz,
                    ybrq = pgjl.ybrq,
                    ybqm = pgjl.ybqm,
                    ybsm = pgjl.ybsm,
                    djrq = pgjl.djrq,
                    czybm = pgjl.czybm,
                    pgzt = pgjl.pgzt,
                    pgjg = pgjl.pgjg,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == pgjl.ljbm).Select(it => it.ljmc),
                    jdmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == pgjl.jdbm).Select(it => it.ljmc),
                    czyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == pgjl.czybm).Select(it => it.czyxm),
                }).FirstAsync();
        }
        /// <summary>
        /// 执行护理临床路径
        /// </summary>
        /// <param name="pgjl"></param>
        /// <param name="listzxjl"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> NurseCPPreExecution(CP_PGJL_HL_VM pgjl, string[] col, List<CP_ZXJL_HL_VM> listzxjl)
        {
            return await Db.UseTranAsync(async () =>
            {
                var x = base.Db.Storageable(pgjl).ToStorage();
                await x.AsInsertable.IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
                await x.AsUpdateable.UpdateColumns(col).ExecuteCommandAsync();
                foreach(var vm in listzxjl)
                {
                    await Db.Insertable(vm).IgnoreColumns(ignoreNullColumn: true).IgnoreColumns(it => it.id).ExecuteCommandAsync();
                }
            });
        }
        #endregion
    }
}
