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
using App.DAL;
using System.Reflection;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using App.Components;

namespace App
{
    public partial class BaseModel : PageModel
    {
        // private
        private DynamicViewData _viewBag;

        /// <summary>
        /// 为 PageModel 提供 ViewBag 属性（动态类型，无法编译时检查，不推荐使用。此处只是提供兼容延续）
        /// RazorPageBase 提供了 ViewBag，是否重复？
        /// PageModel 提供了 ViewData
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


        //-------------------------------------------------
        // 页面处理事件
        //-------------------------------------------------
        /// <summary>页面处理器调用之前执行（加了在线逻辑）</summary>
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);

            // 如果用户已经登录，更新在线记录
            if (User.Identity.IsAuthenticated)
                Auth.UpdateOnlineUser(GetIdentityID());
        }

        public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            base.OnPageHandlerExecuted(context);
        }







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
