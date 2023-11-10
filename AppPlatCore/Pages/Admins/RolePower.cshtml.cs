using App.Components;
using App.DAL;
using App.Utils;
using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace App.Pages.Admin
{
    /// <summary>分组权限列表</summary>
    public class GroupPowers
    {
        [Display(Name = "分组名称")]
        public string GroupName { get; set; }

        [Display(Name = "权限列表")]
        public JArray Powers { get; set; }

    }

    [CheckPower(Power.RolePowerEdit)]
    public class RolePowerModel : BaseAdminModel
    {
        public IEnumerable<Role> Roles { get; set; }
        public IEnumerable<GroupPowers> GroupPowers { get; set; }
        public PagingInfo Grid1PagingInfo { get; set; }
        public PagingInfo Grid2PagingInfo { get; set; }
        public bool PowerCoreRolePowerEdit { get; set; }
        public string Grid1SelectedRowID { get; set; }
        public string RolePowerIds { get; set; }

        // GET
        public async Task<IActionResult> OnGetAsync()
        {
            this.PowerCoreRolePowerEdit = CheckPower(Power.RolePowerEdit);

            // 表格1
            this.Grid1PagingInfo = new PagingInfo("Name", false);
            this.Roles = await Sort<Role>(DB.Roles, Grid1PagingInfo).ToListAsync();
            if (Roles.Count() == 0)
                return Content("请先添加角色！");
            var grid1SelectedRowID = Roles.First().ID;
            Grid1SelectedRowID = grid1SelectedRowID.ToString();

            // 表格2
            this.RolePowerIds = await GetRolePowersAsync(grid1SelectedRowID);  // 当前选中角色拥有的权限列表
            this.Grid2PagingInfo = new PagingInfo("GroupName", false); ;
            this.GroupPowers = await GetGroupPowersAsync(Grid2PagingInfo);

            return Page();
        }


        /// <summary>获取当前选中角色拥有的权限列表</summary>
        private async Task<string> GetRolePowersAsync(long roleID)
        {
            var items = RolePower.Set.Where(t => t.RoleID == roleID).Select(t => t.PowerID).ToList();
            return new JArray(items).ToString(Newtonsoft.Json.Formatting.None);
        }


        /// <summary>获取分组权限清单</summary>
        private async Task<IEnumerable<GroupPowers>> GetGroupPowersAsync(PagingInfo pagingInfo)
        {
             // 权限组
            var groupNames = typeof(Power).GetEnumGroups();
            if (pagingInfo.SortField == "GroupName")
            {
                if (pagingInfo.SortDirection == "ASC")
                    groupNames.Sort();
                else
                    groupNames.Reverse();
            }


            // 遍历权限组，获取所有权限，并转化为Json对象
            var infos = typeof(Power).GetEnumInfos();
            var groupPowers = new List<GroupPowers>();
            foreach (var groupName in groupNames)
            {
                var groupPower = new GroupPowers() { GroupName = groupName };
                var items = infos.Where(t => t.Group == groupName);
                JArray ja = new JArray();
                foreach (var powerItem in items)
                {
                    JObject jo = new JObject();
                    jo.Add("id", powerItem.ID);
                    jo.Add("name", powerItem.Title);
                    jo.Add("title", powerItem.Title);
                    ja.Add(jo);
                }
                groupPower.Powers = ja;
                groupPowers.Add(groupPower);
            }

            return groupPowers;
        }


        /// <summary>左侧角色网格排序</summary>
        public async Task<IActionResult> OnPostRolePower_Grid1_Sort(string[] Grid1_fields, string Grid1_sortField, string Grid1_sortDirection)
        {
            var grid1UI = UIHelper.Grid("Grid1");
            var pagingInfo = new PagingInfo(Grid1_sortField, Grid1_sortDirection);
            grid1UI.DataSource(await Sort<Role>(DB.Roles, pagingInfo).ToListAsync(), Grid1_fields, clearSelection: false);
            return UIHelper.Result();
        }

        /// <summary>右侧权限网格处理</summary>
        public async Task<IActionResult> OnPostRolePower_Grid2_DoPostBackAsync(
            string[] Grid2_fields, string Grid2_sortField, string Grid2_sortDirection,
            string actionType, long selectedRoleID, long[] selectedPowerIDs)
        {
            // 保存角色权限时，不需要重新加载表格数据
            if (actionType == "saveall")
            {
                // 在操作之前进行权限检查
                if (!CheckPower(Power.RolePowerEdit))
                {
                    Auth.CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }

                // 更新当前角色新的权限列表
                RolePower.SetRolePowers(selectedRoleID, selectedPowerIDs.ToList());
                FineUICore.PageContext.RegisterStartupScript("updateRolePowers(" + await GetRolePowersAsync(selectedRoleID) + ");");
                Alert.ShowInTop("当前角色的权限更新成功！");
            }
            else
            {
                // 排序
                var grid2UI = UIHelper.Grid("Grid2");
                var pagingInfo = new PagingInfo(Grid2_sortField, Grid2_sortDirection);
                grid2UI.DataSource(await GetGroupPowersAsync(pagingInfo), Grid2_fields);
            }

            return UIHelper.Result();
        }

    }
}