using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using App.Components;
using App.Models;
using App.Web;
using App.Utils;
using FineUICore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace App.Pages.AI
{
    /// <summary>
    /// 图片匹配结果
    /// </summary>
    public class ImageMath
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public float Score { get; set; }
        public ImageMath(string name, string path, float score)
        {
            Name = name;
            Path = path;
            Score = score;
        }
    }

    //[CheckPower("CoreUserEdit")]
    public class JewelsModel : BaseAdminModel
    {
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


            // 图片 AI 识别归类
            var imgBytes = System.IO.File.ReadAllBytes(physicalPath);
            ModelInput data = new ModelInput() { ImageSource = imgBytes };
            var results = JewelsAI.PredictAllLabels(data).Take(10).ToList();
            Logger.Info("STOP Predicate:  {0}", DateTime.Now);


            // 将前几名所有的图片都展示出来
            var pattern = new string[] { ".jpg", ".jpeg", ".png", ".tif" };
            List<ImageMath> images = new List<ImageMath>();
            foreach (var result in results)
            {
                var name = result.Label;
                var folder = Asp.MapPath(Path.Combine(JewelsAI.TrainPath, result.Label));
                foreach (var file in Directory.GetFiles(folder))
                {
                    var ext = file.GetFileExtension();
                    if (pattern.Contains(ext))
                    {
                        var path = Asp.ToVirtualPath(file);
                        images.Add(new ImageMath(name, path, result.Score));
                    }
                }
            }

            // 转化为客户端执行的脚本
            var sb = new StringBuilder();
            sb.Append("<ul class=\"icons\">");
            foreach (var item in images)
            {
                sb.AppendFormat("<li class=\"f-state-default\"><a href=\"{0}\" target=\"_blank\"><img src=\"{0}?w=200\"/><div class=\"title\">{1}</div></a></li>", item.Path, item.Name);
            }
            sb.Append("</ul>");
            var html = sb.ToString();

            //
            //var panel = UIHelper.Panel("Panel1");  // panelhelper 没有更新内容的方法
            var script = string.Format("F.ui.Panel1.el.html('{0}');", html);
            FineUICore.PageContext.RegisterStartupScript(script);
            return UIHelper.Result();
        }
    }
}