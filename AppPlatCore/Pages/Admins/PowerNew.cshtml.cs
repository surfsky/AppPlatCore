using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.DAL;
using App.Components;
using FineUICore;

namespace App.Pages.Admin
{
    [CheckPower("CorePowerNew")]
    public class PowerNewModel : BaseAdminModel
    {
        [BindProperty]
        public Power Power { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostPowerNew_btnSaveClose_ClickAsync()
        {
            if (ModelState.IsValid)
            {
                DB.Powers.Add(Power);
                await DB.SaveChangesAsync();
                ActiveWindow.HidePostBack();
            }

            return UIHelper.Result();
        }
    }
}