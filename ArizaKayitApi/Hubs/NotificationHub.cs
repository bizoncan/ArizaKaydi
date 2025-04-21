using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography.X509Certificates;

namespace ArizaKayitApi.Hubs
{
    public class NotificationHub:Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReciveMessage",message);
        }
    }
}
