using AppBoxCore.Models;

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

namespace AppBoxCore.Pages
{
    public class LoginModel : BaseModel
    {
        public string Window1Title { get; set; }
        public void OnGet()
        {
            LoadData();
        }

        private void LoadData()
        {
            Window1Title = String.Format("AppBoxCore v{0}", GetProductVersion());
        }
        public async Task<IActionResult> OnPostBtnSubmit_ClickAsync(string tbxUserName, string tbxPassword)
        {
            string userName = tbxUserName.Trim();
            string password = tbxPassword.Trim();
            User user = await DB.Users
                .Include(u => u.RoleUsers)
                .Where(u => u.Name == userName).AsNoTracking().FirstOrDefaultAsync();

            if (user != null)
            {
                if (PasswordUtil.ComparePasswords(user.Password, password))
                {
                    if (!user.Enabled)
                        Alert.Show("用户未启用，请联系管理员！");
                    else
                    {
                        // 登录成功, 重定向到登陆后首页
                        await LoginSuccess(user);
                        return RedirectToPage("/Index");
                    }
                }
                else
                {
                    Alert.Show("用户名或密码错误！");
                }
            }
            else
            {
                Alert.Show("用户名或密码错误！");
            }

            return UIHelper.Result();
        }

        private async Task LoginSuccess(User user)
        {
            await RegisterOnlineUserAsync(user.ID);

            // 用户所属的角色字符串，以逗号分隔
            string roleIDs = String.Empty;
            if (user.RoleUsers != null)
                roleIDs = String.Join(",", user.RoleUsers.Select(r => r.RoleID).ToArray());

            // Aspnetcore 标准登录代码（Claim-Identity-Principal-Ticket, 属性-身份-主角-验票）
            var claims = new[] { new Claim("UserID", user.ID.ToString()), new Claim("UserName", user.Name), new Claim("RoleIDs", roleIDs) }; // 属性
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);  // 用户身份
            var principal = new ClaimsPrincipal(identity); // 主角
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties() { IsPersistent = false }
                );

        }
    }
}