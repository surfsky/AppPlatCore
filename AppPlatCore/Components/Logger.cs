using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;

namespace App.Components
{
    public class Logs
    {
        static Serilog.Core.Logger _log = new Lazy<Serilog.Core.Logger>(() => CreateLogger()).Value;

        static Serilog.Core.Logger CreateLogger()
        {
            return new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Logs\\log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public static void Info(string format, params object[] ps) => _log.Information(format, ps);
        public static void Error(string format, params object[] ps) => _log.Error(format, ps);
    }
}
