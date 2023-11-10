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
    [CheckPower(Power.DeptView)]
    public class DeptUserModel : BaseAdminModel
    {
        public IEnumerable<Dept> Depts { get; set; }
        public IEnumerable<User> Users { get; set; }
        public bool PowerCoreDeptView { get; set; }
        public bool PowerCoreDeptUserNew { get; set; }
        public bool PowerCoreDeptUserDelete { get; set; }
        public string Grid1SelectedRowID { get; set; }
        public PagingInfo Grid2PagingInfo { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            PowerCoreDeptView = CheckPower(Power.DeptView);
            PowerCoreDeptUserNew = CheckPower(Power.DeptEdit);
            PowerCoreDeptUserDelete = CheckPower(Power.DeptEdit);

            // 表格1
            Depts = DeptHelper.Depts;
            if (DeptHelper.Depts.Count == 0)
                return Content("请先添加部门！");

            var grid1SelectedRowID = DeptHelper.Depts[0].ID;
            Grid1SelectedRowID = grid1SelectedRowID.ToString();
            Users = await DeptUser_LoadDataAsync(grid1SelectedRowID);
            return Page();
        }

        private async Task<IEnumerable<User>> DeptUser_LoadDataAsync(long grid1SelectedRowID)
        {
            var grid2PagingInfo = new PagingInfo("Name", false);
            Grid2PagingInfo = grid2PagingInfo;
            return await DeptUser_GetDataAsync(grid2PagingInfo, grid1SelectedRowID, String.Empty);
        }

        private async Task<IEnumerable<User>> DeptUser_GetDataAsync(PagingInfo pagingInfo, long deptID, string ttbSearchMessage)
        {
            IQueryable<User> q = DB.Users;
            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
                q = q.Where(u => u.Name.Contains(searchText) || u.ChineseName.Contains(searchText) || u.EnglishName.Contains(searchText));
            q = q.Where(u => u.Name != "admin");
            q = q.Where(u => u.Dept.ID == deptID);  // 过滤选中部门下的所有用户

            pagingInfo.RecordCount = await q.CountAsync();
            q = SortAndPage<User>(q, pagingInfo);
            return await q.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeptUser_Grid2_DoPostBackAsync(string[] Grid2_fields, int Grid2_pageIndex, string Grid2_sortField, string Grid2_sortDirection,
            string ttbSearchMessage, int ddlGridPageSize, string actionType, int selectedDeptId, int[] deletedUserIDs)
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
            else if (actionType == "delete")
            {
                // 在操作之前进行权限检查
                if (!CheckPower(Power.DeptEdit))
                {
                    Auth.CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }

                Dept role = await DB.Depts
                    .Include(r => r.Users)
                    .Where(r => r.ID == selectedDeptId)
                    .FirstOrDefaultAsync();

                foreach (int userID in deletedUserIDs)
                {
                    User user = role.Users.Where(u => u.ID == userID).FirstOrDefault();
                    if (user != null)
                        role.Users.Remove(user);
                }
                await DB.SaveChangesAsync();
            }

            var grid2UI = UIHelper.Grid("Grid2");
            var pagingInfo = new PagingInfo
            {
                SortField = Grid2_sortField,
                SortDirection = Grid2_sortDirection,
                PageIndex = Grid2_pageIndex,
                PageSize = ddlGridPageSize
            };
            var deptUsers = await DeptUser_GetDataAsync(pagingInfo, selectedDeptId, ttbSearchMessage);
            grid2UI.RecordCount(pagingInfo.RecordCount);
            if (actionType == "changeGridPageSize")
                grid2UI.PageSize(ddlGridPageSize);
            grid2UI.DataSource(deptUsers, Grid2_fields);
            return UIHelper.Result();
        }
    }
}