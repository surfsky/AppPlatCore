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
    [CheckPower("CoreLogView")]
    public class LogFormModel : BaseAdminModel
    {
        [BindProperty]
        public DAL.Log Log { get; set; }

        public IActionResult OnGet(int id)
        {
            Log = Log.Get(id);

            if (Log == null)
                return Content("无效参数！");
            return Page();
        }
    }
}