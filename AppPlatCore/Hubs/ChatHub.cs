using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Hubs
{
    /// <summary>
    /// 简单的 SignalR 聊天 Hub
    /// </summary>
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ChatMessage", user, message);
        }
    }
}
