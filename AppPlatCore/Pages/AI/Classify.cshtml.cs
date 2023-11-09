using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using App.Components;
using App.DAL;
using App.Web;
using App.Utils;
using App.UIs;
using FineUICore;

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
    public class ClassifyModel : BaseAdminModel
    {
        /// <summary>头像图片上传处理</summary>
        public IActionResult OnPostFilePhoto_FileSelected(IFormFile filePhoto, string[] grid_fields)
        {
            if (filePhoto == null)
                return UIHelper.Result();

            string fileName = filePhoto.FileName;
            if (!UI.ValidateFileType(fileName))
            {
                // 图片无效
                UIHelper.FileUpload("filePhoto").Reset();
                UI.ShowNotify("无效的文件类型！");
                return UIHelper.Result();
            }
            else
            {
                // 上传图片
                var virtualPath = Uploader.GetUploadPath("Images");
                var physicalPath = Asp.MapPath(virtualPath);
                IO.PrepareDirectory(physicalPath);
                using (var stream = new FileStream(physicalPath, FileMode.Create))
                    filePhoto.CopyTo(stream);

                // 显示图片
                UIHelper.Image("imgPhoto").ImageUrl(virtualPath + "?w=200");
                UIHelper.FileUpload("filePhoto").Reset();

                // 图片 AI 识别归类
                Logger.Info("START Predicate: {0}", DateTime.Now);
                var imgBytes = System.IO.File.ReadAllBytes(physicalPath);
                ModelInput data = new ModelInput() { ImageSource = imgBytes };
                var results = ClassifyAI.PredictAllLabels(data).Take(5).ToList();
                Logger.Info("STOP Predicate:  {0}", DateTime.Now);

                // 将结果显示到客户端
                string script = BuildImagesRenderScript(results);
                FineUICore.PageContext.RegisterStartupScript(script);
                return UIHelper.Result();
            }
        }

        /// <summary>构造客户端显示查询结果图片的脚本</summary>
        private static string BuildImagesRenderScript(List<PredicateResult> results)
        {
            // 获取排名前几名所有的图片
            var pattern = new string[] { ".jpg", ".jpeg", ".png", ".tif" };
            List<ImageMath> images = new List<ImageMath>();
            foreach (var result in results)
            {
                var name = result.Label;
                var folder = Asp.MapPath(Path.Combine(ClassifyAI.TrainPath, result.Label));
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

            // 构造客户端脚本，显示查询结果
            var sb = new StringBuilder();
            sb.Append("<ul class=\"icons\">");
            foreach (var item in images)
            {
                sb.AppendFormat("<li class=\"f-state-default\"><a href=\"{0}\" target=\"_blank\"><img src=\"{0}?w=200\"/></a><div class=\"title\">{1}</div></li>", item.Path, item.Name);
            }
            sb.Append("</ul>");
            var html = sb.ToString();
            var script = string.Format("F.ui.Panel1.el.html('{0}');", html);  // PanelHelper 没有更新内容的方法，只能用这种方法更新
            return script;
        }
    }
}