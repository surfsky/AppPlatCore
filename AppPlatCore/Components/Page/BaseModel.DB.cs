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


        protected IQueryable<T> Sort<T>(IQueryable<T> q, PagingInfoViewModel pagingInfo)
        {
            return q.SortBy(pagingInfo.SortField + " " + pagingInfo.SortDirection);
        }

        // 排序
        protected IQueryable<T> Sort<T>(IQueryable<T> q, string sortField, string sortDirection)
        {
            return q.SortBy(sortField + " " + sortDirection);
        }


        protected IQueryable<T> SortAndPage<T>(IQueryable<T> q, PagingInfoViewModel pagingInfo)
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


        // 附加实体到数据库上下文中（首先在Local中查找实体是否存在，不存在才Attach，否则会报错）
        // http://patrickdesjardins.com/blog/entity-framework-4-3-an-object-with-the-same-key-already-exists-in-the-objectstatemanager
        protected T Attach<T>(int keyID) where T : class, IKeyID, new()
        {
            T t = DB.Set<T>().Local.Where(x => x.ID == keyID).FirstOrDefault();
            if (t == null)
            {
                t = new T { ID = keyID };
                DB.Set<T>().Attach(t);
            }
            return t;
        }

        //// 向现有实体集合中添加新项
        //protected void AddEntities<T>(ICollection<T> existItems, int[] newItemIDs) where T : class, IKeyID, new()
        //{
        //    foreach (int roleID in newItemIDs)
        //    {
        //        T t = Attach<T>(roleID);
        //        existItems.Add(t);
        //    }
        //}

        //// 替换现有实体集合中的所有项
        //// http://stackoverflow.com/questions/2789113/entity-framework-update-entity-along-with-child-entities-add-update-as-necessar
        //protected void ReplaceEntities<T>(ICollection<T> existEntities, int[] newEntityIDs) where T : class, IKeyID, new()
        //{
        //    if (newEntityIDs.Length == 0)
        //    {
        //        existEntities.Clear();
        //    }
        //    else
        //    {
        //        int[] tobeAdded = newEntityIDs.Except(existEntities.Select(x => x.ID)).ToArray();
        //        int[] tobeRemoved = existEntities.Select(x => x.ID).Except(newEntityIDs).ToArray();

        //        AddEntities<T>(existEntities, tobeAdded);

        //        existEntities.Where(x => tobeRemoved.Contains(x.ID)).ToList().ForEach(e => existEntities.Remove(e));
        //    }
        //}

        //// http://patrickdesjardins.com/blog/validation-failed-for-one-or-more-entities-see-entityvalidationerrors-property-for-more-details-2
        //// ((System.Data.Entity.Validation.DbEntityValidationException)$exception).EntityValidationErrors





        protected T Attach2<T>(int keyID1, int keyID2) where T : class, IKey2ID, new()
        {
            T t = DB.Set<T>().Local.Where(x => x.ID1 == keyID1 && x.ID2 == keyID2).FirstOrDefault();
            if (t == null)
            {
                t = new T { ID1 = keyID1, ID2 = keyID2 };
                DB.Set<T>().Attach(t);
            }
            return t;
        }

        protected void AddEntities2<T>(int keyID1, int[] keyID2s) where T : class, IKey2ID, new()
        {
            foreach (int id in keyID2s)
            {
                T t = Attach2<T>(keyID1, id);
                DB.Entry(t).State = EntityState.Added;

                //T t = new T { ID1 = keyID1, ID2 = id };
                //existEntities.Add(t);
            }
        }

        protected void AddEntities2<T>(int[] keyID1s, int keyID2) where T : class, IKey2ID, new()
        {
            foreach (int id in keyID1s)
            {
                T t = Attach2<T>(id, keyID2);
                DB.Entry(t).State = EntityState.Added;

                //T t = new T { ID1 = id, ID2 = keyID2 };
                //existEntities.Add(t);
            }
        }

        protected void RemoveEntities2<T>(List<T> existEntities, int[] keyID1s, int[] keyID2s) where T : class, IKey2ID, new()
        {
            List<T> itemsTobeRemoved;
            if (keyID1s == null)
            {
                itemsTobeRemoved = existEntities.Where(x => keyID2s.Contains(x.ID2)).ToList();
            }
            else
            {
                itemsTobeRemoved = existEntities.Where(x => keyID1s.Contains(x.ID1)).ToList();
            }
            itemsTobeRemoved.ForEach(e => existEntities.Remove(e));
        }

        protected void ReplaceEntities2<T>(List<T> existEntities, int keyID1, int[] keyID2s) where T : class, IKey2ID, new()
        {
            if (keyID2s.Length == 0)
            {
                existEntities.Clear();
            }
            else
            {
                int[] tobeAdded = keyID2s.Except(existEntities.Select(x => x.ID2)).ToArray();
                int[] tobeRemoved = existEntities.Select(x => x.ID2).Except(keyID2s).ToArray();

                AddEntities2<T>(keyID1, tobeAdded);
                RemoveEntities2<T>(existEntities, null, tobeRemoved);
            }
        }

        protected void ReplaceEntities2<T>(List<T> existEntities, int[] keyID1s, int keyID2) where T : class, IKey2ID, new()
        {
            if (keyID1s.Length == 0)
            {
                existEntities.Clear();
            }
            else
            {
                int[] tobeAdded = keyID1s.Except(existEntities.Select(x => x.ID1)).ToArray();
                int[] tobeRemoved = existEntities.Select(x => x.ID1).Except(keyID1s).ToArray();

                AddEntities2<T>(tobeAdded, keyID2);
                RemoveEntities2<T>(existEntities, tobeRemoved, null);
            }
        }

    }
}
