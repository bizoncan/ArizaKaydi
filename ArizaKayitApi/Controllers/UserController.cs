using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArizaKayitApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly context _context;

        public UserController(context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult checkUser(String username, String email)
        {
            //var c = new context();
            Boolean userExists = _context.Users.Any(c => c.Email == email || c.UserName == username);
            if (userExists)
            {
                return Ok(true);
            }
            return Ok(false);
        }
        [HttpPost]
        public IActionResult addUser([FromBody] user u)
        {
            //var c = new context();
            var passwordHasher = new PasswordHasher<user>();
            u.PasswordHash = passwordHasher.HashPassword(u, u.PasswordHash);

            _context.mobileUsers.Add(u);
            _context.SaveChanges();
            return Ok(u);
        }
        [HttpGet("GetUsers")]
        public IActionResult getUsers(String username, String password)
        {
            //var c = new context();
            var passwordHasher = new PasswordHasher<user>();
            foreach (user user in _context.mobileUsers)
            {
                if (username == user.UserName)
                {
                    var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                    if (result == PasswordVerificationResult.Success) { return Ok(new {success = true, message="Başarılı", userId = user.Id }); }
                    else { return Ok(new { success = false, message = "Hatalı şifre.", userId= 0 }); }
                }

            }
            return Ok(new { success = false, message = "Kullanıcı adı bulunamadı." });
        }
    

    }
}
