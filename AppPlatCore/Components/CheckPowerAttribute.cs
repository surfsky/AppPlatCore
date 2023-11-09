using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FineUICore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace App.Components
{
    /// <summary>
    /// App自定义权限验证过滤器
    /// </summary>
    public class CheckPowerAttribute : ResultFilterAttribute
    {
        /// <summary>权限名称</summary>
        public string Name { get; set; }

        public CheckPowerAttribute(string name = "")
        {
            Name = name;
        }


        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            HttpContext context = filterContext.HttpContext;
            if (!string.IsNullOrEmpty(Name) && !Auth.CheckPower(context, Name))
            {
                if (context.Request.Method == "GET")
                {
                    Auth.CheckPowerFailWithPage(context);

                    //http://stackoverflow.com/questions/9837180/how-to-skip-action-execution-from-an-actionfilter
                    // -修正越权访问页面时会报错[服务器无法在发送 HTTP 标头之后追加标头]（龙涛软件-9374）。
                    filterContext.Result = new EmptyResult();
                }
                else if (context.Request.Method == "POST")
                {
                    Auth.CheckPowerFailWithAlert();
                    filterContext.Result = UIHelper.Result();
                }
            }

        }




    }
}