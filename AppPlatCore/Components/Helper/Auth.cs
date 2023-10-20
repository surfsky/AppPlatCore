using App.Components;
using FineUICore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace App
{
    /// <summary>
    /// 授权鉴权相关的辅助方法
    /// </summary>
    internal static class Auth
    {
        public static readonly string MSG_CHECK_POWER_FAIL_PAGE = "您无权访问此页面！";
        public static readonly string MSG_CHECK_POWER_FAIL_ACTION = "您无权进行此操作！";

        /// <summary>检查当前用户是否拥有某个权限</summary>
        public static bool CheckPower(HttpContext context, string powerName)
        {
            // 如果权限名为空，则放行
            if (String.IsNullOrEmpty(powerName))
                return true;

            // 当前登陆用户的权限列表
            List<string> rolePowerNames = GetRolePowerNames(context);
            if (rolePowerNames.Contains(powerName))
                return true;

            return false;
        }

        /// <summary>检查权限失败（页面回发）</summary>
        public static void CheckPowerFailWithAlert()
        {
            FineUICore.PageContext.RegisterStartupScript(Alert.GetShowInTopReference(MSG_CHECK_POWER_FAIL_ACTION));
        }

        /// <summary>检查权限失败（页面第一次加载）</summary>
        public static void CheckPowerFailWithPage(HttpContext context)
        {
            string PageTemplate = "<!DOCTYPE html><html><head><meta http-equiv=\"Content-Type\" content=\"text/html;charset=utf-8\"/><head><body>{0}</body></html>";
            context.Response.WriteAsync(String.Format(PageTemplate, MSG_CHECK_POWER_FAIL_PAGE));
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

        /// <summary>获取当前登录用户拥有的全部权限列表</summary>
        public static List<string> GetRolePowerNames(HttpContext context)
        {
            var db = Common.GetDbConnection();

            // 将用户拥有的权限列表保存在Session中，这样就避免每个请求多次查询数据库
            if (context.Session.GetObject<List<string>>("UserPowerList") == null)
            {
                List<string> rolePowerNames = new List<string>();

                if (GetIdentityName(context) == "admin")
                {
                    // 超级管理员拥有所有权限
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
                                rolePowerNames.Add(rolepower.Power.Name);
                        }
                    }
                }

                context.Session.SetObject<List<string>>("UserPowerList", rolePowerNames);
            }
            return context.Session.GetObject<List<string>>("UserPowerList");
        }
    }
}