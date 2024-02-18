using His.DAL;
using His.Entities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace His.ClinicalPathway.Repository
{
    /// <summary>
    /// 路径表单维护
    /// </summary>
    public class CPFormMaintenanceRepository : HisBaseService, ICPFormMaintenanceRepository
    {
        private readonly IPublicDddwRepository _Dddw;
        public CPFormMaintenanceRepository(IPublicDddwRepository IDddw)
        {
            _Dddw = IDddw;
        }

        #region 路径阶段
        /// <summary>
        /// 查询路径阶段列表
        /// </summary>
        /// <param name="ljbm"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public async Task<object> GetListCpPhase(string ljbm, int page, int limit, string filter, RefAsync<int> total)
        {
            var q = Db.Queryable<CP_LJJD_VM>()
                .WhereIF(!string.IsNullOrWhiteSpace(ljbm), ljjd => ljjd.ljbm == ljbm)
                .WhereIF(!string.IsNullOrWhiteSpace(filter), ljjd => ljjd.jdbm.Contains(filter) || ljjd.jdmc.Contains(filter))
                .OrderBy(ljjd => ljjd.jdhf)
                .Select(ljjd => new
                {
                    ljjd.jdbm,
                    ljjd.jdmc,
                    ljjd.jd_begin,
                    ljjd.jd_end,
                    ljjd.jdbz,
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
        /// 查询单个路径阶段
        /// </summary>
        /// <param name="jdbm"></param>
        /// <returns></returns>
        public async Task<object> GetSingleCpPhase(string jdbm)
        {
            return await Db.Queryable<CP_LJJD_VM>()
                .Where(ljjd => ljjd.jdbm == jdbm)
                .Select(ljjd => new
                {
                    ljjd.jdbm,
                    ljjd.jdmc,
                    ljjd.jd_begin,
                    ljjd.jd_end,
                    ljjd.jdbz,
                    ljjd.ljbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == ljjd.ljbm).Select(it => it.ljmc),
                    ljjd.jdsm,
                    ljjd.cjczy,
                    cjczyxm = SqlFunc.Subqueryable<PU_CZY_VM>().Where(it => it.czybm == ljjd.cjczy).Select(it => it.czyxm),
                    ljjd.cjrq,
                    ljjd.jdhf,
                    ljjd.ls_ljbm,
                    ljjd.ls_jdbm
                }).FirstAsync();
        }
        /// <summary>
        /// 保存路径阶段
        /// </summary>
        /// <param name="ljjd"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> SaveCpPhase(CP_LJJD_VM ljjd, string[] col)
        {
            return await base.InsertOrUpdateAsync(ljjd, col);
        }
        /// <summary>
        /// 删除路径阶段
        /// </summary>
        /// <param name="ljjd"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> DelCpPhase(CP_LJJD_VM ljjd)
        {
            return await base.DeleteEntity(ljjd);
        }
        #endregion

        #region 路径项目
        /// <summary>
        /// 查询单个路径项目
        /// </summary>
        /// <param name="xmbm"></param>
        /// <returns></returns>
        public async Task<CP_LJXM_VM> GetSingleCpProject(string xmbm)
        {
            return await Db.Queryable<CP_LJXM_VM>()
                .Where(ljxm => ljxm.xmbm == xmbm)
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
                })
                .FirstAsync();
        }

        /// <summary>
        /// 保存路径项目
        /// </summary>
        /// <param name="ljxm"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> SaveCpProject(CP_LJXM_VM ljxm, string[] col)
        {
            return await base.InsertOrUpdateAsync(ljxm, col);
        }
        /// <summary>
        /// 删除路径项目
        /// </summary>
        /// <param name="ljxm"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> DelCpProject(CP_LJXM_VM ljxm)
        {
            return await base.DeleteEntity(ljxm);
        }
        #endregion

        #region 治疗方案
        /// <summary>
        /// 查询治疗方案列表
        /// </summary>
        /// <param name="ljbm"></param>
        /// <param name="jdbm"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public async Task<object> GetListCPTherapeuticSchedule(string ljbm, string jdbm, int page, int limit, string filter, RefAsync<int> total)
        {
            var q = Db.Queryable<CP_ZLFA_VM>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter), zlfa => zlfa.fabm.Contains(filter) || zlfa.famc.Contains(filter) || zlfa.fadm.ToUpper().Contains(filter.ToUpper()))
                .WhereIF(!string.IsNullOrWhiteSpace(ljbm), zlfa => zlfa.ljbm == ljbm)
                .WhereIF(!string.IsNullOrWhiteSpace(jdbm), zlfa => zlfa.jdbm == jdbm)
                .OrderBy(zlfa => zlfa.xssx)
                .Select(zlfa => new
                {
                    fabm = zlfa.fabm,
                    ljbm = zlfa.ljbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == zlfa.ljbm).Select(it => it.ljmc),
                    jdbm = zlfa.jdbm,
                    jdmc = SqlFunc.Subqueryable<CP_LJJD_VM>().Where(it => it.jdbm == zlfa.jdbm).Select(it => it.jdmc),
                    famc = zlfa.famc,
                    fadm = zlfa.fadm,
                    xssx = zlfa.xssx,
                    ls_ljbm = zlfa.ls_ljbm,
                    ls_jdbm = zlfa.ls_jdbm,
                    ls_xmbm = zlfa.ls_xmbm,
                    ls_fabm = zlfa.ls_fabm
                });
            return await base.GetListForQueryable(q,page,limit,total);
        }
        /// <summary>
        /// 查询单个治疗方案信息
        /// </summary>
        /// <param name="fabm"></param>
        /// <returns></returns>
        public async Task<object> GetSingleCPTherapeuticSchedule(string fabm)
        {
            return await Db.Queryable<CP_ZLFA_VM>()
                .Where(zlfa => zlfa.fabm == fabm)
                .Select(zlfa => new
                {
                    fabm = zlfa.fabm,
                    ljbm = zlfa.ljbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == zlfa.ljbm).Select(it => it.ljmc),
                    jdbm = zlfa.jdbm,
                    jdmc = SqlFunc.Subqueryable<CP_LJJD_VM>().Where(it => it.jdbm == zlfa.jdbm).Select(it => it.jdmc),
                    famc = zlfa.famc,
                    fadm = zlfa.fadm,
                    xssx = zlfa.xssx,
                    ls_ljbm = zlfa.ls_ljbm,
                    ls_jdbm = zlfa.ls_jdbm,
                    ls_xmbm = zlfa.ls_xmbm,
                    ls_fabm = zlfa.ls_fabm
                }).FirstAsync();
        }
        /// <summary>
        /// 保存治疗方案
        /// </summary>
        /// <param name="zlfa"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> SaveCPTherapeuticSchedule(CP_ZLFA_VM zlfa, string[] col)
        {
            return await base.InsertOrUpdateAsync(zlfa, col);
        }
        /// <summary>
        /// 删除治疗方案
        /// </summary>
        /// <param name="zlfa"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> DelCPTherapeuticSchedule(CP_ZLFA_VM zlfa)
        {
            return await base.DeleteEntity(zlfa);
        }
        #endregion

        #region 路径医嘱
        /// <summary>
        /// 查询单个医嘱
        /// </summary>
        /// <param name="yzbm"></param>
        /// <returns></returns>
        public async Task<object> GetSingleCpMedicalAdvice(string yzbm)
        {
            return await Db.Queryable<CP_YZXM_VM>()
                .Where(yzxm => yzxm.yzbm == yzbm)
                .Select(yzxm => new CP_YZXM_VM
                {
                    ljbm = yzxm.ljbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == yzxm.ljbm).Select(it => it.ljmc),
                    jdbm = yzxm.jdbm,
                    jdmc = SqlFunc.Subqueryable<CP_LJJD_VM>().Where(it => it.ljbm == yzxm.jdbm).Select(it => it.jdmc),
                    ljxmbm = yzxm.ljxmbm,
                    ljxmmc = SqlFunc.Subqueryable<CP_LJXM_VM>().Where(it => it.ljbm == yzxm.ljxmbm).Select(it => it.xmnr),
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
                    gg = SqlFunc.IIF(yzxm.yzpd == "0", "", SqlFunc.Subqueryable<YK_YPZD_VM>().Where(it => it.ypbm == yzxm.yzmxxmbm).Select(it => it.ypgg)),
                }, true)
                .Mapper(async yzxm =>
                {
                    yzxm.yzlxmc = await _Dddw.GetNameForCode("zyyzlx", yzxm.yzlx);
                    yzxm.yzzlmc = await _Dddw.GetNameForCode("yzzl_cq", yzxm.yzzl);
                    yzxm.yzpdmc = await _Dddw.GetNameForCode("CP_Yzpd", yzxm.yzpd);
                    yzxm.bxxmmc = await _Dddw.GetNameForCode("CP_Bxxm", yzxm.bxxm);
                    yzxm.sydwmc = await _Dddw.GetNameForCode("sydw", yzxm.sydw);
                }).FirstAsync();
        }
        /// <summary>
        /// 保存医嘱项目
        /// </summary>
        /// <param name="yzxm"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> SaveCpMedicalAdvice(List<CP_YZXM_VM> yzxm, string[] col)
        {
            return await base.InsertOrUpdateAsync(yzxm, col);
        }
        /// <summary>
        /// 保存医嘱项目
        /// </summary>
        /// <param name="yzxm"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> SaveCpMedicalAdvice(CP_YZXM_VM yzxm, string[] col)
        {
            return await base.InsertOrUpdateAsync(yzxm, col);
        }
        /// <summary>
        /// 保存中药医嘱
        /// </summary>
        /// <param name="yzxm"></param>
        /// <param name="colyz"></param>
        /// <param name="listyzmx"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> SaveCpChineseMedicalAdvice(CP_YZXM_VM yzxm, string[] colyz, List<CP_YZXM_ZYMX_VM> listyzmx, string[] colyzmx)
        {
            return await Db.UseTranAsync(async () =>
            {
                var x = base.Db.Storageable(yzxm).ToStorage();
                await x.AsInsertable.IgnoreColumns(ignoreNullColumn: true).IgnoreColumns(new string[] { "timestamp" }).ExecuteCommandAsync();
                await x.AsUpdateable.UpdateColumns(colyz).ExecuteCommandAsync();

                foreach (var vm in listyzmx)
                {
                    var x1 = base.Db.Storageable(vm).ToStorage();
                    await x1.AsInsertable.IgnoreColumns(ignoreNullColumn: true).IgnoreColumns(new string[] { "timestamp" }).ExecuteCommandAsync();
                    await x1.AsUpdateable.UpdateColumns(colyzmx).IgnoreColumns(ignoreAllNullColumns: true).IgnoreColumns(new string[] { "timestamp" }).ExecuteCommandAsync();
                }
            });
        }
        /// <summary>
        /// 删除医嘱项目
        /// </summary>
        /// <param name="yzxm"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> DelCpMedicalAdvice(CP_YZXM_VM yzxm)
        {
            return await base.DeleteEntity(yzxm);
        }
        /// <summary>
        /// 删除医嘱项目 含中药明细
        /// </summary>
        /// <param name="yzxm"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> DelCpMedicalAdvice(CP_YZXM_VM yzxm, List<CP_YZXM_ZYMX_VM> listzymx)
        {
            return await Db.UseTranAsync(async () =>
            {
                await Db.Deleteable(yzxm).ExecuteCommandAsync();
                await Db.Deleteable(listzymx).ExecuteCommandAsync();
            });
        }
        /// <summary>
        /// 删除中药医嘱项目
        /// </summary>
        /// <param name="yzxm"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> DelCpChineseMedicalAdvice(CP_YZXM_ZYMX_VM yzxm)
        {
            return await base.DeleteEntity(yzxm);
        }
        #endregion

        #region 路径查询

        #endregion

        #region 路径表单

        #endregion
    }
}
