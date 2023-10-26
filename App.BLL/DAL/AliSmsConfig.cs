using App.Entities;
using App.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DAL
{
    [UI("系统", "阿里配置")]
    public class AliSmsConfig : EntityBase<AliSmsConfig>
    {
        [UI("阿里短信", "签名")]            public string SmsSignName        { get; set; }
        [UI("阿里短信", "Key")]             public string SmsAccessKeyId     { get; set; }
        [UI("阿里短信", "Secret")]          public string SmsAccessKeySecret { get; set; }
        [UI("阿里短信", "编号-注册")]       public string SmsRegist          { get; set; }
        [UI("阿里短信", "编号-校验")]       public string SmsVerify          { get; set; }
        [UI("阿里短信", "编号-改密码")]     public string SmsChangePassword  { get; set; }
        [UI("阿里短信", "编号-改信息")]     public string SmsChangeInfo      { get; set; }
        [UI("阿里短信", "编号-通知")]       public string SmsNotify          { get; set; }
    }
}
