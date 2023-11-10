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
    public class Dept : EntityBase<Dept>, ITreeNode, ICloneable
    {
        [Display(Name = "名称")]
        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "排序")]
        [Required]
        public int SortIndex { get; set; }

        [Display(Name = "备注")]
        [StringLength(500)]
        public string Remark { get; set; }

        [Display(Name = "上级部门")]
        public long? ParentID { get; set; }


        //
        public Dept Parent { get; set; }
        public List<Dept> Children { get; set; }
        public List<User> Users { get; set; }



        [NotMapped]      public int TreeLevel { get; set; }
        [NotMapped]      public bool Enabled { get; set; }
        [NotMapped]      public bool IsTreeLeaf { get; set; }


        public object Clone()
        {
            Dept dept = new Dept
            {
                ID = ID,
                Name = Name,
                Remark = Remark,
                SortIndex = SortIndex,
                TreeLevel = TreeLevel,
                Enabled = Enabled,
                IsTreeLeaf = IsTreeLeaf
            };
            return dept;
        }

        public override string ToString()
        {
            return "  ".Repeat(this.TreeLevel) + this.Name;
        }
    }
}