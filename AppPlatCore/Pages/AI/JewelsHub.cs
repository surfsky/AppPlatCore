using App.Components;
using App.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Pages.AI
{
    /// <summary>
    /// 首饰识别 AI  Hub
    /// </summary>
    public class JewelsHub : Hub
    {
        /// <summary>编译模型，供客户端调用</summary>
        public async Task BuildModel()
        {
            if (Auth.CheckPower(Asp.Current, "CoreConfigEdit"))
            {
                var startTime = DateTime.Now;
                //System.Threading.Thread.Sleep(1000*60*20);
                JewelsAI.BuildModel();
                var span = DateTime.Now - startTime;
                var msg = "重建模型成功，耗时 " + span.ToString("c");
                Logger.Info(msg);
                await Clients.All.SendAsync("AIMessage", new { Code = 0, Message = msg, CreateDt = DateTime.Now.ToString() });
            }
            else
            {
                await Clients.All.SendAsync("AIMessage", new { Code = 500, Message = "您无权操作", CreateDt = DateTime.Now.ToString() });
            }
        }
    }
}
