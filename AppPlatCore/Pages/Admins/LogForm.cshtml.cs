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
        public DAL.Log Log { get; set; }

        public IActionResult OnGet(long id)
        {
            // 经测试 ef sqlite 无法获取类型为 long 的ID，改为 int 就行了
            //var item1 = DB.Logs.FirstOrDefault(t => t.ID==id);
            //var item2 = Log.Set.FirstOrDefault(t => t.ID==id);
            Log = Log.Get(id);  
            if (Log == null)
                return Content("无效参数！");
            return Page();
        }
    }
}