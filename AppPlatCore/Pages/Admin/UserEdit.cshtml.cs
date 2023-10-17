using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using App.Components;
using App.Models;

using FineUICore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace App.Pages.Admin
{
    [CheckPower(Name = "CoreUserEdit")]
    public class UserEditModel : BaseAdminModel
    {
        [BindProperty]
        public User CurrentUser { get; set; }

        // 部门、角色、职务
        public string SelectedDeptName { get; set; }
        public string SelectedDeptID { get; set; }
        public string SelectedRoleNames { get; set; }
        public string SelectedRoleIDs { get; set; }
        public string SelectedTitleNames { get; set; }
        public string SelectedTitleIDs { get; set; }





        public async Task<IActionResult> OnGetAsync(int id)
        {
            CurrentUser = await DB.Users
                .Include(u => u.Dept)
                .Include(u => u.RoleUsers)
                .ThenInclude(ru => ru.Role)
                .Include(u => u.TitleUsers)
                .ThenInclude(tu => tu.Title)
                .Where(m => m.ID == id).AsNoTracking().FirstOrDefaultAsync();
            if (CurrentUser == null)
                return Content("无效参数！");
            if (CurrentUser.Name == "admin" && GetIdentityName() != "admin")
                return Content("你无权编辑超级管理员！");

            // 用户所属角色
            SelectedRoleNames = String.Join(",", CurrentUser.RoleUsers.Select(ru => ru.Role.Name).ToArray());
            SelectedRoleIDs = String.Join(",", CurrentUser.RoleUsers.Select(ru => ru.RoleID).ToArray());

            // 用户拥有职称
            SelectedTitleNames = String.Join(",", CurrentUser.TitleUsers.Select(tu => tu.Title.Name).ToArray());
            SelectedTitleIDs = String.Join(",", CurrentUser.TitleUsers.Select(tu => tu.TitleID).ToArray());


            // 用户所属部门
            if (CurrentUser.Dept != null)
            {
                SelectedDeptName = CurrentUser.Dept.Name;
                SelectedDeptID = CurrentUser.Dept.ID.ToString();
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
                    fileName = fileName.Replace(":", "_").Replace(" ", "_").Replace("\\", "_").Replace("/", "_");
                    fileName = DateTime.Now.Ticks.ToString() + "_" + fileName;
                    var folder = "~/upload/";
                    var folder2 = FineUICore.PageContext.MapWebPath(folder);
                    App.Utils.IO.PrepareDirectory(folder2);
                    using (var stream = new FileStream(folder2 + fileName, FileMode.Create))
                    {
                        filePhoto.CopyTo(stream);
                    }

                    UIHelper.Image("imgPhoto").ImageUrl(folder + fileName);
                    UIHelper.FileUpload("filePhoto").Reset();
                }
            }

            return UIHelper.Result();
        }



        //public async Task<IActionResult> OnPostbtnSaveClose_ClickAsync(string hfSelectedDept, string hfSelectedRole, string hfSelectedTitle, IFormCollection values)
        public async Task<IActionResult> OnPostUserEdit_btnSaveClose_ClickAsync(string hfSelectedDept, string hfSelectedRole, string hfSelectedTitle, IFormCollection values)
        {
            // 不对 Name 和 Password 进行模型验证???
            ModelState.Remove("Name");
            ModelState.Remove("Password");

            //string hfSelectedDept  = values["hfSelectedDept"].ToString();
            //string hfSelectedRole  = values["hfSelectedRole"].ToString();
            //string hfSelectedTitle = values["hfSelectedTitle"].ToString();
            string imgPhotoUrl     = values["imgPhotoUrl"].ToString();


            if (ModelState.IsValid)
            {
                // 更新部分字段（先从数据库检索用户，再覆盖用户输入值，注意没有更新Name，Password，CreateTime等字段）
                var item = DB.Users
                    .Include(u => u.Dept)
                    .Include(u => u.RoleUsers)
                    .Include(u => u.TitleUsers)
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
                item.Photo = imgPhotoUrl;


                int[] roleIDs = StringUtil.GetIntArrayFromString(hfSelectedRole);
                ReplaceEntities2<RoleUser>(item.RoleUsers, roleIDs, item.ID);

                int[] titleIDs = StringUtil.GetIntArrayFromString(hfSelectedTitle);
                ReplaceEntities2<TitleUser>(item.TitleUsers, titleIDs, item.ID);

                if (String.IsNullOrEmpty(hfSelectedDept))
                    item.DeptID = null;
                else
                    item.DeptID = Convert.ToInt32(hfSelectedDept);

                await DB.SaveChangesAsync();

                // 关闭本窗体（触发窗体的关闭事件）
                ActiveWindow.HidePostBack();
            }

            return UIHelper.Result();
        }
    }
}