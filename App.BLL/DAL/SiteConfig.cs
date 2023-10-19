using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using App.Entities;
using App.Utils;

namespace App.Models
{
    /// <summary>
    /// 站点配置
    /// </summary>
    public class SiteConfig : EntityBase<SiteConfig>
    {
        [UI("网站图标")]           public string Icon { get; set; }
        [UI("网站标题")]           public string Title { get; set; }
        [UI("登陆页背景图片")]     public string LoginBg { get; set; }
        [UI("列表每页显示的个数")] public int PageSize { get; set; }
        [UI("帮助下拉列表")]       public string HelpList { get; set; }
        [UI("菜单样式")]           public string MenuType { get; set; }
        [UI("网站主题")]           public string Theme { get; set; }
        [UI("备案号")]             public string BeiAnNo { get; set; }
    }
}