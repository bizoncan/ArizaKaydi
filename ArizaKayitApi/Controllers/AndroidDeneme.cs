using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ArizaKayitApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AndroidDeneme : ControllerBase
    {

        private readonly String connectionString = "server=DESKTOP-8OU0GKQ\\SQLEXPRESS;database=ArizaKayit;integrated security= true;TrustServerCertificate=True";
        [HttpGet]
        public IActionResult GetMachines()
        {
            //var c = new context();
            //return Ok(c.Machines.ToList());
            List<machine> machines = new List<machine>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using SqlCommand cmd = new SqlCommand("SELECT * FROM Machines", conn);
                using SqlDataReader reader = cmd.ExecuteReader();
                {
                    while (reader.Read())
                    {
                        machines.Add(new machine
                        {
                            id = reader.GetInt32(0),
                            name = reader.GetString(1),
                            desc = reader.GetString(2),
                            number = reader.GetInt32(3),
                            imgURL = reader.GetString(4)
                        });

                    }


                }
            }

            return Ok(machines);

        }
    }
}
