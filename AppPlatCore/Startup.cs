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
            services.AddHttpContextAccessor();                  // ע�� HttpContext ����
            services.AddDistributedMemoryCache();               // ע���ڴ滺�����session�õõ���
            services.AddSession();                              // ע�� Session ����
            services.AddControllersWithViews().AddRazorRuntimeCompilation();  // �޸� cshtml ���Զ���Ч
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = new PathString("/Login");   // ��¼ҳ��
                options.Cookie.HttpOnly = true;
            });
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 1024;                 // ��������ĸ������ƣ�Ĭ��ֵ��1024��
                options.ValueLengthLimit = 4194304;             // �����������ֵ�ĳ������ƣ�Ĭ��ֵ��4194304 = 1024 * 1024 * 4��
            });
            services.AddFineUI(Configuration);                  // ע�� FineUI ����
            services.AddRazorPages()
                .AddMvcOptions(options =>{
                    options.ModelBinderProviders.Insert(0, new JsonModelBinderProvider());  // �Զ���ģ�Ͱ󶨣�Newtonsoft.Json��
                })
                .AddNewtonsoftJson()                            // �� NewtonsoftJson �����л�json�������Դ��ģ�
                ;
            services.AddServerSideBlazor();                     // ���� Blazor
            services.AddSignalR();                              // ʹ�� SignalR


            // ע�����ݿ����ӷ���ÿ������ʱ������
            var sqlserver = Configuration.GetConnectionString("SQLServer");
            var mysql = Configuration.GetConnectionString("MySQL");
            var dm = Configuration.GetConnectionString("DM");
            var sqlite = Configuration.GetConnectionString("Sqlite");
            //services.AddDbContext<AppPlatContext>(options => 
            //{
            //    options.UseSqlServer(sqlserver, builder =>
            //    {
            //        builder.EnableRetryOnFailure();//���Զ���ʧ����������
            //        builder.CommandTimeout(60);
            //        builder.UseRowNumberForPaging(); //Use a ROW_NUMBER() in queries instead of OFFSET/FETCH. This method is backwards-compatible to SQL Server 2005. �������'OFFSET' �������﷨���� �� FETCH �����ѡ�� NEXT ���÷���Ч 'OFFSET' �������﷨���� �� FETCH �����ѡ�� NEXT ���÷���Ч
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
            // ����ʱ֧��
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Error");
                //app.UseHsts();
            }
            //app.UseHttpsRedirection();                // �Զ��� http ת��Ϊ https

            // �Զ����м��
            app.UserAppWeb(env.ContentRootPath);       // ���� App.Web 
            app.UseMonitor(o => Console.WriteLine("{0} {1} {2}", o.Url, o.Seconds, o.ClientIP));  // ���ü��ģ��
            app.UseImage();                            // ��������ͼ��ˮӡ
            app.UseHttpApi(o =>                        // ���� HttpApi
            {
                o.TypePrefix = "App.API.";
                o.FormatEnum = EnumFomatting.Int;
            });

            // ��׼�м��
            app.UseStaticFiles();                       // ���þ�̬�ļ����
            app.UseSession();                           // ����Session
            app.UseRouting();                           // ����·��
            app.UseAuthentication();                    // ���ü�Ȩ���Ƿ��¼��
            app.UseAuthorization();                     // ������Ȩ����ʲôȨ�����ԣ�

            // �����м��
            app.UseFineUI();                            // ���� FineUI �ؼ���
            app.UseWebSockets();                        // ���� WebSocket ��֧��SignalR
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();              // ���� RazorPage ��������
                endpoints.MapHub<ChatHub>("/ChatHub");  // ���� ChatHub��·��Ϊ /Chat
                endpoints.MapBlazorHub();                 // ���� Blazor
                //endpoints.MapFallbackToPage("/Blazor");  // 
            });
        }
    }
}
