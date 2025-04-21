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
            //List<machineNotifications> notifications = new List<machineNotifications>();
            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //  conn.Open();
            //using SqlCommand cmd = new SqlCommand("SELECT * FROM MachineNotifications", conn);
            //using SqlDataReader reader = cmd.ExecuteReader();
            //{
            //  while (reader.Read())
            //{
            //  notifications.Add(new machineNotifications
            //{
            //  id = reader.GetInt32(0),
            //machineId = reader.GetInt32(1),
            //title = reader.GetString(2),
            //description = reader.GetString(3),

            //});

            // }


            //}
            //}


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

            // string query = "INSERT INTO MachineNotifications (machineId, title,description) VALUES (@machineId, @title,@description)";

            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //  conn.Open();
            //SqlCommand cmd = new SqlCommand(query, conn);
            //cmd.Parameters.AddWithValue("@machineId", p.machineId);
            //cmd.Parameters.AddWithValue("@title", p.title);
            //cmd.Parameters.AddWithValue("@description", p.description);
            //cmd.ExecuteNonQuery();
            //}
            //return Ok(new { message = "Kullanıcı eklendi!" });

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
