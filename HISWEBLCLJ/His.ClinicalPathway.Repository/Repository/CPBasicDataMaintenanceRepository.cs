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
    /// 路径名称维护
    /// </summary>
    public class CPBasicDataMaintenanceRepository : HisBaseService, ICPBasicDataMaintenanceRepository
    {        
        #region 路径名称
        /// <summary>
        /// 查询路径名称列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public async Task<object> GetListCpName(int page, int limit, string filter, RefAsync<int> total)
        {
            var q = Db.Queryable<CP_LJMC_VM>()
                        .OrderBy(mc => mc.xssx)
                        .WhereIF(!string.IsNullOrWhiteSpace(filter), mc => mc.ljbm.Contains(filter) || mc.ljmc.Contains(filter) || mc.pydm.ToUpper().Contains(filter.ToUpper()))
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
        /// 获取单个路径名称信息
        /// </summary>
        /// <param name="ljbm"></param>
        /// <returns></returns>
        public async Task<object> GetSingleCpName(string ljbm)
        {
            return await Db.Queryable<CP_LJMC_VM>()
                        .Where(mc => mc.ljbm == ljbm)
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
                            syksmc = SqlFunc.Subqueryable<PU_KS_VM>().Where(it => it.ksbm == mc.syks).Select(it => it.ksmc),
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
                        }).FirstAsync();
        }
        /// <summary>
        /// 保存单个路径名称
        /// </summary>
        /// <param name="ljmc"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> SaveCpName(CP_LJMC_VM ljmc, string[] col)
        {
            return await base.InsertOrUpdateAsync(ljmc, col);
        }
        /// <summary>
        /// 删除单个路径名称
        /// </summary>
        /// <param name="ljmc"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> DelCpName(CP_LJMC_VM ljmc)
        {
            return await base.DeleteEntity(ljmc);
        }
        #endregion

        #region 科室路径
        /// <summary>
        /// 查询科室路径列表
        /// </summary>
        /// <param name="ksbm"></param>
        /// <param name="ljbm"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public async Task<object> GetListDeptCP(string ksbm, string ljbm, int page, int limit, string filter, RefAsync<int> total)
        {
            var q = Db.Queryable<CP_KSLJ_VM, CP_LJMC_VM, PU_KS_VM>((kslj, ljmc, ks) => new JoinQueryInfos(
                JoinType.Inner, kslj.ljbm == ljmc.ljbm,
                JoinType.Inner, kslj.ksbm == ks.ksbm))
                .WhereIF(!string.IsNullOrWhiteSpace(filter), (kslj, ljmc, ks) => ljmc.ljbm.Contains(filter) || ljmc.ljmc.Contains(filter) || ljmc.pydm.ToUpper().Contains(filter.ToUpper()) || ks.ksbm.Contains(filter) || ks.ksmc.Contains(filter) || ks.ksdm.ToUpper().Contains(filter.ToUpper()))
                .WhereIF(!string.IsNullOrWhiteSpace(ksbm), kslj => kslj.ksbm == ksbm)
                .WhereIF(!string.IsNullOrWhiteSpace(ljbm), kslj => kslj.ljbm == ljbm)
                .Select((kslj, ljmc, ks) => new
                {
                    ljbm = kslj.ljbm,
                    ljmc = ljmc.ljmc,
                    ksbm = kslj.ksbm,
                    ksmc = ks.ksmc
                });
            return await base.GetListForQueryable(q,page,limit,total);
        }
        /// <summary>
        /// 查询单个科室路径信息
        /// </summary>
        /// <param name="ljbm"></param>
        /// <param name="ksbm"></param>
        /// <returns></returns>
        public async Task<object> GetSingleDeptCp(string ljbm, string ksbm)
        {
            return await Db.Queryable<CP_KSLJ_VM>()
                .Where(kslj => kslj.ljbm == ljbm && kslj.ksbm == ksbm)
                .Select(kslj => new
                {
                    kslj.ljbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == kslj.ljbm).Select(it => it.ljmc),
                    kslj.ksbm,
                    ksmc = SqlFunc.Subqueryable<PU_KS_VM>().Where(it => it.ksbm == kslj.ksbm).Select(it => it.ksmc)
                }).FirstAsync();
        }
        /// <summary>
        /// 保存单个科室路径信息
        /// </summary>
        /// <param name="ksljold"></param>
        /// <param name="ksljnew"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> SaveDeptCp(CP_KSLJ_VM ksljold, CP_KSLJ_VM ksljnew)
        {
            return await Db.UseTranAsync(async () =>
            {
                await Db.Deleteable(ksljold).ExecuteCommandAsync();
                await Db.Insertable(ksljnew).ExecuteCommandAsync();
            });
        }
        /// <summary>
        /// 删除单个科室路径信息
        /// </summary>
        /// <param name="ksjl"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> DelDeptCp(CP_KSLJ_VM ksjl)
        {
            return await base.DeleteEntity(ksjl);
        }
        #endregion

        #region 路径病种
        /// <summary>
        /// 查询路径病种列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public async Task<object> GetListCpIcd(string ljbm, string jbbm, int page, int limit, string filter, RefAsync<int> total)
        {
            var q = Db.Queryable<CP_LJICD_VM, CP_LJMC_VM, PU_JBBM_VM>((ljicd, ljmc, jb) => new JoinQueryInfos(
                JoinType.Inner, ljicd.ljbm == ljmc.ljbm,
                JoinType.Inner, ljicd.jbbm == jb.jbbm))
                .WhereIF(!string.IsNullOrWhiteSpace(filter), (ljicd, ljmc, jb) => ljmc.ljbm.Contains(filter) || ljmc.ljmc.Contains(filter) || ljmc.pydm.ToUpper().Contains(filter) || jb.jbbm.Contains(filter) || jb.jbmc.Contains(filter) || jb.pydm.ToUpper().Contains(filter.ToUpper()))
                .WhereIF(!string.IsNullOrWhiteSpace(ljbm), (ljicd, ljmc, jb) => ljicd.ljbm == ljbm)
                .WhereIF(!string.IsNullOrWhiteSpace(jbbm), (ljicd, ljmc, jb) => ljicd.jbbm == jbbm)
                .Select((ljicd, ljmc, jb) => new
                {
                    ljicd.ljbm,
                    ljmc.ljmc,
                    ljicd.jbbm,
                    jb.jbmc
                });
            return await base.GetListForQueryable(q,page,limit,total);
        }
        /// <summary>
        /// 查询单个路径病种信息
        /// </summary>
        /// <param name="ljbm"></param>
        /// <param name="jbbm"></param>
        /// <returns></returns>
        public async Task<object> GetSingleCpIcd(string ljbm, string jbbm)
        {
            return await Db.Queryable<CP_LJICD_VM>()
                .Where(ljicd => ljicd.ljbm == ljbm && ljicd.jbbm == jbbm)
                .Select(ljicd => new
                {
                    ljicd.ljbm,
                    ljmc = SqlFunc.Subqueryable<CP_LJMC_VM>().Where(it => it.ljbm == ljicd.ljbm).Select(it => it.ljmc),
                    ljicd.jbbm,
                    jbmc = SqlFunc.Subqueryable<PU_JBBM_VM>().Where(it => it.jbbm == ljicd.jbbm).Select(it => it.jbmc)
                }).FirstAsync();
        }
        /// <summary>
        /// 保存单个科室路径信息
        /// </summary>
        /// <param name="ljicdold"></param>
        /// <param name="ljicdnew"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> SaveCpIcd(CP_LJICD_VM ljicdold, CP_LJICD_VM ljicdnew)
        {
            return await Db.UseTranAsync(async () =>
            {
                await Db.Deleteable(ljicdold).ExecuteCommandAsync();
                await Db.Insertable(ljicdnew).ExecuteCommandAsync();
            });
        }
        /// <summary>
        /// 删除单个科室路径信息
        /// </summary>
        /// <param name="ljicd"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> DelCpIcd(CP_LJICD_VM ljicd)
        {
            return await base.DeleteEntity(ljicd);
        }
        #endregion

        #region 执行大类
        /// <summary>
        /// 获取执行大类列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public async Task<List<CP_ZXDL_VM>> GetListExecuteLargeClass(int page, int limit, string filter, RefAsync<int> total)
        {
            var q = Db.Queryable<CP_ZXDL_VM>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter), dl => dl.dlbm.Contains(filter) || dl.dlmc.Contains(filter))
                .OrderBy(dl => dl.xh)
                .Select(dl => new CP_ZXDL_VM
                {
                    dlbm = dl.dlbm,
                    dlmc = dl.dlmc,
                    xh = dl.xh
                });
            return await base.GetListForQueryable(q,page,limit,total);
        }
        /// <summary>
        /// 获取单个执行大类信息
        /// </summary>
        /// <param name="dlbm"></param>
        /// <returns></returns>
        public async Task<CP_ZXDL_VM> GetSingleExecuteLargeClass(string dlbm)
        {
            return await Db.Queryable<CP_ZXDL_VM>()
                .Where(dl => dl.dlbm == dlbm)
                .Select(dl => new CP_ZXDL_VM
                {
                    dlbm = dl.dlbm,
                    dlmc = dl.dlmc,
                    xh = dl.xh
                }).FirstAsync();
        }
        /// <summary>
        /// 保存执行大类
        /// </summary>
        /// <param name="dl"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> SaveExecuteLargeClass(CP_ZXDL_VM dl, string[] col)
        {
            return await base.InsertOrUpdateAsync(dl, col);
        }
        /// <summary>
        /// 删除执行大类
        /// </summary>
        /// <param name="dl"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> DelExecuteLargeClass(CP_ZXDL_VM dl)
        {
            return await base.DeleteEntity(dl);
        }
        #endregion

        #region 执行细类
        /// <summary>
        /// 获取执行细类列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public async Task<List<CP_ZXXL_VM>> GetListExecuteSubclass(int page, int limit, string filter, RefAsync<int> total)
        {
            var q = Db.Queryable<CP_ZXXL_VM>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter), xl => xl.xlbm.Contains(filter) || xl.xlmc.Contains(filter))
                .OrderBy(xl => xl.xlbm)
                .Select(xl => new CP_ZXXL_VM
                {
                    xlbm = xl.xlbm,
                    xlmc = xl.xlmc
                });
            return await base.GetListForQueryable(q, page, limit, total);
        }
        /// <summary>
        /// 获取单个执行细类信息
        /// </summary>
        /// <param name="xlbm"></param>
        /// <returns></returns>
        public async Task<CP_ZXXL_VM> GetSingleExecuteSubclass(string xlbm)
        {
            return await Db.Queryable<CP_ZXXL_VM>()
                .Where(xl => xl.xlbm == xlbm)
                .Select(xl => new CP_ZXXL_VM
                {
                    xlbm = xl.xlbm,
                    xlmc = xl.xlmc
                }).FirstAsync();
        }
        /// <summary>
        /// 保存执行细类
        /// </summary>
        /// <param name="xl"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> SaveExecuteSubclass(CP_ZXXL_VM xl, string[] col)
        {
            return await base.InsertOrUpdateAsync(xl, col);
        }
        /// <summary>
        /// 删除执行细类
        /// </summary>
        /// <param name="xl"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public async Task<DbResult<bool>> DelExecuteSubclass(CP_ZXXL_VM xl)
        {
            return await base.DeleteEntity(xl);
        }
        #endregion

        
    }
}
