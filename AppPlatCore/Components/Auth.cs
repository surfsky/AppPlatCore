using App.DAL;
using App.Utils;
using App.Web;
using FineUICore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Components
{
    /// <summary>
    /// 授权鉴权相关的辅助方法
    /// </summary>
    public static class Auth
    {
        public static readonly string MSG_CHECK_POWER_FAIL_PAGE = "您无权访问此页面！";
        public static readonly string MSG_CHECK_POWER_FAIL_ACTION = "您无权进行此操作！";
        public static readonly string SESSION_VERIFYCODE = "session_code";                  // 验证码Session名称
        private static readonly string MSG_ONLINE_UPDATE_TIME = "OnlineUpdateTime";

        //--------------------------------------------------
        // 登录注销
        //--------------------------------------------------
        /// <summary>注销</summary>
        public static void Logout()
        {
            Asp.Current.SignOutAsync();
            Asp.Current.Session.Clear();
            Asp.Response.Redirect("/Login");
        }

        public static string GetVerifyCode()
        {
            return Asp.Current.Session.GetString(SESSION_VERIFYCODE);
        }
        public static void SetVerifyCode(string code)
        {
            Asp.Current.Session.SetString(SESSION_VERIFYCODE, code);
        }

        /// <summary>登录</summary>
        /// <returns>
        /// 0  : 登录成功
        /// -1 : 用户不存在
        /// -2 : 用户未启用
        /// -3 : 用户名或密码错误
        /// </returns>
        public static int Login(string userName, string password, string verifyCode)
        {
            if (GetVerifyCode().ToLower() != verifyCode)
                return -4;

            User user = Common.GetDbConnection().Users
                .Include(u => u.RoleUsers)
                .Where(u => u.Name == userName).AsNoTracking().FirstOrDefault();
            if (user == null)
                return -1;
            else
            {
                if (!PasswordUtil.ComparePasswords(user.Password, password))
                    return -1;
                else
                {
                    if (!user.Enabled)
                        return -2;
                    else
                    {
                        LoginSuccess(user);
                        return 0;
                    }
                }
            }
        }

        /// <summary>登录成功，写入验票</summary>
        public static void LoginSuccess(User user)
        {
            RegisterOnlineUser(user.ID);

            // 用户所属的角色字符串，以逗号分隔
            string roleIDs = String.Empty;
            if (user.RoleUsers != null)
                roleIDs = String.Join(",", user.RoleUsers.Select(r => r.RoleID).ToArray());

            // Aspnetcore 标准登录代码（Claim-Identity-Principal-Ticket, 属性-身份-主角-验票）
            var claims = new[] { new Claim("UserID", user.ID.ToString()), new Claim("UserName", user.Name), new Claim("RoleIDs", roleIDs) }; // 属性
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);  // 用户身份
            var principal = new ClaimsPrincipal(identity); // 主角
            Asp.Current.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties() { IsPersistent = false }
                );

        }

        /// <summary>当前登录用户标识符</summary>
        public static int? GetIdentityID(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
                return null;

            var userID = context.User.Claims.Where(x => x.Type == "UserID").FirstOrDefault().Value;
            return Convert.ToInt32(userID);
        }



        /// <summary>当前登录用户名</summary>
        public static string GetIdentityName(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
                return null;

            var userName = context.User.Claims.Where(x => x.Type == "UserName").FirstOrDefault().Value;
            return userName;
        }

        //--------------------------------------------------
        // 在线用户
        //--------------------------------------------------
        public static void UpdateOnlineUser(long? userID)
        {
            if (userID == null)
                return;

            var db = Common.GetDbConnection();
            DateTime now = DateTime.Now;
            object lastUpdateTime = Asp.Session.GetObject<DateTime>(MSG_ONLINE_UPDATE_TIME);
            if (lastUpdateTime == null || (Convert.ToDateTime(lastUpdateTime).Subtract(now).TotalMinutes > 5))
            {
                // 记录本次更新时间
                Asp.Session.SetObject<DateTime>(MSG_ONLINE_UPDATE_TIME, now);
                Online online = db.Onlines.Where(o => o.User.ID == userID).FirstOrDefault();
                if (online != null)
                {
                    online.UpdateTime = now;
                    db.SaveChanges();
                }

            }
        }

        public static void RegisterOnlineUser(long userID)
        {
            var db = Common.GetDbConnection();
            DateTime now = DateTime.Now;
            Online online = db.Onlines.Where(o => o.User.ID == userID).FirstOrDefault();

            // 如果不存在，就创建一条新的记录
            if (online == null)
            {
                online = new Online();
                db.Onlines.Add(online);
            }
            online.UserID = userID;
            online.IPAdddress = Asp.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            online.LoginTime = now;
            online.UpdateTime = now;
            db.SaveChanges();

            // 记录本次更新时间
            Asp.Session.SetObject<DateTime>(MSG_ONLINE_UPDATE_TIME, now);
        }

        /// <summary>在线人数</summary>
        public static  async Task<int> GetOnlineCountAsync()
        {
            var db = Common.GetDbConnection();
            DateTime lastM = DateTime.Now.AddMinutes(-15);
            return await db.Onlines.Where(o => o.UpdateTime > lastM).CountAsync();
        }


        //--------------------------------------------------
        // 权限校验
        //--------------------------------------------------
        /// <summary>检查当前用户是否拥有某个权限</summary>
        public static bool CheckPower(HttpContext context, Power power)
        {
            // 当前登陆用户的权限列表
            List<Power> powers = GetUserPowers(context);
            if (powers.Contains(power))
                return true;
            return false;
        }

        /// <summary>检查权限失败（页面回发）</summary>
        public static void CheckPowerFailWithAlert()
        {
            PageContext.RegisterStartupScript(Alert.GetShowInTopReference(MSG_CHECK_POWER_FAIL_ACTION));
        }

        /// <summary>检查权限失败（页面第一次加载）</summary>
        public static void CheckPowerFailWithPage(HttpContext context)
        {
            string PageTemplate = "<!DOCTYPE html><html><head><meta http-equiv=\"Content-Type\" content=\"text/html;charset=utf-8\"/><head><body>{0}</body></html>";
            context.Response.WriteAsync(string.Format(PageTemplate, MSG_CHECK_POWER_FAIL_PAGE));
        }




        // http://blog.163.com/zjlovety@126/blog/static/224186242010070024282/
        // http://www.cnblogs.com/gaoshuai/articles/1863231.html
        /// <summary>当前登录用户的角色列表</summary>
        public static List<long> GetIdentityRoleIDs(HttpContext context)
        {
            var roleIDs = new List<long>();
            if (context.User.Identity.IsAuthenticated)
            {
                string userData = context.User.Claims.Where(x => x.Type == "RoleIDs").FirstOrDefault().Value;
                foreach (string roleID in userData.Split(','))
                {
                    if (roleID.IsNotEmpty())
                        roleIDs.Add(Convert.ToInt32(roleID));
                }
            }

            return roleIDs;
        }

        /// <summary>获取当前登录用户拥有的全部权限列表</summary>
        public static List<Power> GetUserPowers(HttpContext context)
        {
            // 将用户拥有的权限列表保存在Session中，这样就避免每个请求多次查询数据库
            return Asp.GetSessionData<List<Power>>("UserPowers", () =>
            {
                var name = GetIdentityName(context);
                if (name.IsEmpty())
                    return new List<Power>();
                var user = User.Set.FirstOrDefault(t => t.Name == name);
                return user.GetPowers();
            });
        }
    }
}