using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entities;
using App.Utils;
using Z.EntityFramework.Plus;

namespace App.DAL
{
    /// <summary>
    /// 角色拥有的权限
    /// </summary>
    [UI("系统", "角色拥有的权限")]
    public class RolePower : EntityBase<RolePower>
    {
        public long RoleID { get; set; }
        public Power PowerID { get; set; }
        public virtual Role Role{get;set;}


        /// <summary>设置某个角色拥有的权限列表</summary>
        public static void SetRolePowers(long roleID, List<long> powerIDs)
        {
            RolePower.Set.Where(t => t.RoleID == roleID).Delete();
            foreach (var powerId in powerIDs)
            {
                var item = new RolePower() { RoleID = roleID, PowerID = (Power)powerId };
                item.Save();
            }
        }
    }

}