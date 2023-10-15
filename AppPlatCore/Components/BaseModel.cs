using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using FineUICore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Configuration;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Filters;
using App.Models;
using System.Reflection;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace App
{
    public partial class BaseModel : PageModel
    {
        // 只读静态变量
        private static readonly string SK_ONLINE_UPDATE_TIME = "OnlineUpdateTime";
        public static readonly string CHECK_POWER_FAIL_PAGE_MESSAGE = "您无权访问此页面！";
        public static readonly string CHECK_POWER_FAIL_ACTION_MESSAGE = "您无权进行此操作！";



        #region ViewBag

        private DynamicViewData _viewBag;

        /// <summary>
        /// Add ViewBag to PageModel
        /// https://forums.asp.net/t/2128012.aspx?Razor+Pages+ViewBag+has+gone+
        /// https://github.com/aspnet/Mvc/issues/6754
        /// </summary>
        public dynamic ViewBag
        {
            get
            {
                if (_viewBag == null)
                    _viewBag = new DynamicViewData(ViewData);
                return _viewBag;
            }
        }
        #endregion


        #region OnActionExecuting

        /// <summary>
        /// 页面处理器调用之前执行
        /// </summary>
        /// <param name="context"></param>
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);

            // 如果用户已经登录，更新在线记录
            if (User.Identity.IsAuthenticated)
            {
                UpdateOnlineUser(GetIdentityID());
            }
        }

        public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            base.OnPageHandlerExecuted(context);
        }

        #endregion



        #region 在线用户相关

        protected void UpdateOnlineUser(int? userID)
        {
            if (userID == null)
            {
                return;
            }

            DateTime now = DateTime.Now;
            object lastUpdateTime = HttpContext.Session.GetObject<DateTime>(SK_ONLINE_UPDATE_TIME);
            if (lastUpdateTime == null || (Convert.ToDateTime(lastUpdateTime).Subtract(now).TotalMinutes > 5))
            {
                // 记录本次更新时间
                HttpContext.Session.SetObject<DateTime>(SK_ONLINE_UPDATE_TIME, now);

                Online online = DB.Onlines.Where(o => o.User.ID == userID).FirstOrDefault();
                if (online != null)
                {
                    online.UpdateTime = now;
                    DB.SaveChanges();
                }

            }
        }

        protected async Task RegisterOnlineUserAsync(int userID)
        {
            DateTime now = DateTime.Now;
            Online online = await DB.Onlines.Where(o => o.User.ID == userID).FirstOrDefaultAsync();

            // 如果不存在，就创建一条新的记录
            if (online == null)
            {
                online = new Online();
                DB.Onlines.Add(online);
            }
            online.UserID = userID;
            online.IPAdddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            online.LoginTime = now;
            online.UpdateTime = now;
            await DB.SaveChangesAsync();

            // 记录本次更新时间
            HttpContext.Session.SetObject<DateTime>(SK_ONLINE_UPDATE_TIME, now);
        }

        /// <summary>在线人数</summary>
        protected async Task<int> GetOnlineCountAsync()
        {
            DateTime lastM = DateTime.Now.AddMinutes(-15);
            return await DB.Onlines.Where(o => o.UpdateTime > lastM).CountAsync();
        }

        #endregion

        #region 当前登录用户信息

        // http://blog.163.com/zjlovety@126/blog/static/224186242010070024282/
        // http://www.cnblogs.com/gaoshuai/articles/1863231.html
        /// <summary>
        /// 当前登录用户的角色列表
        /// </summary>
        /// <returns></returns>
        protected List<int> GetIdentityRoleIDs()
        {
            return GetIdentityRoleIDs(HttpContext);
        }

        #endregion

        #region 权限检查

        /// <summary>
        /// 检查当前用户是否拥有某个权限
        /// </summary>
        /// <param name="powerType"></param>
        /// <returns></returns>
        protected bool CheckPower(string powerName)
        {
            return CheckPower(HttpContext, powerName);
        }

        /// <summary>
        /// 获取当前登录用户拥有的全部权限列表
        /// </summary>
        /// <param name="roleIDs"></param>
        /// <returns></returns>
        protected List<string> GetRolePowerNames()
        {
            return GetRolePowerNames(HttpContext);
        }

        /// <summary>
        /// 检查权限失败（页面第一次加载）
        /// </summary>
        public static void CheckPowerFailWithPage(HttpContext context)
        {
            string PageTemplate = "<!DOCTYPE html><html><head><meta http-equiv=\"Content-Type\" content=\"text/html;charset=utf-8\"/><head><body>{0}</body></html>";
            context.Response.WriteAsync(String.Format(PageTemplate, CHECK_POWER_FAIL_PAGE_MESSAGE));
        }

        /// <summary>
        /// 检查权限失败（页面回发）
        /// </summary>
        public static void CheckPowerFailWithAlert()
        {
            FineUICore.PageContext.RegisterStartupScript(Alert.GetShowInTopReference(CHECK_POWER_FAIL_ACTION_MESSAGE));
        }

        /// <summary>
        /// 检查当前用户是否拥有某个权限
        /// </summary>
        /// <param name="context"></param>
        /// <param name="powerName"></param>
        /// <returns></returns>
        public static bool CheckPower(HttpContext context, string powerName)
        {
            // 如果权限名为空，则放行
            if (String.IsNullOrEmpty(powerName))
            {
                return true;
            }

            // 当前登陆用户的权限列表
            List<string> rolePowerNames = GetRolePowerNames(context);
            if (rolePowerNames.Contains(powerName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取当前登录用户拥有的全部权限列表
        /// </summary>
        /// <param name="roleIDs"></param>
        /// <returns></returns>
        public static List<string> GetRolePowerNames(HttpContext context)
        {
            var db = GetDbConnection();

            // 将用户拥有的权限列表保存在Session中，这样就避免每个请求多次查询数据库
            if (context.Session.GetObject<List<string>>("UserPowerList") == null)
            {
                List<string> rolePowerNames = new List<string>();

                // 超级管理员拥有所有权限
                if (GetIdentityName(context) == "admin")
                {
                    rolePowerNames = db.Powers.Select(p => p.Name).ToList();
                }
                else
                {
                    List<int> roleIDs = GetIdentityRoleIDs(context);

                    var roles = db.Roles
                        .Include(r => r.RolePowers)
                        .ThenInclude(rp => rp.Power)
                        .Where(r => roleIDs.Contains(r.ID))
                        .ToList();
                    foreach (var role in roles)
                    {
                        foreach (var rolepower in role.RolePowers)
                        {
                            if (!rolePowerNames.Contains(rolepower.Power.Name))
                            {
                                rolePowerNames.Add(rolepower.Power.Name);
                            }
                        }
                    }
                }

                context.Session.SetObject<List<string>>("UserPowerList", rolePowerNames);
            }
            return context.Session.GetObject<List<string>>("UserPowerList");
        }

        // http://blog.163.com/zjlovety@126/blog/static/224186242010070024282/
        // http://www.cnblogs.com/gaoshuai/articles/1863231.html
        /// <summary>当前登录用户的角色列表</summary>
        public static List<int> GetIdentityRoleIDs(HttpContext context)
        {
            List<int> roleIDs = new List<int>();

            if (context.User.Identity.IsAuthenticated)
            {
                string userData = context.User.Claims.Where(x => x.Type == "RoleIDs").FirstOrDefault().Value;
                foreach (string roleID in userData.Split(','))
                {
                    if (!String.IsNullOrEmpty(roleID))
                        roleIDs.Add(Convert.ToInt32(roleID));
                }
            }

            return roleIDs;
        }


        /// <summary>
        /// 当前登录用户名
        /// </summary>
        /// <returns></returns>
        protected string GetIdentityName()
        {
            return GetIdentityName(HttpContext);
        }

        /// <summary>
        /// 当前登录用户名
        /// </summary>
        /// <returns></returns>
        public static string GetIdentityName(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
                return null;

            var userName = context.User.Claims.Where(x => x.Type == "UserName").FirstOrDefault().Value;
            return userName;
        }

        /// <summary>
        /// 当前登录用户标识符
        /// </summary>
        /// <returns></returns>
        protected int? GetIdentityID()
        {
            return GetIdentityID(HttpContext);
        }

        /// <summary>
        /// 当前登录用户标识符
        /// </summary>
        /// <returns></returns>
        public static int? GetIdentityID(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
                return null;

            var userID = context.User.Claims.Where(x => x.Type == "UserID").FirstOrDefault().Value;
            return Convert.ToInt32(userID);
        }

        #endregion
    }
}
