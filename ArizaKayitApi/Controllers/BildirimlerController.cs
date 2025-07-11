using ArizaKayitApi.Hubs;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata.Ecma335;

namespace ArizaKayitApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class BildirimlerController : ControllerBase
    {
        private readonly context _context;

        private readonly IHubContext<NotificationHub> _hubcontext;

      

        public BildirimlerController(context context)
        {
            _context = context;
        }

        private readonly String connectionString = "server=DESKTOP-8OU0GKQ\\SQLEXPRESS;database=ArizaKayit;integrated security= true;TrustServerCertificate=True";
        [HttpGet]
        public IActionResult GetNotification()
        {
            var work_not = _context.WorkOrders.Where(e=> e.isClosed == false).ToList();
			
			var not = _context.MachineNotifications.ToList();

            var result = new
            {
                workNotificationList = work_not,
                notificationList = not,
            };
			return Ok(result);
    
        }
        [HttpPost]
        public IActionResult AddNotification([FromBody] machineNotifications p)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var c = new context();
            _context.MachineNotifications.Add(p);
            _context.SaveChanges();
            _hubcontext.Clients.All.SendAsync("RecieveMessage", "Yeni Bİldirim Eklendi").GetAwaiter().GetResult(); ;


            return Created("", p);


        }
        [HttpDelete("{id}")]
        public IActionResult DeleteNotification(int id)
        {
            //            var c = new context();
            _context.MachineNotifications.Remove(_context.Set<machineNotifications>().Find(id));
            _context.SaveChanges();
            return NoContent();
        }

    }
}
