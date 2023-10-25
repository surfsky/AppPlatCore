using App.Components;
using App.Models;
using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace App.Pages.Test
{
    public class DynamicGridModel : BaseModel
    {
        public List<User> Users;
        public void OnGet()
        {
            Users = DataSources.GetUsers();
            InitGridColumns();
        }


        private void InitGridColumns()
        {
            List<GridColumn> columns = new List<GridColumn>();
            RenderField field = null;

            //
            columns.Add(new RowNumberField());

            //
            field = new RenderField();
            field.HeaderText = "����";
            field.DataField = "Name";
            columns.Add(field);

            //
            field = new RenderField();
            field.HeaderText = "�Ա�";
            field.DataField = "Gender";
            field.FieldType = FieldType.Int;
            field.RendererFunction = "renderGender";
            field.Width = 80;
            columns.Add(field);

            //
            field = new RenderField();
            field.HeaderText = "��ѧ���";
            field.DataField = "EntranceYear";
            field.FieldType = FieldType.Int;
            field.Width = 100;
            columns.Add(field);

            //
            RenderCheckField checkfield = new RenderCheckField();
            checkfield.HeaderText = "�Ƿ���У";
            checkfield.DataField = "AtSchool";
            checkfield.RenderAsStaticField = true;
            checkfield.Width = 100;
            columns.Add(checkfield);

            //
            checkfield = new RenderCheckField();
            checkfield.HeaderText = "�Ƿ���У";
            checkfield.DataField = "AtSchool";
            checkfield.RenderAsStaticField = false;
            checkfield.Enabled = false;
            columns.Add(checkfield);

            //
            field = new RenderField();
            field.HeaderText = "��ѧרҵ";
            field.DataField = "Major";
            field.RendererFunction = "renderMajor";
            field.ExpandUnusedSpace = true;
            columns.Add(field);

            //
            field = new RenderField();
            field.HeaderText = "����";
            field.DataField = "Group";
            field.RendererFunction = "renderGroup";
            field.Width = 80;
            columns.Add(field);

            //
            field = new RenderField();
            field.HeaderText = "ע������";
            field.DataField = "LogTime";
            field.FieldType = FieldType.Date;
            field.Renderer = Renderer.Date;
            field.RendererArgument = "yyyy-MM-dd";
            field.Width = 100;
            columns.Add(field);

            // �� ViewBag ���ݸ���ͼ
            ViewBag.Grid1Columns = columns.ToArray();
        }
    }
}
