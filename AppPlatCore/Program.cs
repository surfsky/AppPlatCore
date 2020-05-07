using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppBoxCore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppBoxCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .Build()
                ;
            CreateDbIfNotExists(host);
            host.Run();
        }


        // https://docs.microsoft.com/zh-cn/aspnet/core/data/ef-rp/intro
        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<Models.AppPlatContext>();
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
