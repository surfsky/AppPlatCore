using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entities;

namespace App.DAL
{
    public class Role : EntityBase<Role>
    {
        [Display(Name="名称")]
        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "备注")]
        [StringLength(500)]
        public string Remark { get; set; }


        public List<RoleUser> RoleUsers { get; set; }
        public List<RolePower> RolePowers { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}