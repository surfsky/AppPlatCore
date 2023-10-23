using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using App.Components;
using App.Models;
using App.Web;
using FineUICore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace App.Pages.AI
{
    //[CheckPower("CoreUserEdit")]
    public class JewelsModel : BaseAdminModel
    {
        public List<PredicateResult> Results{ get; set; }


        /// <summary>头像图片上传处理</summary>
        public IActionResult OnPostFilePhoto_FileSelected(IFormFile filePhoto)
        {
            Logger.Info("START Predicate: {0}", DateTime.Now);
            if (filePhoto == null)
                return UIHelper.Result();

            // 保存上传的图片
            string fileName = filePhoto.FileName;
            string physicalPath = "";
            string virtualPath = "";
            if (!UI.ValidateFileType(fileName))
            {
                UIHelper.FileUpload("filePhoto").Reset();
                UI.ShowNotify("无效的文件类型！");
            }
            else
            {
                virtualPath = App.Api.Demo.GetUploadPath("Images");
                physicalPath = Asp.MapPath(virtualPath);

                //fileName = fileName.Replace(":", "_").Replace(" ", "_").Replace("\\", "_").Replace("/", "_");
                //fileName = DateTime.Now.Ticks.ToString() + "_" + fileName;
                //var folder = "~/Files/";
                //var folder2 = FineUICore.PageContext.MapPath(folder);
                //physicalPath = folder2 + fileName;
                //virtualPath = folder + fileName;
                App.Utils.IO.PrepareDirectory(physicalPath);
                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    filePhoto.CopyTo(stream);
                }

                UIHelper.Image("imgPhoto").ImageUrl(virtualPath + "?w=100");
                UIHelper.FileUpload("filePhoto").Reset();
            }


            // 图片归类
            var modelPath = Asp.MapPath("/Pages/AI/JewelsImages/model.zip");
            var imgBytes = System.IO.File.ReadAllBytes(physicalPath);
            ModelInput sampleData = new ModelInput() { ImageSource = imgBytes };
            JewelsAI.MLNetModelPath = modelPath;
            this.Results = JewelsAI.PredictAllLabels(sampleData).Take(10).ToList();

            //
            Logger.Info("STOP Predicate:  {0}", DateTime.Now);
            return UIHelper.Result();
        }
    }
}