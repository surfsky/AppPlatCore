﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Utils;
using App.Entities;

namespace App.DAL
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class Menu : EntityBase<Menu>, ITreeNode, ICloneable
    {
        [Display(Name = "菜单名称")]
        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "图标")]
        [StringLength(200)]
        public string ImageUrl { get; set; }

        [Display(Name = "链接")]
        [StringLength(200)]
        public string NavigateUrl { get; set; }

        [Display(Name = "备注")]
        [StringLength(500)]
        public string Remark { get; set; }

        [Display(Name = "排序")]
        [Required]
        public int SortIndex { get; set; }


        [Display(Name = "目标页面")]
        public string Target { get; set; }

        [Display(Name = "是否展开")]
        public bool? Expanded { get; set; } = false;

        [Display(Name = "是否可见")]
        public bool? Visible { get; set; } = true;


        [Display(Name = "上级菜单")]
        public long? ParentID { get; set; }
        public Menu Parent { get; set; }


        [Display(Name = "浏览权限")]
        public Power? Power { get; set; }

        
        public List<Menu> Children { get; set; }


        //------------------------------------------------
        // 扩展属性（不入数据库）
        //------------------------------------------------
        /// <summary>菜单在树形结构中的层级（从0开始）</summary>
        [NotMapped]
        public int TreeLevel { get; set; }

        /// <summary>是否可用（默认true）,在模拟树的下拉列表中使用</summary>
        [NotMapped]
        public bool Enabled { get; set; }

        /// <summary>是否叶子节点（默认true）</summary>
        [NotMapped]
        public bool IsTreeLeaf { get; set; }


        //------------------------------------------------
        // 方法
        //------------------------------------------------
        public object Clone()
        {
            Menu menu = new Menu
            {
                ID = ID,
                Name = Name,
                ImageUrl = ImageUrl,
                NavigateUrl = NavigateUrl,
                Remark = Remark,
                Target = Target,
                Expanded = Expanded,
                Visible = Visible,
                SortIndex = SortIndex,
                TreeLevel = TreeLevel,
                Enabled = Enabled,
                IsTreeLeaf = IsTreeLeaf
            };
            return menu;
        }

        public override string ToString()
        {
            return "  ".Repeat(this.TreeLevel) + this.Name;
        }
    }
}