using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using App.Models;
using App;

namespace App.Models
{
    public class DeptHelper
    {
        private static List<Dept> _depts;

        public static List<Dept> Depts
        {
            get
            {
                if (_depts == null)
                {
                    _depts = new List<Dept>();
                    var db = BaseModel.GetDbConnection();
                    List<Dept> dbDepts = db.Depts.OrderBy(d => d.SortIndex).ToList();
                    ResolveDeptCollection(dbDepts, null, 0);
                }
                return _depts;
            }
        }

        public static void Reload()
        {
            _depts = null;
        }

        private static int ResolveDeptCollection(List<Dept> items, Dept parentItem, int level)
        {
            int count = 0;
            foreach (var item in items.Where(d => d.Parent == parentItem))
            {
                item.TreeLevel = level;
                item.IsTreeLeaf = true;
                item.Enabled = true;
                _depts.Add(item);
                count++;

                // 递归子节点
                level++;
                int childCount = ResolveDeptCollection(items, item, level);
                if (childCount != 0)
                    item.IsTreeLeaf = false;
                level--;

            }

            return count;
        }

    }
}
