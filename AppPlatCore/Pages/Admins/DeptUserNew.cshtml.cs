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
    [CheckPower(Power.DeptEdit)]
    public class DeptUserNewModel : BaseAdminModel
    {
        public Dept Dept { get; set; }
        public IEnumerable<User> Users { get; set; }
        public PagingInfo PagingInfo { get; set; }

        public async Task<IActionResult> OnGetAsync(int deptID)
        {
            Dept = await DB.Depts
                .Where(d => d.ID == deptID).AsNoTracking().FirstOrDefaultAsync();
            if (Dept == null)
                return Content("无效参数！");

            Users = await DeptUserNew_LoadDataAsync(deptID);
            return Page();
        }

        private async Task<IEnumerable<User>> DeptUserNew_LoadDataAsync(int deptID)
        {
            var pagingInfo = new PagingInfo("Name", false);
            PagingInfo = pagingInfo;
            return await DeptUserNew_GetDataAsync(pagingInfo, deptID, String.Empty);
        }

        private async Task<IEnumerable<User>> DeptUserNew_GetDataAsync(PagingInfo pagingInfo, int deptID, string ttbSearchMessage)
        {
            IQueryable<User> q = DB.Users;
            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
                q = q.Where(u => u.Name.Contains(searchText) || u.ChineseName.Contains(searchText) || u.EnglishName.Contains(searchText));
            q = q.Where(u => u.Name != "admin");
            q = q.Where(u => u.Dept == null);  // 排除所有已经属于某个部门的用户

            pagingInfo.RecordCount = await q.CountAsync();
            q = SortAndPage<User>(q, pagingInfo);
            return await q.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeptUserNew_DoPostBackAsync(string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
            string ttbSearchMessage, int ddlGridPageSize, string actionType, int deptID)
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


            //grid1UI.DataSource(await DeptUserNew_GetDataAsync(pagingInfo, deptID, ttbSearchMessage), Grid1_fields, clearSelection: false);
            //grid1UI.RecordCount(pagingInfo.RecordCount);
            var deptUsers = await DeptUserNew_GetDataAsync(pagingInfo, deptID, ttbSearchMessage);
            grid1UI.RecordCount(pagingInfo.RecordCount);
            if (actionType == "changeGridPageSize")
                grid1UI.PageSize(ddlGridPageSize);
            grid1UI.DataSource(deptUsers, Grid1_fields, clearSelection: false);
            return UIHelper.Result();
        }

        public async Task<IActionResult> OnPostDeptUserNew_btnSaveClose_ClickAsync(int deptID, List<long> selectedRowIDs)
        {
            var users = await DB.Users
                 .Where(u => selectedRowIDs.Contains(u.ID))
                 .ToListAsync();

            users.ForEach(u => u.DeptID = deptID);
            await DB.SaveChangesAsync();
            ActiveWindow.HidePostBack();
            return UIHelper.Result();
        }
    }
}