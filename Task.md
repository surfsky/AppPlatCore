
# FineUI 配置代码

```
public static class FineUIServiceExtensions
{
    public static IServiceCollection AddFineUI(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        ConfigSection section = config.GetSection("FineUI").Get<ConfigSection>();
        GlobalConfig.Configure(section);
        return services.AddSingleton((IFineUIService)new FineUIService());
    }
}

public static class FineUIMiddlewareExtensions
{
    public static IApplicationBuilder UseFineUI(this IApplicationBuilder builder)
    {
        PageContext.Configure(builder.ApplicationServices);
        builder.MapWhen((Func<HttpContext, bool>)((HttpContext P_0) => P_0.Request.Path.ToString().EndsWith("res.axd")), (Action<IApplicationBuilder>)delegate(IApplicationBuilder P_0)
        {
            P_0.UseFineUIHandler();
        });
        return builder.UseMiddleware<FineUIMiddleware>(Array.Empty<object>());
    }
}

service.AddFineUI(Configuration);
app.UseFileUI();
```



        /// <summary>
        /// 注册 HttpContextAccessor 单例服务。
        /// 可用 var accessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>() 获取上下文对象
        /// </summary>
        //public static void AddHttpContextAccessor(this IServiceCollection services)
        //{
        //    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        //}


        //---------------------------------------------
        private static EntityConfig _cfg;
        public static EntityConfig Instance
        {
            get
            {
                if (_cfg == null)
                    _cfg = new EntityConfig();
                return _cfg;
            }
        }


未识别 TargetFramework 值“”。可能是因为拼写错误。
如果拼写正确，必须显式指定 TargetFrameworkIdentifier 和/或 TargetFrameworkVersion 属性。
