using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.Components;

namespace FineUICore.Examples.RazorPages.Pages.GridPaging
{
    public class GridPagingModel : PageModel
    {
        /// <summary>记录数</summary>
        public int DataCount { get; set; }
        public DataTable DataSource { get; set; }
        
        public void OnGet()
        {
            // 1.设置总项数（特别注意：数据库分页初始化时，一定要设置总记录数RecordCount）
            // 2.获取当前分页数据
            //ViewBag.Grid1RecordCount = recordCount;
            //ViewBag.Grid1DataSource = DataSourceUtil.GetPagedDataTable(pageIndex: 0, pageSize: 5, recordCount: recordCount);
            DataCount = DemoData.GetTotalCount(); ;
            DataSource = DemoData.GetPagedDataTable(pageIndex: 0, pageSize: 5, DataCount);
        }

        public IActionResult OnPostGrid1_PageIndexChanged(string[] Grid1_fields, int Grid1_pageIndex)
        {
            var grid1 = UIHelper.Grid("Grid1");
            var recordCount = DemoData.GetTotalCount();

            // 1.设置总项数（数据库分页回发时，如果总记录数不变，可以不设置RecordCount）
            grid1.RecordCount(recordCount);

            // 2.获取当前分页数据
            var dataSource = DemoData.GetPagedDataTable(pageIndex: Grid1_pageIndex, pageSize: 5, recordCount: recordCount);
            grid1.DataSource(dataSource, Grid1_fields);
            return UIHelper.Result();
        }

    }
}