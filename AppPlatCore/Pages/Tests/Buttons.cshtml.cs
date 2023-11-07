using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App;
using App.Components;
using App.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FineUICore.Examples.RazorPages.Pages.Button
{
    public class ButtonsModel : BaseModel
    {

        //public string BtnSignOutHandler = Url.Action("SignOut");
        public string Script = Alert.GetShowInTopReference("客户端事件");

        public void OnGet()
        {
            //ViewBag.btnClientClick2Script = Alert.GetShowInTopReference("通过ViewBag传递的客户端事件");
            var action = Asp.GetQueryString("action");
            if (action == "SignOut")
                Response.Redirect("/Login");
        }

        public IActionResult OnPost()
        {
            var action = Asp.GetQueryString("action");
            if (action == "SignOut")
                return RedirectToPage("/Login");
            return UIHelper.Result();
        }

        public IActionResult OnPostBtnServerClick_Click()
        {
            UI.ShowNotify("这是服务器端事件");
            //return UIHelper.Result();
            //return RedirectToPage("/Login");
            return UIHelper.Result();
        }

        public IActionResult OnPostBtnChangeClientClick2_Click()
        {
            UIHelper.Button("btnClientClick2").ClientClick(Alert.GetShowInTopReference("客户端事件已改变！"));
            return UIHelper.Result();
        }

        public IActionResult OnPostExceptionClick()
        {
            // 触发异常
            Console.WriteLine(1 / 0.0);
            throw new Exception("Some Exception");
            //return UIHelper.Result();
        }


        public void SignOut()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            //RedirectToAction("/Login");
            RedirectToPage("/Login");
        }

        /*
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
        */
    }
}