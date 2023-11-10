using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using App;
using App.Entities;

namespace App.DAL
{
    /// <summary>
    /// 菜单树辅助处理
    /// </summary>
    public class MenuHelper
    {
        private static List<Menu> _menus;

        /// <summary>所有菜单列表，并组织好父子层级关系</summary>
        public static List<Menu> Menus
        {
            get
            {
                if (_menus == null)
                {
                    _menus = new List<Menu>();
                    var dbMenus = Menu.Set.OrderBy(m => m.SortIndex).ToList();
                    ResolveMenuCollection(dbMenus, null, 0);
                }
                return _menus;
            }
        }

        public static void Reload()
        {
            _menus = null;
        }


        private static int ResolveMenuCollection(List<Menu> items, Menu parentItem, int level)
        {
            int count = 0;
            foreach (var item in items.Where(m => m.Parent == parentItem))
            {
                item.TreeLevel = level;
                item.IsTreeLeaf = true;
                item.Enabled = true;
                _menus.Add(item);
                count++;

                // 递归子节点
                level++;
                int childCount = ResolveMenuCollection(items, item, level);
                if (childCount != 0)
                    item.IsTreeLeaf = false;
                level--;
            }

            return count;
        }


    }
}
