using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using App;

namespace App.Models
{
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
                    var db = BaseModel.GetDbConnection();
                    List<Menu> dbMenus = db.Menus.Include(m => m.ViewPower).OrderBy(m => m.SortIndex).ToList();
                    ResolveMenuCollection(dbMenus, null, 0);
                }
                return _menus;
            }
        }

        private static int ResolveMenuCollection(List<Menu> dbMenus, Menu parentMenu, int level)
        {
            int count = 0;
            foreach (var menu in dbMenus.Where(m => m.Parent == parentMenu))
            {
                menu.TreeLevel = level;
                menu.IsTreeLeaf = true;
                menu.Enabled = true;
                _menus.Add(menu);
                count++;

                // 递归子菜单
                level++;
                int childCount = ResolveMenuCollection(dbMenus, menu, level);
                if (childCount != 0)
                    menu.IsTreeLeaf = false;
                level--;
            }

            return count;
        }


        public static void Reload()
        {
            _menus = null;
        }

    }
}
