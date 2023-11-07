using App.DAL;
using App.Utils;
using App.Web;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace App.Components
{
    /// <summary>
    /// 文件相关辅助方法
    /// </summary>
    public class Uploader
    {

        /// <summary>获取上传文件要保存的虚拟路径</summary>
        /// <param name="folderName">文件夹名称</param>
        public static string GetUploadPath(string folderName, string fileName = ".png")
        {
            // 默认保存在 /Files/ 目录下
            // 如果 folderName 以/开头，则保存在 folderName 目录下
            string folder = string.Format("~/Files/{0}", folderName);
            if (folderName != null && folderName.StartsWith("/"))
                folder = folderName;

            // 合并目录和文件名
            string extension = fileName.GetFileExtension();
            string path = string.Format("{0}/{1}{2}", folder, new SnowflakeID().NewID(), extension);
            return path.TrimStart("~");
            //return Asp.ResolveUrl(path);
        }

        /// <summary>上传多张图片（Base64编码）</summary>
        /// <param name="folder">保存目录。如：Products</param>
        /// <param name="base64imageOrUrls">Base64 编码的图片字符串或Url数组</param>
        public static List<string> UploadBase64Images(string folder, params string[] base64imageOrUrls)
        {
            var urls = new List<string>();
            foreach (var text in base64imageOrUrls)
            {
                if (text.IsEmpty())
                    continue;

                // 如果是base64图片文本，则上传后记录url
                if (text.IsBase64Image())
                {
                    var url = Uploader.UploadBase64Image(folder, text);
                    if (!url.IsEmpty())
                        urls.Add(url);
                }
                // 否则直接记录url
                else
                {
                    urls.Add(text);
                }
            }
            return urls;
        }

        /// <summary>上传 base64 编码的图像</summary>
        static string UploadBase64Image(string folder, string imageText)
        {
            var image = Painter.ParseImage(imageText);
            if (image != null)
            {
                var url = GetUploadPath(folder);
                var path = Asp.MapPath(url);
                IO.PrepareDirectory(path);
                image.Save(path);  // waring: 仅在 windows 上受支持
                //using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                //    image.Save(fs, image.RawFormat); // waring: 仅在 windows 上受支持
                return url;
            }
            return "";
        }

        /// <summary>上传文件</summary>
        /// <param name="folder">上传目录</param>
        /// <param name="fileName">文件名。若为空，则自动生成文件名。</param>
        public static string UploadFile(IFormFile file, string folder, string fileName = "", bool checkExtension = true)
        {
            // 扩展名及校验
            string ext;
            if (file.FileName != "blob")
                ext = file.FileName.GetFileExtension();
            else
                ext = fileName.GetFileExtension();
            if (checkExtension)
            {
                var exts = SiteConfig.Instance.UpFileTypes.SplitString();
                if (!exts.Contains(ext))
                    throw new Exception("禁止上传该类型文件");
            }

            // 文件名和路径
            if (fileName.IsEmpty())
                fileName = string.Format("{0}{1}", SnowflakeID.Instance.NewID(), ext);
            var dir = folder.IsEmpty() ? "/Files/" : string.Format("/Files/{0}/", folder);

            // 保存
            var url = string.Format("{0}{1}", dir, fileName);
            var path = Asp.MapPath(url);
            IO.PrepareDirectory(path);
            //file.SaveAs(path);
            using (var stream = new FileStream(path, FileMode.Create))
                file.CopyTo(stream);
            return url;
        }

    }
}
