using ArizaKayitApi.Models;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;

namespace ArizaKayitApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageDataController : ControllerBase
    {
        context _context;

        public ImageDataController(context context)
        {
            _context = context;
        }
        [HttpPost("addImageData")]
        public IActionResult addImageData(List<String> img_data)
        {
            int veri = 1;
            SqlConnection con = new SqlConnection("server=10.10.82.69,1433;database=ArizaKayit;User Id=sa ;Password=A/f-mrf_12 ;TrustServerCertificate=True ;");
            con.Open();
            string query = "select top 1 * from dbo.Errors order by id desc";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                veri = reader.GetInt32(0);
            }
            
           
            if (img_data.Count()>0)
            {
                foreach( String i in img_data)
                {
                    imageCollection image = new imageCollection();
                    image.errorId = veri;
                    try
                    {
                        // Convert Base64 string to byte array
                        image.imageDataByte = Convert.FromBase64String(i);
                    }
                    catch (FormatException ex)
                    {
                        // Handle invalid Base64 string format
                        return BadRequest("Invalid image format.");
                    }
                    _context.ImageCollection.Add(image); 
                    _context.SaveChanges();
                }
            }
            return Ok();
        }
		[HttpPost("addImageDataWork")]
		public IActionResult addImageDataWork([FromBody] WorkImagePostModel workImagePostModel)
		{
			//int veri = 1;
			//SqlConnection con = new SqlConnection("server=10.10.82.69,1433;database=ArizaKayit;User Id=sa ;Password=A/f-mrf_12 ;TrustServerCertificate=True");
			//con.Open();
			//string query = "select top 1 * from dbo.Works order by id desc";
			//SqlCommand cmd = new SqlCommand(query, con);
			//SqlDataReader reader = cmd.ExecuteReader();
			//if (reader.Read())
			//{
			//	veri = reader.GetInt32(0);
			//}
            List<String> img_data = workImagePostModel.imageCollectionModel;
			int veri = workImagePostModel.workId;

			if (img_data.Count() > 0)
			{
				foreach (String i in img_data)
				{
					imageCollection image = new imageCollection();
					image.workId = veri;
					try
					{
						image.imageDataByte = Convert.FromBase64String(i);
					}
					catch (FormatException ex)
					{
						return BadRequest("Invalid image format.");
					}
					_context.ImageCollection.Add(image);
					_context.SaveChanges();
				}
			}
			return Ok();
		}


		[HttpGet("{errorId}")]
        public IActionResult getImages(int errorId)
        {
            List<imageCollection> imageCollection = _context.ImageCollection.Where(e=> e.errorId == errorId).ToList();
            List<String> values = new List<String>();
            foreach (imageCollection image in imageCollection) 
            {
                values.Add(Convert.ToBase64String(image.imageDataByte));
            }
            return Ok(values);
        }
		[HttpGet("api/images")]
		public async Task<IActionResult> GetImages([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var images = await _context.ImageCollection
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Select(x => new {
					x.id,
					imageBase64 = Convert.ToBase64String(x.imageDataByte)
				}).ToListAsync();

			return Ok(images);
		}
	}
}
