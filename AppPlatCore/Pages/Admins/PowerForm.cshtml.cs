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
    [CheckPower("CorePowerEdit")]
    public class PowerFormModel : BaseAdminModel
    {
        [BindProperty]
        public Power Power { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id != 0)
            {
                Power = await DB.Powers.Where(m => m.ID == id).AsNoTracking().FirstOrDefaultAsync();
                if (Power == null)
                    return Content("无效参数！");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostPowerEdit_btnSaveClose_ClickAsync()
        {
            ModelState.Remove("Power.ID");
            if (ModelState.IsValid)
            {
                if (Power.ID == 0)
                    DB.Powers.Add(Power);
                else
                    DB.Entry(Power).State = EntityState.Modified;
                await DB.SaveChangesAsync();
                ActiveWindow.HidePostBack();
            }

            return UIHelper.Result();
        }
    }
}