using App.Models;


using FineUICore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Pages.Admin
{
    [CheckPower("CoreUserView")]
    public class UserListModel : BaseAdminModel
    {
        public IEnumerable<User> Users { get; set; }
        public PagingInfoViewModel PagingInfo { get; set; }

        public bool PowerCoreUserNew { get; set; }
        public bool PowerCoreUserEdit { get; set; }
        public bool PowerCoreUserDelete { get; set; }
        public bool PowerCoreUserChangePassword { get; set; }

        public async Task OnGetAsync()
        {
            PowerCoreUserNew = CheckPower("CoreUserNew");
            PowerCoreUserEdit = CheckPower("CoreUserEdit");
            PowerCoreUserDelete = CheckPower("CoreUserDelete");
            PowerCoreUserChangePassword = CheckPower("CoreUserChangePassword");

            var pagingInfo = new PagingInfoViewModel
            {
                SortField = "Name",
                SortDirection = "DESC",
                PageIndex = 0,
                PageSize = SiteConfig.Instance.PageSize
            };
            PagingInfo = pagingInfo;
            Users = await UserList_GetDataAsync(pagingInfo, String.Empty, "all");
        }


        private async Task<IEnumerable<User>> UserList_GetDataAsync(PagingInfoViewModel pagingInfo, string ttbSearchMessage, string rblEnableStatus)
        {
            IQueryable<User> q = DB.Users;
            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
                q = q.Where(u => u.Name.Contains(searchText) || u.ChineseName.Contains(searchText) || u.EnglishName.Contains(searchText));
            if (GetIdentityName() != "admin")
                q = q.Where(u => u.Name != "admin");
            if (rblEnableStatus != "all")
                q = q.Where(u => u.Enabled == (rblEnableStatus == "enabled" ? true : false));

            // 分页和排序
            pagingInfo.RecordCount = await q.CountAsync();
            q = SortAndPage<User>(q, pagingInfo);
            return await q.ToListAsync();
        }

        public async Task<IActionResult> OnPostUserList_DoPostBackAsync(
            string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
            string ttbSearchMessage, string rblEnableStatus, int ddlGridPageSize, string actionType, int[] deletedRowIDs)
        {
            List<int> ids = new List<int>();
            if (deletedRowIDs != null)
            {
                ids.AddRange(deletedRowIDs);
            }

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
            else if (actionType == "delete")
            {
                // 在操作之前进行权限检查
                if (!CheckPower("CoreUserDelete"))
                {
                    Auth.CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }

                DB.Users.Where(u => ids.Contains(u.ID)).ToList().ForEach(u => DB.Users.Remove(u));
                await DB.SaveChangesAsync();
            }
            else if (actionType == "enable")
            {
                // 在操作之前进行权限检查
                if (!CheckPower("CoreUserEdit"))
                {
                    Auth.CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }

                DB.Users.Where(u => ids.Contains(u.ID)).ToList().ForEach(u => u.Enabled = true);
                await DB.SaveChangesAsync();
            }
            else if (actionType == "disable")
            {
                // 在操作之前进行权限检查
                if (!CheckPower("CoreUserEdit"))
                {
                    Auth.CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }

                DB.Users.Where(u => ids.Contains(u.ID)).ToList().ForEach(u => u.Enabled = false);
                await DB.SaveChangesAsync();
            }


            var grid1UI = UIHelper.Grid("Grid1");
            var pagingInfo = new PagingInfoViewModel
            {
                SortField = Grid1_sortField,
                SortDirection = Grid1_sortDirection,
                PageIndex = Grid1_pageIndex,
                PageSize = ddlGridPageSize
            };

            // 获取分页数据
            var users = await UserList_GetDataAsync(pagingInfo, ttbSearchMessage, rblEnableStatus);
            grid1UI.RecordCount(pagingInfo.RecordCount);
            if (actionType == "changeGridPageSize")
                grid1UI.PageSize(ddlGridPageSize);
            grid1UI.DataSource(users, Grid1_fields);
            return UIHelper.Result();
        }
    }
}