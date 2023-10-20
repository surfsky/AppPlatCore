using System.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Components
{
    /// <summary>
    /// 公共方法类
    /// </summary>
    public class Common
    {
        public static string GetProductVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            return String.Format("{0}.{1}.{2}", v.Major, v.Minor, v.Build);
        }

        /// <summary>获取数据库连接实例（静态方法）</summary>
        public static Models.AppPlatContext GetDbConnection()
        {
            return FineUICore.PageContext.GetRequestService<Models.AppPlatContext>();
        }



        /// <summary>
        /// 获取实例的属性名称列表
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string[] GetReflectionProperties(object instance)
        {
            var result = new List<string>();
            foreach (PropertyInfo property in instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var propertyName = property.Name;
                // NotMapped特性
                var notMappedAttr = property.GetCustomAttribute<NotMappedAttribute>(false);
                if (notMappedAttr == null && propertyName != "ID")
                {
                    result.Add(propertyName);
                }
            }
            return result.ToArray();
        }


        //protected void InvalidModelState(ModelStateDictionary state)
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    sb.Append("<ul>");
        //    foreach (var key in state.Keys)
        //    {
        //        //将错误描述添加到sb中
        //        foreach (var error in state[key].Errors)
        //        {
        //            sb.AppendFormat("<li>{0}</li>", error.ErrorMessage);
        //        }
        //    }
        //    sb.Append("</ul>");

        //    Alert.Show(sb.ToString());
        //}

    }
}
