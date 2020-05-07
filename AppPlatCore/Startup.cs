using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.HttpApi;
using App.Web;
using AppBoxCore.Models;
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

namespace AppBoxCore
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
            services.AddHttpContextAccessor();                  // 注册 HttpContext 服务
            services.AddDistributedMemoryCache();               // 注册内存缓存服务（session用得到）
            services.AddSession();                              // 注册 Session 服务
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


            // 注册数据库连接服务（每次请求时创建）
            var sqlserver = Configuration.GetConnectionString("SQLServer");
            var sqlserver2 = Configuration.GetConnectionString("SQLServer2");
            var mysql = Configuration.GetConnectionString("MySQL");
            var dm = Configuration.GetConnectionString("DM");
            var sqlite = Configuration.GetConnectionString("Sqlite");
            services.AddDbContext<AppPlatContext>(options => options.UseSqlServer(sqlserver2));  // SqlServer linux. EFCore 2.2 ok
            //services.AddDbContext<AppPlatContext>(options => options.UseSqlite(sqlite));       // EFCore 2.2 ok
            //services.AddDbContext<AppPlatContext>(options => options.UseMySql(mysql));         // MySql(MariDB) linux. EFCore 2.2 ok
            //services.AddDbContext<AppPlatContext>(options => options.UseSqlServer(sqlserver, options=> options.UseRowNumberForPaging()));  // SqlServer 2008. EFCore 2.2 ok, EFCore 3.1 fail. see https://aka.ms/AA6h122
            //services.AddDbContext<AppPlatContext>(options => options.UseDm(dm));               // fail
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Error");
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();   // 自动将 http 转化为 https
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // 中间件
            app.UseHttpApi(options =>
            {
                options.TypePrefix = "App.API.";
                options.FormatEnum = EnumFomatting.Int;
            });
            app.UserAppWeb(env.ContentRootPath);
            app.UseFineUI();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
