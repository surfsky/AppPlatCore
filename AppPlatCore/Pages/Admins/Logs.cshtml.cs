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
    [CheckPower(Power.MonitorLog)]
    public class LogsModel : BaseAdminModel
    {
        public IEnumerable<Log> Logs { get; set; }
        public PagingInfo PagingInfo { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var pagingInfo = new PagingInfo("CreateDt", false);
            PagingInfo = pagingInfo;
            Logs = await GetDataAsync(pagingInfo, String.Empty);
            return Page();
        }


        private async Task<IEnumerable<Log>> GetDataAsync(PagingInfo pagingInfo, string ttbSearchMessage)
        {
            IQueryable<Log> q = Log.Set;
            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
                q = q.Where(o => o.Operator.Contains(searchText));

            pagingInfo.RecordCount = await q.CountAsync();
            q = SortAndPage<Log>(q, pagingInfo);
            return await q.ToListAsync();
        }

        public async Task<IActionResult> OnPostLogs_DoPostBackAsync(
            string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
            string ttbSearchMessage, int ddlGridPageSize, string actionType)
        {
            var ttbSearchMessageUI = UIHelper.TwinTriggerBox("ttbSearchMessage");
            if (actionType == "trigger1")
            {
                ttbSearchMessageUI.Text(String.Empty);
                ttbSearchMessageUI.ShowTrigger1(false);
                ttbSearchMessage = String.Empty;
            }
            else if (actionType == "trigger2")
            {
                ttbSearchMessageUI.ShowTrigger1(true);
            }

            //
            var grid = UIHelper.Grid("Grid1");
            var pagingInfo = new PagingInfo
            {
                SortField = Grid1_sortField,
                SortDirection = Grid1_sortDirection,
                PageIndex = Grid1_pageIndex,
                PageSize = ddlGridPageSize
            };
            grid.DataSource(await GetDataAsync(pagingInfo, ttbSearchMessage), Grid1_fields);
            grid.RecordCount(pagingInfo.RecordCount);
            return UIHelper.Result();
        }


    }
}