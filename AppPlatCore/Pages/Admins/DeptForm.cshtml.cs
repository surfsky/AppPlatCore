using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Components;
using App.DAL;
using App.UIs;
using App.Web;
using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace App.Pages.Admin
{

    [Auth(Power.DeptView, Power.DeptNew, Power.DeptEdit, Power.DeptDelete)]
    [CheckPower(Power.DeptEdit)]
    public class DeptFormModel : BaseAdminModel
    {
        public IEnumerable<Dept> Depts { get; set; }

        [BindProperty]
        public Dept Dept { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            //var powers = App.Utils.Reflector.GetAttribute<AuthAttribute>();
            if (id != 0)
            {
                // 查看与编辑模式
                Dept = await DB.Depts.Where(m => m.ID == id).AsNoTracking().FirstOrDefaultAsync();
                if (Dept == null)
                    return Content("无效参数！");

                Depts = UI.ResolveDDL<Dept>(DeptHelper.Depts, id).ToArray();
            }
            else
            {
                // 新增模式
                Depts = UI.ResolveDDL<Dept>(DeptHelper.Depts).ToArray();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeptEdit_btnSaveClose_ClickAsync()
        {
            ModelState.Remove("Dept.ID");
            if (ModelState.IsValid)
            {
                // 下拉列表的顶级节点值为-1
                if (Dept.ParentID == -1)
                    Dept.ParentID = null;

                if (Dept.ID == 0)
                {
                    // new
                    DB.Depts.Add(Dept);
                    await DB.SaveChangesAsync();
                }
                else
                {
                    // modify
                    DB.Entry(Dept).State = EntityState.Modified;
                    await DB.SaveChangesAsync();
                }

                DeptHelper.Reload();
                ActiveWindow.HidePostBack();
            }

            return UIHelper.Result();
        }
    }
}