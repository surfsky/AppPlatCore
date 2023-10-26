using App.Components;
using App.DAL;

using FineUICore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Pages
{
    public class LoginModel : BaseModel
    {
        public string WinTitle { get; set; }
        public void OnGet()
        {
            WinTitle = String.Format("{0} v{1}", SiteConfig.Instance.Title, Common.GetVersion());
        }

        public  IActionResult OnPostBtnSubmit_Click(string userName, string password, string verifyCode)
        {
            var n = Auth.Login(userName, password, verifyCode);
            switch (n)
            {
                case 0:  { return RedirectToPage("/Index"); }
                case -1: { Alert.Show("用户名或密码错");           break;}
                case -2: { Alert.Show("用户未启用，请联系管理员"); break;}
                case -3: { Alert.Show("用户名或密码错");           break;}
                case -4: { Alert.Show("验证码错误");               break;}
            }
            UIHelper.Image("imgVerify").ImageUrl("/HttpApi/Common/VerifyImage?" + new Random().Next().ToString());
            return UIHelper.Result();
        }

    }
}