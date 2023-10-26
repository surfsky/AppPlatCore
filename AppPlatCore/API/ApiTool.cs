using App.Entities;
using App.HttpApi;
using App.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace App.API
{
    /// <summary>
    /// API 接口辅助扩展方法
    /// </summary>
    public static class ApiTool
    {
        /// <summary>转化为标准 API 结构</summary>
        /// <param name="info">正常返回时的提示信息</param>
        /// <param name="nullInfo">对象为空时的提示信息</param>
        public static APIResult ToResult(this object obj, ExportMode type = ExportMode.Normal, string info = "操作成功", string nullInfo = "操作失败")
        {
            if (obj == null)
                return new APIResult(false, nullInfo, null);
            else
            {
                if (obj is IExport)
                    obj = (obj as IExport).Export(type);
                return new APIResult(true, info, obj);
            }
        }

        /// <summary>转化为标准 API 结构</summary>
        public static APIResult ToResult<T>(this IEnumerable<T> obj, ExportMode type = ExportMode.Normal, string info = "操作成功", string nullInfo = "操作失败")
        {
            if (obj is string)
                return new APIResult(true, info, obj);
            var o = obj.Cast(t =>
            {
                if (t is IExport)
                    return (t as IExport).Export(type);
                return t;

            });
            return new APIResult(true, info, o);
        }

        /// <summary>转化为标准 API 结构</summary>
        public static APIResult ToResult<T>(this IQueryable<T> obj, ExportMode type = ExportMode.Normal, string info = "操作成功", string nullInfo = "操作失败")
        {
            return obj.ToList().ToResult();
        }

        /// <summary>增加属性（将忽略空值）</summary>
        public static JObject AddJProperty(this object o, string name, object value)
        {
            return o.AsJObject(HttpApiConfig.Instance.JsonSetting).AddProperty(name, value);
        }
    }
}
