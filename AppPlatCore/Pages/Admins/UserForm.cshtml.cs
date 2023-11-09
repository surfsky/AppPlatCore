﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using App.Components;
using App.DAL;
using App.Web;
using App.UIs;
using FineUICore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using App.Utils;

namespace App.Pages.Admin
{
    [CheckPower("CoreUserEdit")]
    public class UserFormModel : BaseAdminModel
    {
        [BindProperty]
        public User CurrentUser { get; set; }

        // 部门、角色、职务
        public string SelectedDeptName { get; set; }
        public string SelectedDeptID { get; set; }
        public string SelectedRoleNames { get; set; }
        public string SelectedRoleIDs { get; set; }



        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id != 0)
            {
                CurrentUser = await DB.Users
                    .Include(u => u.Dept)
                    .Include(u => u.RoleUsers)
                    .ThenInclude(ru => ru.Role)
                    .Where(m => m.ID == id).AsNoTracking().FirstOrDefaultAsync();
                if (CurrentUser == null)
                    return Content("无效参数！");
                if (CurrentUser.Name == "admin" && GetIdentityName() != "admin")
                    return Content("你无权编辑超级管理员！");

                // 用户所属角色
                SelectedRoleNames = String.Join(",", CurrentUser.RoleUsers.Select(ru => ru.Role.Name).ToArray());
                SelectedRoleIDs = String.Join(",", CurrentUser.RoleUsers.Select(ru => ru.RoleID).ToArray());

                // 用户所属部门
                if (CurrentUser.Dept != null)
                {
                    SelectedDeptName = CurrentUser.Dept.Name;
                    SelectedDeptID = CurrentUser.Dept.ID.ToString();
                }
            }


            return Page();
        }

        /// <summary>头像图片上传处理</summary>
        public IActionResult OnPostFilePhoto_FileSelected(IFormFile filePhoto, IFormCollection values)
        {
            if (filePhoto != null)
            {
                string fileName = filePhoto.FileName;
                if (!UI.ValidateFileType(fileName))
                {
                    UIHelper.FileUpload("filePhoto").Reset();
                    UI.ShowNotify("无效的文件类型！");
                }
                else
                {
                    var virtualPath = Uploader.GetUploadPath("Images");
                    var physicalPath = Asp.MapPath(virtualPath);
                    Utils.IO.PrepareDirectory(physicalPath);
                    using (var stream = new FileStream(physicalPath, FileMode.Create))
                        filePhoto.CopyTo(stream);

                    UIHelper.Image("imgPhoto").ImageUrl(virtualPath + "?w=100");
                    UIHelper.FileUpload("filePhoto").Reset();
                }
            }

            return UIHelper.Result();
        }



        //public async Task<IActionResult> OnPostbtnSaveClose_ClickAsync(string hfSelectedDept, string hfSelectedRole, string hfSelectedTitle, IFormCollection values)
        public async Task<IActionResult> OnPostUserEdit_btnSaveClose_ClickAsync(string hfSelectedDept, string hfSelectedRole, IFormCollection values)
        {
            // 不对 Name 和 Password 进行模型验证，不保存
            ModelState.Remove("CurrentUser.ID");
            ModelState.Remove("Name");
            ModelState.Remove("Password");

            //string hfSelectedDept  = values["hfSelectedDept"].ToString();
            //string hfSelectedRole  = values["hfSelectedRole"].ToString();
            //string hfSelectedTitle = values["hfSelectedTitle"].ToString();
            string imgPhotoUrl     = values["imgPhotoUrl"].ToString();
            CurrentUser.Photo = imgPhotoUrl.TrimQuery();
            if (ModelState.IsValid)
            {
                if (CurrentUser.ID == 0)
                {
                    // 新增
                    var _user = await DB.Users.Where(u => u.Name == CurrentUser.Name).FirstOrDefaultAsync();
                    if (_user != null)
                    {
                        Alert.Show("用户 " + CurrentUser.Name + " 已经存在！");
                        return UIHelper.Result();
                    }

                    // user
                    CurrentUser.Password = PasswordUtil.CreateDbPassword(CurrentUser.Password.Trim());
                    CurrentUser.CreateTime = DateTime.Now;

                    if (hfSelectedRole.IsNotEmpty())
                    {
                        int[] roleIDs = Components.StringUtil.GetIntArrayFromString(hfSelectedRole);
                        AddEntities2<RoleUser>(roleIDs, CurrentUser.ID);
                    }
                    if (hfSelectedDept.IsNotEmpty())
                        CurrentUser.DeptID = Convert.ToInt32(hfSelectedDept);
                    DB.Users.Add(CurrentUser);
                }
                else
                {
                    // 更新
                    // 更新部分字段（先从数据库检索用户，再覆盖用户输入值，注意不更新Name，Password，CreateTime等字段）
                    var item = DB.Users
                        .Include(u => u.Dept)
                        .Include(u => u.RoleUsers)
                        .Where(m => m.ID == CurrentUser.ID).FirstOrDefault();
                    item.ChineseName = CurrentUser.ChineseName;
                    item.Gender = CurrentUser.Gender;
                    item.Enabled = CurrentUser.Enabled;
                    item.Email = CurrentUser.Email;
                    item.CompanyEmail = CurrentUser.CompanyEmail;
                    item.OfficePhone = CurrentUser.OfficePhone;
                    item.OfficePhoneExt = CurrentUser.OfficePhoneExt;
                    item.HomePhone = CurrentUser.HomePhone;
                    item.CellPhone = CurrentUser.CellPhone;
                    item.Remark = CurrentUser.Remark;
                    item.Photo = CurrentUser.Photo;
                    item.Title = CurrentUser.Title;

                    // role, dept
                    int[] roleIDs = Components.StringUtil.GetIntArrayFromString(hfSelectedRole);
                    ReplaceEntities2<RoleUser>(item.RoleUsers, roleIDs, item.ID);
                    if (hfSelectedDept.IsEmpty())
                        item.DeptID = null;
                    else
                        item.DeptID = Convert.ToInt32(hfSelectedDept);

                }


                await DB.SaveChangesAsync();
                ActiveWindow.HidePostBack();
            }

            return UIHelper.Result();
        }
    }
}