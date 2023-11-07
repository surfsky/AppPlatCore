using App.Components;
using App.DAL;
using App.Utils;
using FineUICore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace App.Pages.Admin
{
    [CheckPower("CoreUserNew")]
    public class UserNewModel : BaseAdminModel
    {
        [BindProperty]
        public User CurrentUser { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostUserNew_btnSaveClose_ClickAsync(string hfSelectedDept, string hfSelectedRole)
        {
            if (ModelState.IsValid)
            {
                var _user = await DB.Users.Where(u => u.Name == CurrentUser.Name).FirstOrDefaultAsync();
                if (_user != null)
                {
                    Alert.Show("用户 " + CurrentUser.Name + " 已经存在！");
                    return UIHelper.Result();
                }

                // user
                CurrentUser.Password = PasswordUtil.CreateDbPassword(CurrentUser.Password.Trim());
                CurrentUser.CreateTime = DateTime.Now;
                if (hfSelectedDept.IsNotEmpty())
                    CurrentUser.DeptID = Convert.ToInt32(hfSelectedDept);
                if (hfSelectedRole.IsNotEmpty())
                {
                    int[] roleIDs = Components.StringUtil.GetIntArrayFromString(hfSelectedRole);
                    AddEntities2<RoleUser>(roleIDs, CurrentUser.ID);
                }

                DB.Users.Add(CurrentUser);
                await DB.SaveChangesAsync();
                ActiveWindow.HidePostBack(); // 关闭本窗体（触发窗体的关闭事件）
            }

            return UIHelper.Result();
        }
    }
}