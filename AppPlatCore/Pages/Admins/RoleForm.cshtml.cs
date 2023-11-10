using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.DAL;
using App.Components;
using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace App.Pages.Admin
{
    [CheckPower(Power.RolePowerEdit)]
    [Auth(Power.RolePowerEdit)]
    public class RoleFormModel : BaseAdminModel
    {
        [BindProperty]
        public Role Role { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id != 0)
            {
                Role = await DB.Roles.Where(m => m.ID == id).AsNoTracking().FirstOrDefaultAsync();
                if (Role == null)
                    return Content("无效参数！");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostRoleEdit_btnSaveClose_ClickAsync()
        {
            ModelState.Remove("Role.ID");
            if (ModelState.IsValid)
            {
                if (Role.ID == 0)
                    DB.Roles.Add(Role);
                else
                    DB.Entry(Role).State = EntityState.Modified;
                await DB.SaveChangesAsync();
                ActiveWindow.HidePostBack();
            }

            return UIHelper.Result();
        }
    }
}