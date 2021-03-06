﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Models;


using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace App.Pages.Admin
{
    [CheckPower(Name = "CoreRoleUserNew")]
    public class RoleUserNewModel : BaseAdminModel
    {
        public Role Role { get; set; }
        public IEnumerable<User> Users { get; set; }

        public PagingInfoViewModel PagingInfo { get; set; }


        public async Task<IActionResult> OnGetAsync(int roleID)
        {
            Role = await DB.Roles
                .Where(r => r.ID == roleID).AsNoTracking().FirstOrDefaultAsync();
				
            if (Role == null)
            {
                return Content("无效参数！");
            }

            Users = await RoleUserNew_LoadDataAsync(roleID);

            return Page();
        }

        private async Task<IEnumerable<User>> RoleUserNew_LoadDataAsync(int roleID)
        {
            var pagingInfo = new PagingInfoViewModel
            {
                SortField = "Name",
                SortDirection = "DESC",
                PageIndex = 0,
                PageSize = ConfigHelper.PageSize
            };
            PagingInfo = pagingInfo;

            return await RoleUserNew_GetDataAsync(pagingInfo, roleID, String.Empty);
        }

        private async Task<IEnumerable<User>> RoleUserNew_GetDataAsync(PagingInfoViewModel pagingInfo, int roleID, string ttbSearchMessage)
        {
            IQueryable<User> q = DB.Users;

            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
            {
                q = q.Where(u => u.Name.Contains(searchText) || u.ChineseName.Contains(searchText) || u.EnglishName.Contains(searchText));
            }

            q = q.Where(u => u.Name != "admin");

            // 排除已经属于本角色的用户
            q = q.Where(u => u.RoleUsers.All(r => r.RoleID != roleID));


            // 获取总记录数（在添加条件之后，排序和分页之前）
            pagingInfo.RecordCount = await q.CountAsync();

            // 排列和数据库分页
            q = SortAndPage<User>(q, pagingInfo);

            return await q.ToListAsync();
        }

        public async Task<IActionResult> OnPostRoleUserNew_DoPostBackAsync(string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
            string ttbSearchMessage, int ddlGridPageSize, string actionType, int roleID)
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
            var pagingInfo = new PagingInfoViewModel
            {
                SortField = Grid1_sortField,
                SortDirection = Grid1_sortDirection,
                PageIndex = Grid1_pageIndex,
                PageSize = ddlGridPageSize
            };
            //grid1UI.DataSource(await RoleUserNew_GetDataAsync(pagingInfo, roleID, ttbSearchMessage), Grid1_fields, clearSelection: false);
            //grid1UI.RecordCount(pagingInfo.RecordCount);

            var roleUsers = await RoleUserNew_GetDataAsync(pagingInfo, roleID, ttbSearchMessage);
            // 1. 设置总项数
            grid1UI.RecordCount(pagingInfo.RecordCount);
            // 2. 设置每页显示项数
            if (actionType == "changeGridPageSize")
            {
                grid1UI.PageSize(ddlGridPageSize);
            }
            // 3.设置分页数据
            grid1UI.DataSource(roleUsers, Grid1_fields, clearSelection: false);


            return UIHelper.Result();
        }

        public async Task<IActionResult> OnPostRoleUserNew_btnSaveClose_ClickAsync(int roleID, int[] selectedRowIDs)
        {
            AddEntities2<RoleUser>(roleID, selectedRowIDs);

            await DB.SaveChangesAsync();

            // 关闭本窗体（触发窗体的关闭事件）
            ActiveWindow.HidePostBack();

            return UIHelper.Result();
        }
    }
}