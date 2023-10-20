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
        private static readonly string MSG_ONLINE_UPDATE_TIME = "OnlineUpdateTime";



        #region ViewBag

        private DynamicViewData _viewBag;

        /// <summary>
        /// Add ViewBag to PageModel（动态类型，无法编译时检查，不推荐使用。此处只是提供兼容延续）
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

        /// <summary>页面处理器调用之前执行（加了在线逻辑）</summary>
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);

            // 如果用户已经登录，更新在线记录
            if (User.Identity.IsAuthenticated)
                UpdateOnlineUser(GetIdentityID());
        }

        public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            base.OnPageHandlerExecuted(context);
        }

        #endregion



        #region 在线用户

        protected void UpdateOnlineUser(int? userID)
        {
            if (userID == null)
                return;

            DateTime now = DateTime.Now;
            object lastUpdateTime = HttpContext.Session.GetObject<DateTime>(MSG_ONLINE_UPDATE_TIME);
            if (lastUpdateTime == null || (Convert.ToDateTime(lastUpdateTime).Subtract(now).TotalMinutes > 5))
            {
                // 记录本次更新时间
                HttpContext.Session.SetObject<DateTime>(MSG_ONLINE_UPDATE_TIME, now);
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
            HttpContext.Session.SetObject<DateTime>(MSG_ONLINE_UPDATE_TIME, now);
        }

        /// <summary>在线人数</summary>
        protected async Task<int> GetOnlineCountAsync()
        {
            DateTime lastM = DateTime.Now.AddMinutes(-15);
            return await DB.Onlines.Where(o => o.UpdateTime > lastM).CountAsync();
        }

        #endregion


        #region 用户与权限检查

        // http://blog.163.com/zjlovety@126/blog/static/224186242010070024282/
        // http://www.cnblogs.com/gaoshuai/articles/1863231.html
        /// <summary>当前登录用户的角色列表</summary>
        protected List<int> GetIdentityRoleIDs()
        {
            return Auth.GetIdentityRoleIDs(HttpContext);
        }


        /// <summary>检查当前用户是否拥有某个权限</summary>
        protected bool CheckPower(string powerName)
        {
            return Auth.CheckPower(HttpContext, powerName);
        }

        /// <summary>获取当前登录用户拥有的全部权限列表</summary>
        protected List<string> GetRolePowerNames()
        {
            return Auth.GetRolePowerNames(HttpContext);
        }


        /// <summary>当前登录用户名</summary>
        protected string GetIdentityName()
        {
            return Auth.GetIdentityName(HttpContext);
        }
        /// <summary>当前登录用户标识符</summary>
        protected int? GetIdentityID()
        {
            return Auth.GetIdentityID(HttpContext);
        }

        #endregion
    }
}
