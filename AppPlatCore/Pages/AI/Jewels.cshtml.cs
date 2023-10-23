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
        public IActionResult OnPostFilePhoto_FileSelected(IFormFile filePhoto, string[] grid_fields)
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
                virtualPath = Common.GetUploadPath("Images");
                physicalPath = Asp.MapPath(virtualPath);
                Utils.IO.PrepareDirectory(physicalPath);
                using (var stream = new FileStream(physicalPath, FileMode.Create))
                    filePhoto.CopyTo(stream);

                UIHelper.Image("imgPhoto").ImageUrl(virtualPath + "?w=200");
                UIHelper.FileUpload("filePhoto").Reset();
            }


            // 图片归类
            var imgBytes = System.IO.File.ReadAllBytes(physicalPath);
            ModelInput data = new ModelInput() { ImageSource = imgBytes };
            this.Results = JewelsAI.PredictAllLabels(data).Take(10).ToList();

            //
            UIHelper.Grid("grid").DataSource(this.Results, grid_fields);
            Logger.Info("STOP Predicate:  {0}", DateTime.Now);
            return UIHelper.Result();
        }
    }
}