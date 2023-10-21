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
    [CheckPower("CoreConfigView")]
    public class ConfigModel : BaseAdminModel
    {
        public bool PowerCoreConfigEdit { get; set; }

        public SiteConfig Config { get; set; }
        public string HelpListText { get; set; }

        public async Task OnGetAsync()
        {
            PowerCoreConfigEdit = CheckPower("CoreConfigEdit");
            Config = SiteConfig.Instance; //.Set.FirstOrDefault(); //.Instance;
            await Task.Run(() =>
            {
                JSBeautifyLib.JSBeautify jsb = new JSBeautifyLib.JSBeautify(Config.HelpList, new JSBeautifyLib.JSBeautifyOptions());
                HelpListText = jsb.GetResult();
            });
        }


        public IActionResult OnPostConfig_btnSave_OnClick(int ddlPageSize, string tbxHelpList, string tbTitle, string tbIcon, string tbLoginBg, string tbBeiAnNo)
        {
            // 在操作之前进行权限检查
            if (!CheckPower("CoreConfigEdit"))
            {
                Auth.CheckPowerFailWithAlert();
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

            // 保存配置
            var cfg = SiteConfig.Set.FirstOrDefault();
            cfg.PageSize = ddlPageSize;
            cfg.HelpList = tbxHelpList;
            cfg.Title = tbTitle;
            cfg.LoginBg = tbLoginBg;
            cfg.Icon = tbIcon;
            cfg.BeiAnNo = tbBeiAnNo;
            cfg.Save();

            // 刷新页面
            FineUICore.PageContext.RegisterStartupScript("top.window.location.reload(false);");
            return UIHelper.Result();
        }
    }
}