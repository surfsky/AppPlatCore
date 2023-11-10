using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entities;
using App.Utils;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace App.DAL
{
    /// <summary>角色人员</summary>
    [UI("系统", "角色人员")]
    public class RoleUser : EntityBase<RoleUser>
    {
        public long RoleID { get; set; }
        public long UserID { get; set; }

        public virtual Role Role { get; set; }
        public virtual User User { get; set; }

        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        /// <summary>导出</summary>
        public override object Export(ExportMode type = ExportMode.Normal)
        {
            return new
            {
                this.ID,
                this.UserID,
                this.RoleID,
                UserName = this.User.Name,
                RoleName = this.Role.Name
            };
        }

        /// <summary>获取详情</summary>
        public new static RoleUser GetDetail(long id)
        {
            return Set.Include(t => t.User).Include(t => t.Role).Where(t => t.ID == id).FirstOrDefault();
        }

        /// <summary>检索</summary>
        public static IQueryable<RoleUser> Search(
            long? userId = null,
            string userName = null,
            long? roleId = null,
            string roleName = null
            )
        {
            IQueryable<RoleUser>       q = Set.Include(t => t.User).Include(t => t.Role);
            if (userId.IsNotEmpty())   q = q.Where(t => t.UserID == userId);
            if (userName.IsNotEmpty()) q = q.Where(t => t.User.Name.Contains(userName));
            if (roleId.IsNotEmpty())   q = q.Where(t => t.RoleID == roleId);
            if (roleName.IsNotEmpty()) q = q.Where(t => t.Role.Name.Contains(roleName));
            return q;
        }

        /// <summary>设置某个用户拥有的角色清单</summary>
        public static void SetUserRoles(long userID, List<long> roleIDs)
        {
            RoleUser.Set.Where(t => t.UserID == userID).Delete();
            foreach (var roleID in roleIDs)
            {
                var item = new RoleUser();
                item.RoleID = roleID;
                item.UserID = userID;
                item.Save();
            }
        }

        /// <summary>设置某个角色拥有的用户清单</summary>
        public static void SetRoleUsers(long roleID, List<long> userIDs)
        {
            RoleUser.Set.Where(t => t.RoleID == roleID).Delete();
            foreach (var userID in userIDs)
            {
                var item = new RoleUser();
                item.RoleID = roleID;
                item.UserID = userID;
                item.Save();
            }
        }
    }


}