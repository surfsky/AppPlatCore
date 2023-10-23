﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using App.Components;
using App.HttpApi;
using App.Utils;
using App.Web;

namespace App.Api
{
    [Description("HttpApi Demo")]
    [HttpApi.Script(CacheDuration =0, ClassName ="Demo", NameSpace ="App")]
    [HttpApi.History("2016-11-01", "SURFSKY", "History log1")]
    [HttpApi.History("2019-08-15", "SURFSKY", "Fix token")]
    public partial class Demo
    {
        //---------------------------------------------
        // 静态方法
        //---------------------------------------------
        [HttpApi("HelloWorld")]
        public static string HelloWorld(string info)
        {
            System.Threading.Thread.Sleep(200);
            return string.Format("Hello world! {0} {1}", info, DateTime.Now);
        }

        [HttpApi("TestSession")]
        public static string TestSession(string info)
        {
            Asp.SetSession("info", info);
            return Asp.GetSession<string>("info");
        }

        [HttpApi("静态方法示例", Type = ResponseType.JSON)]
        public static object GetStaticObject()
        {
            return new { h = "3", a = "1", b = "2", c = "3" };
        }

        [HttpApi("Json结果包裹器示例", Wrap = true, WrapCondition ="获取数据成功")]
        public static object TestWrap()
        {
            return new { h = "3", a = "1", b = "2", c = "3" };
        }

        [HttpApi("默认方法参数示例", Remark = "p2的默认值为a", Status = ApiStatus.Delete, AuthVerbs ="GET")]
        public static object TestDefaultParameter(string p1, string p2="a")
        {
            return new { p1 = p1, p2 = p2};
        }

        [HttpApi("测试错误")]
        public static object TestError()
        {
            int n = 0;
            int m = 1 / n;
            return true;
        }

        [HttpApi("限制访问方式", AuthVerbs ="Post")]
        public static string TestVerbs()
        {
            return Asp.Current.Request.Method;
        }

        [HttpApi("测试可空枚举")]
        public static Sex? GetNullalbeEnum(Sex? sex=Sex.Male)
        {
            return sex;
        }

        [HttpApi("Upload file", PostFile=true)]
        [HttpParam("folder", "file folder, eg. Articles")]
        [HttpParam("fileName", "file name, eg. a.png")]
        public APIResult Up(string folder, string fileName)
        {
            var exts = new List<string> { ".jpg", ".png", ".gif", ".mp3", ".mp4", ".txt", ".md" };
            var ext = fileName.GetFileExtension();
            if (!exts.Contains(ext))
                return new APIResult(false, "File deny", 13);

            // 构造存储路径
            var url = Common.GetUploadPath(folder, fileName);
            var path = Asp.MapPath(url);
            var fi = new FileInfo(path);
            if (!fi.Directory.Exists)
                Directory.CreateDirectory(fi.Directory.FullName);

            // 存储第一个文件
            var files = Asp.Request.Form.Files;
            if (files.Count == 0)
                return new APIResult(false, "File doesn't exist", 11);
            using (var stream = File.Create(path))
                files[0].CopyToAsync(stream);
            return new APIResult(true, url);
        }


    }
}
