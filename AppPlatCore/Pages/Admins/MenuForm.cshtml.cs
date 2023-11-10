using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Components;
using App.DAL;
using App.UIs;
using App.Utils;

using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace App.Pages.Admin
{
    [CheckPower(Power.ConfigMenu)]
    [Auth(Power.ConfigMenu)]
    public class MenuFormModel : BaseAdminModel
    {
        [BindProperty]
        public DAL.Menu Menu { get; set; }
        public IEnumerable<DAL.Menu> Menus { get; set; }
        public RadioItem[] IconItems { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            IconItems = MenuEdit_GetIconItems().ToArray();
            if (id == 0)
            {
                // new
                Menus = UI.ResolveDDL<DAL.Menu>(MenuHelper.Menus).ToArray();
            }
            else
            {
                // edit
                Menu = await DB.Menus
                    .Include(m => m.Parent)
                    //.Include(m => m.ViewPower)
                    .Where(m => m.ID == id).FirstOrDefaultAsync();
                if (Menu == null)
                    return Content("无效参数！");
                Menus = UI.ResolveDDL<DAL.Menu>(MenuHelper.Menus, id).ToArray();
            }

            return Page();
        }


        public List<RadioItem> MenuEdit_GetIconItems()
        {
            List<RadioItem> items = new List<RadioItem>();
            string[] icons = new string[] { "page", "application", "tag_blue", "folder", "door_out", "outline", "cog", "user", "chart_bar", "chart_organisation", "clipboard", "map"};
            foreach (string icon in icons)
            {
                string value = String.Format("~/res/icon/{0}.png", icon);
                string text = String.Format("<img style=\"vertical-align:bottom;\" src=\"{0}\" />&nbsp;{1}", Url.Content(value), icon);
                items.Add(new RadioItem(text, value));
            }
            return items;
        }

        public async Task<IActionResult> OnPostMenuEdit_btnSaveClose_ClickAsync(string powerName)
        {
            ModelState.Remove("Menu.ID");
            if (ModelState.IsValid)
            {
                // 下拉列表的顶级节点值为-1
                if (Menu.ParentID == -1)
                    Menu.ParentID = null;
                Menu.Power = powerName.ParseEnum<Power>();

                if (Menu.ID == 0)
                    DB.Menus.Add(Menu);
                else
                    DB.Entry(Menu).State = EntityState.Modified;
                await DB.SaveChangesAsync();
                ActiveWindow.HidePostBack();
            }

            return UIHelper.Result();

        }
    }
}