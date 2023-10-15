using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FineUICore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using App.HttpApi;
using App.Middlewares;
using App.Web;
using App.Models;
using App.Hubs;
using App.Components;

namespace App
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            Logs.Info("server start");
            services.AddHttpContextAccessor();                  // 注册 HttpContext 服务
            services.AddDistributedMemoryCache();               // 注册内存缓存服务（session用得到）
            services.AddSession();                              // 注册 Session 服务
            services.AddControllersWithViews().AddRazorRuntimeCompilation();  // 修改 cshtml 后自动生效
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = new PathString("/Login");   // 登录页面
                options.Cookie.HttpOnly = true;
            });
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 1024;                 // 请求参数的个数限制（默认值：1024）
                options.ValueLengthLimit = 4194304;             // 单个请求参数值的长度限制（默认值：4194304 = 1024 * 1024 * 4）
            });
            services.AddFineUI(Configuration);                  // 注册 FineUI 服务
            services.AddRazorPages()
                .AddMvcOptions(options =>{
                    options.ModelBinderProviders.Insert(0, new JsonModelBinderProvider());  // 自定义模型绑定（Newtonsoft.Json）
                })
                .AddNewtonsoftJson()                            // 用 NewtonsoftJson 来序列化json（而非自带的）
                ;
            services.AddServerSideBlazor();                     // 启用 Blazor
            services.AddSignalR();                              // 使用 SignalR


            // 注册数据库连接服务（每次请求时创建）
            var sqlserver = Configuration.GetConnectionString("SQLServer");
            var mysql = Configuration.GetConnectionString("MySQL");
            var dm = Configuration.GetConnectionString("DM");
            var sqlite = Configuration.GetConnectionString("Sqlite");
            //services.AddDbContext<AppPlatContext>(options => 
            //{
            //    options.UseSqlServer(sqlserver, builder =>
            //    {
            //        builder.EnableRetryOnFailure();//可自定义失败重连次数
            //        builder.CommandTimeout(60);
            //        builder.UseRowNumberForPaging(); //Use a ROW_NUMBER() in queries instead of OFFSET/FETCH. This method is backwards-compatible to SQL Server 2005. 避免错误：'OFFSET' 附近有语法错误。 在 FETCH 语句中选项 NEXT 的用法无效 'OFFSET' 附近有语法错误。 在 FETCH 语句中选项 NEXT 的用法无效
            //        //builder.MigrationsAssembly("App.BLL.dll");
            //    });
            //});  // SqlServer linux. EFCore 2.2 ok
            services.AddDbContext<AppPlatContext>(options => options.UseSqlite(sqlite));       // EFCore 2.2 ok
            //services.AddDbContext<AppPlatContext>(options => options.UseMySql(mysql));         // MySql(MariDB) linux. EFCore 2.2 ok
            //services.AddDbContext<AppPlatContext>(options => options.UseSqlServer(sqlserver, options=> options.UseRowNumberForPaging()));  // SqlServer 2008. EFCore 2.2 ok, EFCore 3.1 fail. see https://aka.ms/AA6h122
            //services.AddDbContext<AppPlatContext>(options => options.UseDm(dm));               // fail
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 开发时支持
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Error");
                //app.UseHsts();
            }
            //app.UseHttpsRedirection();                // 自动将 http 转化为 https

            // 自定义中间件
            app.UserAppWeb(env.ContentRootPath);       // 配置 App.Web 
            app.UseMonitor(o => Console.WriteLine("{0} {1} {2}", o.Url, o.Seconds, o.ClientIP));  // 启用监控模块
            app.UseImage();                            // 启用缩略图及水印
            app.UseHttpApi(o =>                        // 启用 HttpApi
            {
                o.TypePrefix = "App.API.";
                o.FormatEnum = EnumFomatting.Int;
            });

            // 标准中间件
            app.UseStaticFiles();                       // 启用静态文件输出
            app.UseSession();                           // 启用Session
            app.UseRouting();                           // 启用路由
            app.UseAuthentication();                    // 启用鉴权（是否登录）
            app.UseAuthorization();                     // 启用授权（有什么权限属性）

            // 其它中间件
            app.UseFineUI();                            // 启用 FineUI 控件库
            app.UseWebSockets();                        // 启用 WebSocket 以支持SignalR
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();              // 启用 RazorPage 解析引擎
                endpoints.MapHub<ChatHub>("/ChatHub");  // 启用 ChatHub，路径为 /Chat
                endpoints.MapBlazorHub();                 // 启用 Blazor
                //endpoints.MapFallbackToPage("/Blazor");  // 
            });
        }
    }
}
