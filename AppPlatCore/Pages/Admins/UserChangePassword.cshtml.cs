﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Components;
using App.DAL;

using FineUICore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace App.Pages.Admin
{
    [CheckPower("CoreUserChangePassword")]
    public class UserChangePasswordModel : BaseAdminModel
    {
        public User CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            CurrentUser = await DB.Users.Where(m => m.ID == id).AsNoTracking().FirstOrDefaultAsync();
            if (CurrentUser == null)
                return Content("无效参数！");

            if (CurrentUser.Name == "admin" && GetIdentityName() != "admin")
                return Content("你无权编辑超级管理员！");

            return Page();
        }

        public async Task<IActionResult> OnPostUserChangePassword_btnSaveClose_ClickAsync(int hfUserID, string tbxPassword)
        {
            var item = await DB.Users.FindAsync(hfUserID);
            item.Password = PasswordUtil.CreateDbPassword(tbxPassword.Trim());
            await DB.SaveChangesAsync();
            ActiveWindow.HidePostBack();
            return UIHelper.Result();
        }
    }
}