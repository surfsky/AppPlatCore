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
    [CheckPower("CoreDeptEdit")]
    public class DeptEditModel : BaseAdminModel
    {
        public IEnumerable<Dept> Depts { get; set; }

        [BindProperty]
        public Dept Dept { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Dept = await DB.Depts.Where(m => m.ID == id).AsNoTracking().FirstOrDefaultAsync();
            if (Dept == null)
                return Content("无效参数！");

            Depts = UI.ResolveDDL<Dept>(DeptHelper.Depts, id).ToArray();
            return Page();
        }

        public async Task<IActionResult> OnPostDeptEdit_btnSaveClose_ClickAsync()
        {
            if (ModelState.IsValid)
            {
                // 下拉列表的顶级节点值为-1
                if (Dept.ParentID == -1)
                    Dept.ParentID = null;

                DB.Entry(Dept).State = EntityState.Modified;
                await DB.SaveChangesAsync();
                DeptHelper.Reload();

                // 关闭本窗体（触发窗体的关闭事件）
                //ActiveWindow.HideRefresh(); // 刷新整个页面
                ActiveWindow.HidePostBack();  // 刷新表格
            }

            return UIHelper.Result();
        }
    }
}