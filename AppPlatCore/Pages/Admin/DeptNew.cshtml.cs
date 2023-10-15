﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Components;
using App.Models;
using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Pages.Admin
{
    [CheckPower(Name = "CoreDeptNew")]
    public class DeptNewModel : BaseAdminModel
    {
        public IEnumerable<Dept> Depts { get; set; }

        [BindProperty]
        public Dept Dept { get; set; }

        public void OnGet()
        {
            Depts = UI.ResolveDDL<Dept>(DeptHelper.Depts).ToArray();
        }

        public async Task<IActionResult> OnPostDeptNew_btnSaveClose_ClickAsync()
        {
            if (ModelState.IsValid)
            {
                // 下拉列表的顶级节点值为-1
                if (Dept.ParentID == -1)
                    Dept.ParentID = null;

                DB.Depts.Add(Dept);
                await DB.SaveChangesAsync();

                // 关闭本窗体（触发窗体的关闭事件）
                ActiveWindow.HidePostBack();
            }

            return UIHelper.Result();
        }
    }
}