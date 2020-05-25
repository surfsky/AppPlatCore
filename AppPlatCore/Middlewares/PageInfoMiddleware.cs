using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using App.Utils;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using App.Web;
using Microsoft.AspNetCore.Builder;

namespace App.Middlewares
{
    public static class PageInfoMidlewareExtension 
    {
        public static IApplicationBuilder UsePageInfo(this IApplicationBuilder app)
        {
            return app.UseMiddleware<PageInfoMiddleware>();
        }
    }

    /// <summary>
    /// 显示页面信息的中间件. app.UsePageInfo();
    /// </summary>
    public class PageInfoMiddleware
    {
        private readonly RequestDelegate _next;
        public PageInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var url = context.Request.Path.Value.ToLower();
            if (url.EndsWith("$"))
                WritePageInfo(url);
            else
                await _next.Invoke(context);
        }

        /// <summary>输出页面信息</summary>
        public static void WritePageInfo(string url)
        {
            Asp.Write(url);
            //Asp.Write(WebHelper.BuildBootstrapCss());
            //Asp.Write(BuildPageInfo(url));
        }

        /*
        /// <summary>构建页面信息</summary>
        public static string BuildPageInfo(string url)
        {
            var type = Asp.GetHandler(url);
            var auth = type?.GetAttribute<AuthAttribute>() ?? new AuthAttribute(false);
            var ps = type?.GetAttributes<ParamAttribute>();
            auth.CheckSafe();

            // 自动补足参数
            if (type != null)
            {
                // 如果是FormPage，自动补足参数
                if (type.BaseType.Name.Contains("FormPage"))
                {
                    if (ps.FirstOrDefault(t => t.Name == "id") == null)
                        ps.Add(new ParamAttribute("id", "ID", false));
                    if (ps.FirstOrDefault(t => t.Name == "md") == null)
                        ps.Add(new ParamAttribute("md", "模式", typeof(PageMode), false));
                }
                // 如果是GridPage，自动补足参数
                else 
                {
                    var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                    if (fields.FirstOrDefault(t => t.FieldType.IsAssignableFrom(typeof(GridPro))) != null)
                    {
                        ps.Add(new ParamAttribute("md", "模式", typeof(PageMode)));
                        ps.Add(new ParamAttribute("search", "是否显示搜索工具栏", typeof(bool)));
                        ps.Add(new ParamAttribute("multi", "是否允许多选", typeof(bool)));
                        ps.Add(new ParamAttribute("readonly", "是否只读", typeof(bool)));
                    }
                }
            }

            var sb = new StringBuilder();
            var ui = type?.GetUIAttribute();
            sb.AppendFormat("<h1>{0}</h1><br/>", type?.GetTitle());
            sb.AppendFormat("<h5>URL:&nbsp;&nbsp;{0}</h5>", url);
            sb.AppendFormat("<h5>CLS:&nbsp;&nbsp;{0}</h5>", type?.FullName);
            sb.AppendFormat("<h6>{0}</h6><br/>", ui?.Remark);
            sb.AppendFormat("<h1>Auth</h1>");
            sb.AppendFormat("<br/><table class='table table-sm table-hover'>");
            sb.AppendFormat("<thead><tr>");
            sb.AppendFormat("<td>{0}</td>", nameof(auth.AuthLogin));
            sb.AppendFormat("<td>{0}</td>", nameof(auth.AuthSign));
            sb.AppendFormat("<td>{0}</td>", nameof(auth.ViewPower));
            sb.AppendFormat("<td>{0}</td>", nameof(auth.NewPower));
            sb.AppendFormat("<td>{0}</td>", nameof(auth.EditPower));
            sb.AppendFormat("<td>{0}</td>", nameof(auth.DeletePower));
            sb.AppendFormat("<td>{0}</td>", nameof(auth.Ignore));
            sb.AppendFormat("</tr></thead>");
            sb.AppendFormat("<tr>");
            sb.AppendFormat("<td>{0}</td>", auth?.AuthLogin);
            sb.AppendFormat("<td>{0}</td>", auth?.AuthSign);
            sb.AppendFormat("<td>{0}</td>", auth?.ViewPower);
            sb.AppendFormat("<td>{0}</td>", auth?.NewPower);
            sb.AppendFormat("<td>{0}</td>", auth?.EditPower);
            sb.AppendFormat("<td>{0}</td>", auth?.DeletePower);
            sb.AppendFormat("<td>{0}</td>", auth?.Ignore);
            sb.AppendFormat("</tr></table>");

            // 页面参数
            if (ps != null)
            {
                sb.AppendFormat("<h1>Parameters</h1>");
                sb.AppendFormat("<br/><table class='table table-sm table-hover'>");
                sb.AppendFormat("<thead><tr>");
                sb.AppendFormat("<td>Name</td>");
                sb.AppendFormat("<td>Title</td>");
                sb.AppendFormat("<td>Type</td>");
                sb.AppendFormat("<td>Required</td>");
                sb.AppendFormat("<td>Values</td>");
                sb.AppendFormat("</tr></thead>");
                foreach (var p in ps)
                {
                    sb.AppendFormat("<tr>");
                    sb.AppendFormat("<td>{0}&nbsp;</td>", p.Name);
                    sb.AppendFormat("<td>{0}&nbsp;</td>", p.Title);
                    sb.AppendFormat("<td>{0}&nbsp;</td>", p.TypeName);
                    sb.AppendFormat("<td>{0}&nbsp;</td>", p.Required);
                    sb.AppendFormat("<td>{0}&nbsp;</td>", p.ValueInfo);
                    sb.AppendFormat("</tr>");
                }
                sb.AppendFormat("</tr></table>");
            }
            return sb.ToString();
        }
    */
    }
}
