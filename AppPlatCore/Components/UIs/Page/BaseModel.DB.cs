using App.Components;
using App.Entities;
using App.DAL;
using Jint.Parser.Ast;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace App
{
    /// <summary>
    /// 数据库相关（后面的Linq 方法可用 App.Entities.EFHelper 替代，有空做）
    /// </summary>
    public partial class BaseModel
    {
        private DAL.AppPlatContext _db;

        /// <summary>每个请求共享一个数据库连接实例</summary>
        protected DAL.AppPlatContext DB
        {
            get
            {
                if (_db == null)
                    _db = Common.GetDbConnection();
                return _db;
            }
        }


        protected IQueryable<T> Sort<T>(IQueryable<T> q, PagingInfo pagingInfo)
        {
            return q.SortBy(pagingInfo.SortField + " " + pagingInfo.SortDirection);
        }

        // 排序
        protected IQueryable<T> Sort<T>(IQueryable<T> q, string sortField, string sortDirection)
        {
            return q.SortBy(sortField + " " + sortDirection);
        }


        protected IQueryable<T> SortAndPage<T>(IQueryable<T> q, PagingInfo pagingInfo)
        {
            return SortAndPage(q, pagingInfo.PageIndex, pagingInfo.PageSize, pagingInfo.RecordCount, pagingInfo.SortField, pagingInfo.SortDirection);
        }

        // 排序后分页
        protected IQueryable<T> SortAndPage<T>(IQueryable<T> q, int pageIndex, int pageSize, int recordCount, string sortField, string sortDirection)
        {
            //// 对传入的 pageIndex 进行有效性验证//////////////
            if (pageSize == 0) pageSize = 20;
            int pageCount = recordCount / pageSize;
            if (recordCount % pageSize != 0)
                pageCount++;
            if (pageIndex > pageCount - 1)
                pageIndex = pageCount - 1;
            if (pageIndex < 0)
                pageIndex = 0;

            return Sort(q, sortField, sortDirection).Skip(pageIndex * pageSize).Take(pageSize);
        }

    }
}
