using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.Models;

using FineUICore;
using Newtonsoft.Json.Linq;

namespace App.Pages.Admin
{
    [CheckPower(Name = "CoreConfigView")]
    public class ConfigModel : BaseAdminModel
    {
        public bool PowerCoreConfigEdit { get; set; }
        public string HelpListText { get; set; }
        public string SiteTitle { get; set; }
        public string SiteIcon { get; set; }

        public SiteConfig Config { get; set; }

        public async Task OnGetAsync()
        {
            PowerCoreConfigEdit = CheckPower("CoreConfigEdit");
            Config = SiteConfig.Instance;
            SiteTitle = SiteConfig.Instance.Title;
            SiteIcon = SiteConfig.Instance.Icon;
            await Task.Run(() =>
            {
                JSBeautifyLib.JSBeautify jsb = new JSBeautifyLib.JSBeautify(SiteConfig.Instance.HelpList, new JSBeautifyLib.JSBeautifyOptions());
                HelpListText = jsb.GetResult();
            });
        }


        public IActionResult OnPostConfig_btnSave_OnClick(int ddlPageSize, string tbxHelpList, string tbTitle, string tbIcon, string tbLoginBg, string tbBeiAnNo)
        {
            // 在操作之前进行权限检查
            if (!CheckPower("CoreConfigEdit"))
            {
                CheckPowerFailWithAlert();
                return UIHelper.Result();
            }

            try
            {
                JArray.Parse(tbxHelpList.Trim());
            }
            catch (Exception)
            {
                UIHelper.TextArea("tbxHelpList").MarkInvalid("格式不正确，必须是JSON字符串！");
                return UIHelper.Result();
            }

            SiteConfig.Instance.PageSize = ddlPageSize;
            SiteConfig.Instance.HelpList = tbxHelpList;
            SiteConfig.Instance.Title = tbTitle;
            SiteConfig.Instance.LoginBg = tbLoginBg;
            SiteConfig.Instance.Icon = tbIcon;
            SiteConfig.Instance.BeiAnNo = tbBeiAnNo;
            SiteConfig.Instance.Save();

            FineUICore.PageContext.RegisterStartupScript("top.window.location.reload(false);");

            return UIHelper.Result();
        }
    }
}