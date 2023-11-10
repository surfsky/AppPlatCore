using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Components;
using App.DAL;

using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace App.Pages.Admin
{
    [CheckPower(Power.MonitorOnline)]
    public class OnlinesModel : BaseAdminModel
    {
        public IEnumerable<Online> Onlines { get; set; }
        public PagingInfo PagingInfo { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var pagingInfo = new PagingInfo("UpdateTime", false);
            PagingInfo = pagingInfo;
            Onlines = await Online_GetDataAsync(pagingInfo, String.Empty);
            return Page();
        }


        private async Task<IEnumerable<Online>> Online_GetDataAsync(PagingInfo pagingInfo, string ttbSearchMessage)
        {
            IQueryable<Online> q = DB.Onlines.Include(o => o.User);
            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
                q = q.Where(o => o.User.Name.Contains(searchText));
            DateTime lastD = DateTime.Now.AddHours(-2);
            q = q.Where(o => o.UpdateTime > lastD);   // 2个小时内活跃的用户

            pagingInfo.RecordCount = await q.CountAsync();
            q = SortAndPage<Online>(q, pagingInfo);
            return await q.ToListAsync();
        }

        public async Task<IActionResult> OnPostOnline_DoPostBackAsync(string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
            string ttbSearchMessage, int ddlGridPageSize, string actionType)
        {
            var ttbSearchMessageUI = UIHelper.TwinTriggerBox("ttbSearchMessage");
            if (actionType == "trigger1")
            {
                ttbSearchMessageUI.Text(String.Empty);
                ttbSearchMessageUI.ShowTrigger1(false);

                // 清空传入的搜索值
                ttbSearchMessage = String.Empty;
            }
            else if (actionType == "trigger2")
            {
                ttbSearchMessageUI.ShowTrigger1(true);
            }


            var grid1UI = UIHelper.Grid("Grid1");
            var pagingInfo = new PagingInfo
            {
                SortField = Grid1_sortField,
                SortDirection = Grid1_sortDirection,
                PageIndex = Grid1_pageIndex,
                PageSize = ddlGridPageSize
            };
            grid1UI.DataSource(await Online_GetDataAsync(pagingInfo, ttbSearchMessage), Grid1_fields);
            grid1UI.RecordCount(pagingInfo.RecordCount);
            return UIHelper.Result();
        }


    }
}