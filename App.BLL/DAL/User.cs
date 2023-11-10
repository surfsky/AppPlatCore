using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Utils;
using App.Entities;

namespace App.DAL
{
    //public class User : IKeyID
    public class User : EntityBase<User>, IDeleteLogic
    {
        [UI("是否在用")] public bool? InUsed { get; set; } = true;

        [Display(Name = "用户名")]
        [StringLength(50)]
        [Required]
        public string Name { get; set; }


        [Display(Name = "邮箱")]
        [StringLength(100)]
        public string Email { get; set; }

        [Display(Name = "密码")]
        [StringLength(50)]
        [Required]
        public string Password { get; set; }

        [Display(Name = "是否启用")]
        [Required]
        public bool Enabled { get; set; } = true;

        [Display(Name = "性别")]
        [StringLength(10)]
        public string Gender { get; set; }

        [Display(Name = "中文名")]
        [StringLength(100)]
        public string ChineseName { get; set; }

        [Display(Name = "英文名")]
        [StringLength(100)]
        public string EnglishName { get; set; }

        [Display(Name = "照片")]
        [StringLength(200)]
        public string Photo { get; set; }

        [Display(Name = "QQ")]
        [StringLength(50)]
        public string QQ { get; set; }

        [Display(Name = "公司邮箱")]
        [StringLength(100)]
        public string CompanyEmail { get; set; }

        [Display(Name = "工作电话")]
        [StringLength(50)]
        public string OfficePhone { get; set; }

        [Display(Name = "分机号")]
        [StringLength(50)]
        public string OfficePhoneExt { get; set; }

        [Display(Name = "家庭电话")]
        [StringLength(50)]
        public string HomePhone { get; set; }

        [Display(Name = "手机号")]
        [StringLength(50)]
        public string CellPhone { get; set; }

        [Display(Name = "地址")]
        [StringLength(500)]
        public string Address { get; set; }

        [Display(Name = "备注")]
        [StringLength(500)]
        public string Remark { get; set; }

        [Display(Name = "身份证")]
        [StringLength(50)]
        public string IdentityCard { get; set; }


        [Display(Name = "生日")]
        public DateTime? Birthday { get; set; }
        [Display(Name = "任职时间")]
        public DateTime? TakeOfficeTime { get; set; }
        [Display(Name = "上次登录时间")]
        public DateTime? LastLoginTime { get; set; }
        [Display(Name = "创建时间")]
        public DateTime? CreateTime { get; set; }


        [Display(Name = "职务")]
        public string  Title { get; set; }


        [Display(Name = "所属部门")]
        public long? DeptID { get; set; }

        [Display(Name = "所属部门")]
        public Dept Dept { get; set; }

        [Display(Name = "所属角色")]
        public List<RoleUser> RoleUsers { get; set; }


        // 用户拥有的权限
        [NotMapped]
        public List<Power> Powers { get; set; }// => IO.GetCache(this.UniID + "-Power", () => GetUserPowers(this));

        // 用户可访问的菜单
        [NotMapped]
        public List<Menu> Menus { get; set; }  // => GetAllowMenus(this.Powers);


        //------------------------------------------------------
        // 角色、权限
        //------------------------------------------------------
        /// <summary>是否具有某个权限</summary>
        public bool HasPower(Power power)
        {
            if (this.Powers == null)
                this.Powers = GetPowers();
            return this.Powers.Contains(power);
        }

        // 获取用户权限（admin拥有所有权限、普通用户根据角色来获取权限）
        public List<Power> GetPowers()
        {
            var powers = new List<Power>();
            if (this.Name == "admin")
                powers = typeof(Power).GetEnums<Power>();
            else
            {
                var roleIds = RoleUser.Set.Where(t => t.UserID == this.ID).Select(t => t.RoleID).ToList();
                RolePower.Search(t => roleIds.Contains(t.RoleID)).ToList().ForEach(t => powers.Add(t.PowerID));
            }
            return powers;
        }




    }


}