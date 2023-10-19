using System;
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
    [CheckPower(Name = "CoreTitleUserNew")]
    public class TitleUserNewModel : BaseAdminModel
    {
        public Title Title { get; set; }
        public IEnumerable<User> Users { get; set; }

        public PagingInfoViewModel PagingInfo { get; set; }


        public async Task<IActionResult> OnGetAsync(int titleID)
        {
            Title = await DB.Titles.Where(t => t.ID == titleID).AsNoTracking().FirstOrDefaultAsync();
            if (Title == null)
                return Content("无效参数！");

            //Users = await TitleUserNew_LoadDataAsync(titleID);
            var pagingInfo = new PagingInfoViewModel
            {
                SortField = "Name",
                SortDirection = "DESC",
                PageIndex = 0,
                PageSize = SiteConfig.Instance.PageSize
            };
            PagingInfo = pagingInfo;
            this.Users =  await TitleUserNew_GetDataAsync(pagingInfo, titleID, String.Empty);
            return Page();
        }

        private async Task<IEnumerable<User>> TitleUserNew_GetDataAsync(PagingInfoViewModel pagingInfo, int titleID, string ttbSearchMessage)
        {
            IQueryable<User> q = DB.Users;
            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
            {
                q = q.Where(u => u.Name.Contains(searchText) || u.ChineseName.Contains(searchText) || u.EnglishName.Contains(searchText));
            }
            q = q.Where(u => u.Name != "admin");
            q = q.Where(u => u.TitleUsers.All(r => r.TitleID != titleID));  // 排除已经属于本职称的用户

            pagingInfo.RecordCount = await q.CountAsync();  // 获取总记录数（在添加条件之后，排序和分页之前）
            q = SortAndPage<User>(q, pagingInfo);  // 排列和数据库分页
            return await q.ToListAsync();
        }

        public async Task<IActionResult> OnPostTitleUserNew_DoPostBackAsync(
            string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
            string ttbSearchMessage, int ddlGridPageSize, string actionType, int titleID)
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

            var grid1UI = UIHelper.Grid("Grid1");
            var pagingInfo = new PagingInfoViewModel
            {
                SortField = Grid1_sortField,
                SortDirection = Grid1_sortDirection,
                PageIndex = Grid1_pageIndex,
                PageSize = ddlGridPageSize
            };


            //grid1UI.DataSource(await TitleUserNew_GetDataAsync(pagingInfo, titleID, ttbSearchMessage), Grid1_fields, clearSelection: false);
            //grid1UI.RecordCount(pagingInfo.RecordCount);
            // 1. 设置总项数
            // 2. 设置每页显示项数
            // 3.设置分页数据
            var titleUsers = await TitleUserNew_GetDataAsync(pagingInfo, titleID, ttbSearchMessage);
            grid1UI.RecordCount(pagingInfo.RecordCount);
            if (actionType == "changeGridPageSize")
            {
                grid1UI.PageSize(ddlGridPageSize);
            }
            grid1UI.DataSource(titleUsers, Grid1_fields, clearSelection: false);
            return UIHelper.Result();
        }

        public async Task<IActionResult> OnPostTitleUserNew_btnSaveClose_ClickAsync(int titleID, int[] selectedRowIDs)
        {
            AddEntities2<TitleUser>(titleID, selectedRowIDs);
            await DB.SaveChangesAsync();

            // 关闭本窗体（触发窗体的关闭事件）
            ActiveWindow.HidePostBack();
            return UIHelper.Result();
        }
    }
}