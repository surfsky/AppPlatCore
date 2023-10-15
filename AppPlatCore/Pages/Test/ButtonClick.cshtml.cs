using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App;
using App.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FineUICore.Examples.RazorPages.Pages.Button
{
    public class ButtonClickModel : BaseModel
    {
        public void OnGet()
        {
            ViewBag.btnClientClick2Script = Alert.GetShowInTopReference("通过ViewBag传递的客户端事件");
        }

        public IActionResult OnPostBtnServerClick_Click()
        {
            UI.ShowNotify("这是服务器端事件");
            return UIHelper.Result();
        }

        public IActionResult OnPostBtnChangeClientClick2_Click()
        {
            UIHelper.Button("btnClientClick2").ClientClick(Alert.GetShowInTopReference("客户端事件已改变！"));
            return UIHelper.Result();
        }
    }
}