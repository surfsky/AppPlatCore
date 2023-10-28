﻿using App.Components;
using App.DAL;
using App.Utils;
using App.Web;
using FineUICore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Pages
{
    [Authorize]
    public class IndexModel : BaseModel
    {
        public TreeNode[] MenuTreeNodes { get; set; } = new TreeNode[0];
        public string UserName { get; set; }
        public string OnlineUserCount { get; set; }
        public string ProductVersion { get; set; }
        public string ConfigTitle { get; set; }
        public FineUICore.Menu SystemHelpMenu { get; set; } = new FineUICore.Menu();

        public async Task OnGetAsync()
        {
            // 用户菜单
            List<DAL.Menu> menus = ResolveUserMenuList();
            if (menus.Count == 0)
            {
                UI.ShowNotify("系统管理员尚未给你配置菜单！");
                return;
            }
            MenuTreeNodes = GetTreeNodes(menus).ToArray();

            //
            UserName = GetIdentityName();
            OnlineUserCount =  (await Auth.GetOnlineCountAsync()).ToString();
            ProductVersion = Common.GetVersion();
            ConfigTitle = SiteConfig.Instance.Title;   //"AppPlat";
            SystemHelpMenu = GetSystemHelpMenu();  // 动态组织客户端菜单
        }

        // 帮助菜单
        private FineUICore.Menu GetSystemHelpMenu()
        {
            FineUICore.Menu menu = new FineUICore.Menu();
            JArray ja = JArray.Parse(SiteConfig.Instance.HelpList);
            foreach (JObject jo in ja)
            {
                string text = jo.Value<string>("Text");
                Icon icon = IconHelper.String2Icon(jo.Value<string>("Icon"), true);
                string id = jo.Value<string>("ID");
                string url = jo.Value<string>("URL");
                if (!String.IsNullOrEmpty(text) && !String.IsNullOrEmpty(id) && !String.IsNullOrEmpty(url))
                {
                    FineUICore.MenuButton menuItem = new FineUICore.MenuButton();
                    menuItem.Text = text;
                    menuItem.Icon = icon;
                    menuItem.OnClientClick = String.Format("addExampleTab('{0}','{1}','{2}')", id, Url.Content(url), text);
                    menu.Items.Add(menuItem);
                }
            }

            return menu;
        }


        #region GetTreeNodes

        /// <summary>创建树菜单</summary>
        private IList<TreeNode> GetTreeNodes(List<DAL.Menu> menus)
        {
            IList<TreeNode> nodes = new List<TreeNode>();
            ResolveMenuTree(menus, null, nodes);
            nodes[0].Expanded = true; // 展开第一个树节点
            return nodes;
        }

        /// <summary>生成菜单树</summary>
        private int ResolveMenuTree(List<DAL.Menu> menus, int? parentMenuID, IList<TreeNode> nodes)
        {
            int count = 0;
            foreach (var menu in menus.Where(m => m.ParentID == parentMenuID))
            {
                TreeNode node = new TreeNode();
                nodes.Add(node);
                count++;

                node.Text = menu.Name;
                node.IconUrl = menu.ImageUrl;
                if (menu.Target.IsNotEmpty())
                    node.Target = menu.Target;
                if (menu.NavigateUrl.IsNotEmpty())
                    node.NavigateUrl = Url.Content(menu.NavigateUrl);
                node.Expanded = menu.Expanded;

                if (menu.IsTreeLeaf)
                {
                    // 如果是叶子节点，但不是超链接，则是空目录，删除
                    node.Leaf = true;
                    if (String.IsNullOrEmpty(menu.NavigateUrl))
                    {
                        nodes.Remove(node);
                        count--;
                    }
                }
                else
                {
                    // 如果是目录，但是计算的子节点数为0，可能目录里面的都是空目录，则要删除此父目录
                    int childCount = ResolveMenuTree(menus, menu.ID, node.Nodes);
                    if (childCount == 0 && String.IsNullOrEmpty(menu.NavigateUrl))
                    {
                        nodes.Remove(node);
                        count--;
                    }
                }

            }

            return count;
        }

        #endregion

        #region ResolveUserMenuList

        // 获取用户可用的菜单列表
        private List<DAL.Menu> ResolveUserMenuList()
        {
            // 当前登陆用户的权限列表
            List<string> rolePowerNames = GetRolePowerNames();

            // 当前用户所属角色可用的菜单列表
            List<DAL.Menu> menus = new List<DAL.Menu>();

            foreach (var menu in MenuHelper.Menus)
            {
                // 如果此菜单不属于任何模块，或者此用户所属角色拥有对此模块的权限
                if (menu.ViewPowerID == null || rolePowerNames.Contains(menu.ViewPower.Name))
                {
                    menus.Add(menu);
                }
            }

            return menus;
        }

        #endregion

        #region btnSignOut_Click
        public async Task<IActionResult> OnPostBtnSignOut_ClickAsync()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }


        #endregion
    }
}
