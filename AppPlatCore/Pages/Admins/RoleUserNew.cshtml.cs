using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Components;
using App.DAL;
using App.Utils;
using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace App.Pages.Admin
{
    [CheckPower(Power.RoleUserEdit)]
    public class RoleUserNewModel : BaseAdminModel
    {
        public Role Role { get; set; }
        public IEnumerable<User> Users { get; set; }
        public PagingInfo PagingInfo { get; set; }


        public async Task<IActionResult> OnGetAsync(int roleID)
        {
            Role = await DB.Roles.Where(r => r.ID == roleID).AsNoTracking().FirstOrDefaultAsync();
            if (Role == null)
                return Content("无效参数！");

            this.PagingInfo = new PagingInfo("Name", false);
            this.Users = await GetDataAsync(PagingInfo, roleID, "");
            return Page();
        }

        /// <summary>查找</summary>
        private async Task<IEnumerable<User>> GetDataAsync(PagingInfo pagingInfo, int roleID, string searchText)
        {
            searchText = searchText?.Trim();
            IQueryable<User> q = DB.Users;
            if (searchText.IsNotEmpty())
                q = q.Where(u => u.Name.Contains(searchText) || u.ChineseName.Contains(searchText) || u.EnglishName.Contains(searchText));
            q = q.Where(u => u.Name != "admin");
            q = q.Where(u => u.RoleUsers.All(r => r.RoleID != roleID));  // 排除已经属于本角色的用户

            pagingInfo.RecordCount = await q.CountAsync();
            q = SortAndPage<User>(q, pagingInfo);
            return await q.ToListAsync();
        }

        /// <summary>查找分页等操作</summary>
        public async Task<IActionResult> OnPostRoleUserNew_DoPostBackAsync(
            string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
            string ttbSearchMessage, int ddlGridPageSize, string actionType, int roleID)
        {
            // SearchBox
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

            // Search
            var grid1UI = UIHelper.Grid("Grid1");
            var pagingInfo = new PagingInfo
            {
                SortField = Grid1_sortField,
                SortDirection = Grid1_sortDirection,
                PageIndex = Grid1_pageIndex,
                PageSize = ddlGridPageSize
            };
            var items = await GetDataAsync(pagingInfo, roleID, ttbSearchMessage);
            grid1UI.RecordCount(pagingInfo.RecordCount);
            if (actionType == "changeGridPageSize")
                grid1UI.PageSize(ddlGridPageSize);
            grid1UI.DataSource(items, Grid1_fields, clearSelection: false);
            return UIHelper.Result();
        }

        /// <summary>设置角色拥有的用户列表</summary>
        public async Task<IActionResult> OnPostRoleUserNew_btnSaveClose_ClickAsync(long roleID, long[] selectedRowIDs)
        {
            RoleUser.SetRoleUsers(roleID, selectedRowIDs.ToList());
            ActiveWindow.HidePostBack();
            return UIHelper.Result();
        }


    }
}