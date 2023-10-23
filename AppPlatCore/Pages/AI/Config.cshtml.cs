using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.Models;

using FineUICore;
using Newtonsoft.Json.Linq;
using App.Components;

namespace App.Pages.AI
{
    //[CheckPower("CoreConfigView")]
    public class ConfigModel : BaseAdminModel
    {
        public bool PowerCoreConfigEdit { get; set; }
        public string TrainPath { get; set; }
        public string ModelPath { get; set; }

        public void OnGet()
        {
            PowerCoreConfigEdit = CheckPower("CoreConfigEdit");
            TrainPath = JewelsAI.TrainPath;
            ModelPath = JewelsAI.ModelPath;
        }


        public IActionResult OnPostConfig_btnBuild_OnClick()
        {
            JewelsAI.BuildModel();
            UI.ShowNotify("模型创建成功");
            return UIHelper.Result();
        }
    }
}