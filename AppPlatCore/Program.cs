using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using App.DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseUrls("http://*:8080", "https://*:8088");  // ��Ч������
                    //webBuilder.UseKestrel(opts =>
                    //{
                    //    // Bind directly to a socket handle or Unix socket
                    //    // opts.ListenHandle(123554);
                    //    // opts.ListenUnixSocket("/tmp/kestrel-test.sock");
                    //    opts.Listen(IPAddress.Loopback, port: 5002);
                    //    opts.ListenAnyIP(5003);
                    //    opts.ListenLocalhost(5004, opts => opts.UseHttps());  // ����
                    //});
                });
            var host = builder.Build();


            // ��ȡ����
            //var config = host.Services.GetRequiredService<IConfiguration>();
            //var urls = config["urls"];

            // Server ����, ��ȡ IServerAddressesFeature, ��� URL
            //var server = host.Services.GetRequiredService<IServer>();
            //var addrfeature = server.Features.Get<IServerAddressesFeature>();
            //addrfeature.Addresses.Add(urls);// ����

            //
            CreateDbIfNotExists(host);
            host.Run();
            //host.Run("http://localhost:8000"); // �޴˷���
        }


        // https://docs.microsoft.com/zh-cn/aspnet/core/data/ef-rp/intro
        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppPlatContext>();
                    AppPlatContextInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "�������ݿ�ʱ��������");
                }
            }
        }
    }
}
