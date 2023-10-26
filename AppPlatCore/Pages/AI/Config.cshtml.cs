using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.DAL;

using FineUICore;
using Newtonsoft.Json.Linq;
using App.Components;
using App.Web;

namespace App.Pages.AI
{
    //[CheckPower("CoreConfigView")]
    public class ConfigModel : BaseAdminModel
    {
        public bool PowerCoreConfigEdit { get; set; }
        public string TrainPath { get; set; }
        public string ModelPath { get; set; }
        public string FtpPath { get; set; } = "ftp://test.com/";

        public void OnGet()
        {
            PowerCoreConfigEdit = CheckPower("CoreConfigEdit");
            TrainPath = Asp.MapPath(JewelsAI.TrainPath);
            ModelPath = Asp.MapPath(JewelsAI.ModelPath);
        }

        /// <summary>页面方法进行重新模型。该方法较为耗时，已改为用 signalR 来实现</summary>
        public IActionResult OnPostConfig_btnBuild_OnClick()
        {
            JewelsAI.BuildModel();
            UI.ShowNotify("模型创建成功");
            return UIHelper.Result();
        }
    }
}