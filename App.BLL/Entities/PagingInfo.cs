using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.DAL;

namespace App.Components
{
    /// <summary>分页及排序信息</summary>
    public class PagingInfo
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public string SortField { get; set; }
        public string SortDirection { get; set; }


        public PagingInfo() { }
        public PagingInfo(string sortField, bool sortDirection)
        {
            SortField = sortField;
            SortDirection = sortDirection ? "ASC" : "DESC";
            PageIndex = 0;
            PageSize = SiteConfig.Instance.PageSize;
        }
        public PagingInfo(string sortField, string sortDirection)
        {
            SortField = sortField;
            SortDirection = sortDirection;
            PageIndex = 0;
            PageSize = SiteConfig.Instance.PageSize;
        }
    }
}