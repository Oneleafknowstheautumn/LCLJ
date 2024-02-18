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
    public class CPPublicRepository : HisBaseService, ICPPublicRepository
    {
        private readonly IPublicDddwRepository _Dddw;
        public CPPublicRepository(IPublicDddwRepository dddw)
        {
            _Dddw = dddw;
        }

        /// <summary>
        /// 取单个路径对应的阶段
        /// </summary>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        public async Task<List<CP_LJJD_VM>> GetListPhaseForCPCode(string ljbm)
        {
            return await base.GetEntityList<CP_LJJD_VM>(a => a.ljbm == ljbm);
        }

        /// <summary>
        /// 根据医嘱编码获取路径项目
        /// </summary>
        /// <param name="listyzbm"></param>
        /// <returns></returns>
        public async Task<List<CP_LJXM_VM>> GetListCPProjectForCodeList(List<string> listljxmbm)
        {
            return await base.GetEntityList<CP_LJXM_VM>(a => listljxmbm.Contains(a.xmbm));
        }

        #region 按科室查询
        /// <summary>
        /// 查询科室路径名称列表
        /// </summary>
        /// <param name="ksbm"></param>
        /// <returns></returns>
        public async Task<object> GetListCPNameByDept(string ksbm, int page, int limit, string filter, RefAsync<int> total)
        {
            var q = Db.Queryable<CP_LJMC_VM>()
                .Where(mc => SqlFunc.Subqueryable<CP_KSLJ_VM>().Where(kslj => kslj.ksbm == ksbm).Where(kslj => kslj.ljbm == mc.ljbm).Any())
                .Select(mc => new
                {
                    mc.ljbm,
                    mc.ljmc,
                    mc.pydm,
                    mc.bzms,
                    mc.fl,
                    flmc = SqlFunc.IIF(mc.fl == "0", "卫生部", SqlFunc.IIF(mc.fl == "1", "院内", "")),
                    mc.blfx,
                    blfxmc = SqlFunc.IIF(mc.blfx == "1", "单纯普通型", SqlFunc.IIF(mc.blfx == "2", "单纯急症型", SqlFunc.IIF(mc.blfx == "3", "复杂疑难型", SqlFunc.IIF(mc.blfx == "4", "复杂危重型", "")))),
                    mc.sybq,
                    sybqmc = SqlFunc.IIF(mc.blfx == "0", "不区分", SqlFunc.IIF(mc.blfx == "1", "危", SqlFunc.IIF(mc.blfx == "2", "重", SqlFunc.IIF(mc.blfx == "3", "一般", "")))),
                    mc.syxb,
                    syxbmc = SqlFunc.IIF(mc.blfx == "0", "不区分", SqlFunc.IIF(mc.blfx == "1", "男", SqlFunc.IIF(mc.blfx == "2", "女", ""))),
                    mc.syks,
                    syksmc = SqlFunc.IIF(mc.fl == "1", "是", "否"),
                    mc.bzts,
                    mc.tybz,
                    mc.cjczy,
                    cjczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == mc.cjczy).Select(it => it.czyxm),
                    mc.cjrq,
                    mc.shbz,
                    mc.shczy,
                    shczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == mc.shczy).Select(it => it.czyxm),
                    mc.shrq,
                    mc.sydx,
                    mc.bzzyr,
                    mc.ckfy_from,
                    mc.ckfy_to,
                    mc.bddyls,
                    mc.rjxdsj,
                    mc.xssx,
                    mc.qysj,
                    mc.ls_ljbm,
                    mc.ls_yljbm,
                    mc.yyzbxtptbm,
                    mc.yyzbxtptmc
                });
            return await base.GetListForQueryable(q, page, limit, total);
        }
        /// <summary>
        /// 查询科室路径名称列表
        /// </summary>
        /// <param name="ksbm"></param>
        /// <returns></returns>
        public async Task<List<CP_LJMC_VM>> GetListCPNameByDept(string ksbm)
        {
            return await Db.Queryable<CP_LJMC_VM>()
                .Where(mc => SqlFunc.Subqueryable<CP_KSLJ_VM>().Where(kslj => kslj.ksbm == ksbm).Where(kslj => kslj.ljbm == mc.ljbm).Any())
                .Select(mc => new CP_LJMC_VM
                {

                }, true).ToListAsync();
        } 
        #endregion

        /// <summary>
        /// 查询路径下的阶段
        /// </summary>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        public async Task<object> GetCpPhase(string ljbm, int page, int limit, string filter, RefAsync<int> total)
        {
            var q = Db.Queryable<CP_LJJD_VM>()
                .Where(ljjd => ljjd.ljbm == ljbm)
                .Select(ljjd => new
                {
                    ljjd.jdbm,
                    ljjd.jdmc,
                    ljjd.jd_begin,
                    ljjd.jd_end,
                    ljjd.jdbz,
                    jdbzmc = SqlFunc.IIF(ljjd.jdbz == "0", "住院日", SqlFunc.IIF(ljjd.jdbz == "1", "手术日", SqlFunc.IIF(ljjd.jdbz == "2", "分娩日", SqlFunc.IIF(ljjd.jdbz == "3", "出院日", "")))),
                    ljjd.ljbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == ljjd.ljbm).Select(it => it.ljmc),
                    ljjd.jdsm,
                    ljjd.cjczy,
                    cjczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == ljjd.cjczy).Select(it => it.czyxm),
                    ljjd.cjrq,
                    ljjd.jdhf,
                    ljjd.ls_ljbm,
                    ljjd.ls_jdbm
                });
            return await base.GetListForQueryable(q, page, limit, total);
        }
        /// <summary>
        /// 查询路径下的阶段及项目
        /// </summary>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        public async Task<List<CP_LJJD_VM>> GetCpPhase(string ljbm)
        {
            return await Db.Queryable<CP_LJJD_VM>()
                .Where(ljjd => ljjd.ljbm == ljbm)
                .Select(ljjd => new CP_LJJD_VM
                {
                    jdbm = ljjd.jdbm,
                    jdmc = ljjd.jdmc,
                    jd_begin = ljjd.jd_begin,
                    jd_end = ljjd.jd_end,
                    jdbz = ljjd.jdbz,
                    ljbm = ljjd.ljbm,
                    jdsm = ljjd.jdsm,
                    cjczy = ljjd.cjczy,
                    cjrq = ljjd.cjrq,
                    jdhf = ljjd.jdhf,
                    ls_ljbm = ljjd.ls_ljbm,
                    ls_jdbm = ljjd.ls_jdbm
                }).ToListAsync();
        }
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
        public async Task<object> GetListCpProject(string ljbm, string jdbm, int page, int limit, string filter, RefAsync<int> total)
        {
            var q = Db.Queryable<CP_LJXM_VM>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter), ljxm => ljxm.xmbm.Contains(filter) || ljxm.xmnr.Contains(filter) || ljxm.xmdm.ToUpper().Contains(filter.ToUpper()))
                .WhereIF(!string.IsNullOrWhiteSpace(ljbm), ljxm => ljxm.ljbm == ljbm)
                .WhereIF(!string.IsNullOrWhiteSpace(jdbm), ljxm => ljxm.jdbm == jdbm)
                .Select(ljxm => new CP_LJXM_VM
                {
                    xmbm = ljxm.xmbm,
                    ljbm = ljxm.ljbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == ljxm.ljbm).Select(it => it.ljmc),
                    jdbm = ljxm.jdbm,
                    jdmc = SqlFunc.Subqueryable<CP_LJJD_VM>().Where(it => it.jdbm == ljxm.jdbm).Select(it => it.jdmc),
                    xmdl = ljxm.xmdl,
                    xmlx = ljxm.xmlx,
                    xsxh = ljxm.xsxh,
                    xmnr = ljxm.xmnr,
                    xmdm = ljxm.xmdm,
                    xmxz = ljxm.xmxz,
                    xmzxfs = ljxm.xmzxfs,
                    xmzxr = ljxm.xmzxr,
                    xmjdpg = ljxm.xmjdpg,
                    cjczy = ljxm.cjczy,
                    cjczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == ljxm.cjczy).Select(it => it.czyxm),
                    cjrq = ljxm.cjrq,
                    zxdlbh = ljxm.zxdlbh,
                    zxdlmc = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == ljxm.cjczy).Select(it => it.czyxm),
                    zxxlbh = ljxm.zxxlbh,
                    kxx = ljxm.kxx,
                    ls_ljbm = ljxm.ls_ljbm,
                    ls_jdbm = ljxm.ls_jdbm,
                    ls_xmbm = ljxm.ls_xmbm
                })
                .Mapper(async ljxm =>
                {
                    ljxm.xmdlmc = await _Dddw.GetNameForCode("CP_xmdl", ljxm.xmdl);
                    ljxm.xmlxmc = await _Dddw.GetNameForCode("CP_Xmlx", ljxm.xmlx);
                    ljxm.xmxzmc = await _Dddw.GetNameForCode("CP_Xmxzfs", ljxm.xmxz);
                    ljxm.xmzxfsmc = await _Dddw.GetNameForCode("CP_Xmzxfs", ljxm.xmzxfs);
                    ljxm.xmzxrlbmc = await _Dddw.GetNameForCode("CP_Xmzxrlb", ljxm.xmzxr);
                });
            return await base.GetListForQueryable(q, page, limit, total);
        }
        /// <summary>
        /// 查询路径项目列表
        /// </summary>
        /// <param name="ljbm">阶段编码</param>
        /// <returns></returns>
        public async Task<List<CP_LJXM_VM>> GetListCpProject(string ljbm)
        {
            return await Db.Queryable<CP_LJXM_VM>()
                .Where(ljxm => ljxm.ljbm == ljbm)
                .Select(ljxm => new CP_LJXM_VM
                {
                    xmbm = ljxm.xmbm,
                    ljbm = ljxm.ljbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == ljxm.ljbm).Select(it => it.ljmc),
                    jdbm = ljxm.jdbm,
                    jdmc = SqlFunc.Subqueryable<CP_LJJD_VM>().Where(it => it.jdbm == ljxm.jdbm).Select(it => it.jdmc),
                    xmdl = ljxm.xmdl,
                    xmlx = ljxm.xmlx,
                    xsxh = ljxm.xsxh,
                    xmnr = ljxm.xmnr,
                    xmdm = ljxm.xmdm,
                    xmxz = ljxm.xmxz,
                    xmzxfs = ljxm.xmzxfs,
                    xmzxr = ljxm.xmzxr,
                    xmjdpg = ljxm.xmjdpg,
                    cjczy = ljxm.cjczy,
                    cjczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == ljxm.cjczy).Select(it => it.czyxm),
                    cjrq = ljxm.cjrq,
                    zxdlbh = ljxm.zxdlbh,
                    zxdlmc = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == ljxm.cjczy).Select(it => it.czyxm),
                    zxxlbh = ljxm.zxxlbh,
                    kxx = ljxm.kxx,
                    ls_ljbm = ljxm.ls_ljbm,
                    ls_jdbm = ljxm.ls_jdbm,
                    ls_xmbm = ljxm.ls_xmbm
                })
                .Mapper(async ljxm =>
                {
                    ljxm.xmdlmc = await _Dddw.GetNameForCode("CP_xmdl", ljxm.xmdl);
                    ljxm.xmlxmc = await _Dddw.GetNameForCode("CP_Xmlx", ljxm.xmlx);
                    ljxm.xmxzmc = await _Dddw.GetNameForCode("CP_Xmxzfs", ljxm.xmxz);
                    ljxm.xmzxfsmc = await _Dddw.GetNameForCode("CP_Xmzxfs", ljxm.xmzxfs);
                    ljxm.xmzxrlbmc = await _Dddw.GetNameForCode("CP_Xmzxrlb", ljxm.xmzxr);
                }).ToListAsync();
        }
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
        public async Task<object> GetListCpMedicalAdvice(string ljbm, string jdbm, string xmbm, int page, int limit, string filter, RefAsync<int> total)
        {
            var q = Db.Queryable<CP_YZXM_VM>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter), yzxm => yzxm.yzmxxmbm.Contains(filter))
                .WhereIF(!string.IsNullOrWhiteSpace(ljbm), yzxm => yzxm.ljbm == ljbm)
                .WhereIF(!string.IsNullOrWhiteSpace(jdbm), yzxm => yzxm.jdbm == jdbm)
                .WhereIF(!string.IsNullOrWhiteSpace(xmbm), yzxm => yzxm.ljxmbm == xmbm)
                .Select(yzxm => new CP_YZXM_VM
                {
                    ljbm = yzxm.ljbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljbm).Select(it => it.ljmc),
                    jdbm = yzxm.jdbm,
                    jdmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.jdbm).Select(it => it.ljmc),
                    ljxmbm = yzxm.ljxmbm,
                    ljxmmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljxmbm).Select(it => it.ljmc),
                    zlfamc = yzxm.zlfamc,
                    zlfa = yzxm.zlfa,
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
                });
            return await base.GetListForQueryable(q, page, limit, total);
        }
        /// <summary>
        /// 查询中药医嘱明细
        /// </summary>
        /// <param name="yzbm"></param>
        /// <returns></returns>
        public async Task<object> GetListCpChineseMedicalAdvice(string yzbm)
        {
            return await Db.Queryable<CP_YZXM_ZYMX_VM, YK_YPZD_VM>((zymx, ypzd) => new JoinQueryInfos(
                JoinType.Inner, zymx.ypbm == ypzd.ypbm))
                .Where(zymx => zymx.yzbm == yzbm)
                .Select((zymx, ypzd) => new
                {
                    yzbm = zymx.yzbm,
                    mxxh = zymx.mxxh,
                    ypbm = zymx.ypbm,
                    ypmc = ypzd.ypmc,
                    dcsl = zymx.dcsl,
                    tjbm = zymx.tjbm,
                    tjmc = SqlFunc.Subqueryable<PU_GYTJ_VM>().Where(it => it.tjbm == zymx.tjbm).Select(it => it.tjmc),
                    jldw = zymx.jldw,
                    jldwmc = SqlFunc.Subqueryable<YK_JLDWBM_VM>().Where(it => it.jldwid == zymx.jldw).Select(it => it.jldwmc),
                }).ToListAsync();
        }

        #region 路径表单
        /// <summary>
        /// 根据路径编码查询路径阶段项目信息
        /// </summary>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        public async Task<object> GetListCpProject(string ljbm, string zyh)
        {
            return await Db.Queryable<CP_LJXM_VM, CP_LJJD_VM, CP_ZXDL_VM, CP_LJXM_ZXJL_VM>((ljxm, ljjd, zxdl, zxjl) => new JoinQueryInfos(
                JoinType.Inner, ljxm.ljbm == ljjd.ljbm && ljxm.jdbm == ljjd.jdbm,
                JoinType.Inner, ljxm.zxdlbh == zxdl.dlbm,
                JoinType.Left, zxjl.xmbm == ljxm.xmbm && zxjl.zyh == zyh))
                .Where((ljxm, ljjd, zxdl) => ljjd.ljbm == ljbm)
                .OrderBy((ljxm, ljjd, zxdl) => new { zxdl.xh, ljxm.xsxh })
                .Select((ljxm, ljjd, zxdl, zxjl) => new
                {
                    xh = zxdl.xh,
                    dlmc = zxdl.dlmc,
                    jdbm = ljjd.jdbm,
                    jdmc = ljjd.jdmc,
                    jd_begin = ljjd.jd_begin,
                    jd_end = ljjd.jd_end,
                    jdbz = ljjd.jdbz,
                    ljbm = ljjd.ljbm,
                    jdsm = ljjd.jdsm,
                    jdhf = ljjd.jdhf,
                    xmbm = ljxm.xmbm,
                    xmnr = ljxm.xmnr,
                    xsxh = ljxm.xsxh,
                    kxx = ljxm.kxx,
                    xmxz = ljxm.xmxz,
                    zxr = zxjl.zxr,
                    zxrxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == zxjl.zxr).Select(it => it.czyxm)
                }).ToListAsync();
        }
        /// <summary>
        /// 查询病人路径执行记录
        /// </summary>
        /// <param name="ljbm"></param>
        /// <param name="zyh"></param>
        /// <returns></returns>
        public async Task<object> GetListPatientCpExecutionRecord(string ljbm, string zyh)
        {
            return await Db.Queryable<CP_LJXM_ZXJL_VM>()
                .Where(zxjl => zxjl.zyh == zyh)
                .Where(zxjl => zxjl.ljbm == ljbm)
                .Select(zxjl => new
                {
                    zxjl.id,
                    zxjl.zyh,
                    zxjl.xmbm,
                    zxjl.ljbm,
                    zxjl.jdbm,
                    zxjl.xmnr,
                    zxjl.zxjg,
                    zxjl.jdpg,
                    zxjl.zxr,
                    zxjl.zxsj
                }).ToListAsync();
        }
        /// <summary>
        /// 获取病人路径阶段评估记录
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="jdbm"></param>
        /// <returns></returns>
        public async Task<object> GetPatientCPEvaluationRecord(string ljbm, string zyh)
        {
            return await Db.Queryable<CP_PGJL_VM>()
                .Where(pgjl => pgjl.zyh == zyh)
                .Where(pgjl => pgjl.ljbm == ljbm)
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
                }).ToListAsync();
        }
        #endregion
        #region 护理表单
        /// <summary>
        /// 根据路径编码查询护理路径阶段项目信息
        /// </summary>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        public async Task<object> GetListNurseCpProject(string ljbm, string zyh)
        {
            return await Db.Queryable<CP_LJXM_VM, CP_LJJD_VM, CP_ZXDL_VM, CP_ZXJL_HL_VM>((ljxm, ljjd, zxdl, zxjl) => new JoinQueryInfos(
                JoinType.Inner, ljxm.ljbm == ljjd.ljbm && ljxm.jdbm == ljjd.jdbm,
                JoinType.Inner, ljxm.zxdlbh == zxdl.dlbm,
                JoinType.Left, zxjl.xmbm == ljxm.xmbm && zxjl.zyh == zyh))
                .Where((ljxm, ljjd, zxdl) => ljjd.ljbm == ljbm)
                .Where(ljxm => ljxm.xmzxr == "2")
                .OrderBy((ljxm, ljjd, zxdl) => new { zxdl.xh, ljxm.xsxh })
                .Select((ljxm, ljjd, zxdl, zxjl) => new
                {
                    xh = zxdl.xh,
                    dlmc = zxdl.dlmc,
                    jdbm = ljjd.jdbm,
                    jdmc = ljjd.jdmc,
                    jd_begin = ljjd.jd_begin,
                    jd_end = ljjd.jd_end,
                    jdbz = ljjd.jdbz,
                    ljbm = ljjd.ljbm,
                    jdsm = ljjd.jdsm,
                    jdhf = ljjd.jdhf,
                    xmbm = ljxm.xmbm,
                    xmnr = ljxm.xmnr,
                    xsxh = ljxm.xsxh,
                    kxx = ljxm.kxx,
                    xmxz = ljxm.xmxz,
                    zxr = zxjl.zxry,
                    zxrxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == zxjl.zxry).Select(it => it.czyxm)
                }).ToListAsync();
        }
        /// <summary>
        /// 获取病人护理路径阶段评估记录
        /// </summary>
        /// <param name="zyh"></param>
        /// <param name="jdbm"></param>
        /// <returns></returns>
        public async Task<object> GetPatientNurseCPEvaluationRecord(string ljbm, string zyh)
        {
            return await Db.Queryable<CP_PGJL_HL_VM>()
                .Where(pgjl => pgjl.zyh == zyh)
                .Where(pgjl => pgjl.ljbm == ljbm)
                .Select(pgjl => new
                {
                    pgjl.id,
                    pgjl.zyh,
                    pgjl.pgzt,
                    ztsm = SqlFunc.IIF(pgjl.pgzt == "1", "变异继续", SqlFunc.IIF(pgjl.pgzt == "2", "变异退出", SqlFunc.IIF(pgjl.pgzt == "3", "正常出院", "正常"))),
                    pgjl.czybm,
                    czyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == pgjl.czybm).Select(it => it.czyxm),
                    pgjl.pgjg,
                    pgjl.jdbm,
                    jdmc = SqlFunc.Subqueryable<CP_LJJD_VM>().Where(it => it.jdbm == pgjl.jdbm).Select(it => it.jdmc),
                    pgjl.ljbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == pgjl.ljbm).Select(it => it.ljmc)
                }).ToListAsync();
        }
        #endregion
    }
}
