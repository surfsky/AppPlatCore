﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FineUICore;


namespace App.Pages.Tests
{
    public class FormDynamicModel : BaseModel
    {
        public void OnGet()
        {
            InitFormRows();
        }

        private void InitFormRows()
        {
            List<Field> fields = new List<Field>();

            var tbxUser = new TextBox();
            tbxUser.ID = "tbxUserName";
            tbxUser.Text = "";
            tbxUser.Label = "用户名";
            tbxUser.ShowLabel = true;
            tbxUser.ShowRedStar = true;
            tbxUser.Required = true;
            tbxUser.EmptyText = "请输入用户名";
            fields.Add(tbxUser);

            var ddlGender = new FineUICore.DropDownList();
            ddlGender.ID = "ddlGender";
            ddlGender.Label = "性别（回发事件）";
            ddlGender.Items.Add("男", "0");
            ddlGender.Items.Add("女", "1");
            ddlGender.AutoSelectFirstItem = false;
            // 添加后台事件处理函数
            ddlGender.Events.Add(new Event("change", Url.Handler("ddlGender_SelectedIndexChanged"), "ddlGender"));
            fields.Add(ddlGender);

            ViewBag.DynamicItems = fields.ToArray();
        }


        public IActionResult OnPostDdlGender_SelectedIndexChanged(string ddlGender_text)
        {
            ShowNotify("选择的性别：" + ddlGender_text);
            return UIHelper.Result();
        }

        public IActionResult OnPostButton1_Click(string tbxUserName, string ddlGender_text)
        {
            ShowNotify("用户名：" + tbxUserName + "  性别：" + ddlGender_text);
            return UIHelper.Result();
        }
    }
}