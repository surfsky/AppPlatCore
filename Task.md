
# FineUI ���ô���

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
        /// ע�� HttpContextAccessor ��������
        /// ���� var accessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>() ��ȡ�����Ķ���
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


δʶ�� TargetFramework ֵ��������������Ϊƴд����
���ƴд��ȷ��������ʽָ�� TargetFrameworkIdentifier ��/�� TargetFrameworkVersion ���ԡ�
