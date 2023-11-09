using App.Utils;

namespace App.API
{
    /// <summary>
    /// App 类型
    /// </summary>
    public enum AppType : int
    {
        [UI("Web")]           Web = 0,
        [UI("MobileWeb")]     MobileWeb = 1,

        [UI("iOS")]           iOS = 10,
        [UI("Android")]       Android = 11,
        [UI("Windows")]       Windows = 12,
        [UI("Mac")]           Mac = 13,
        [UI("Linux")]         Linux = 14,

        [UI("微信小程序")]    WechatMP = 20,
        [UI("支付宝小程序")]  AlipayMP = 21,
        [UI("钉钉小程序")]    DingTalkMP = 22
    }

    /// <summary>
    /// 短信消息类别
    /// </summary>
    public enum SmsType : int
    {
        [UI("注册")]          Regist = 0,
        [UI("登陆验证")]      Verify = 1,
        [UI("修改密码")]      ChangePassword = 2,
        [UI("更改用户信息")]  ChangeInfo = 3
    }
}
