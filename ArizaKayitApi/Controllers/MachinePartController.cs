using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArizaKayitApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachinePartController : ControllerBase
    {
        private readonly context _context;

        public MachinePartController(context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetMachinePart(int id)
        {
           // var c = new context();
            List<machinePart> n = _context.MachineParts.Where(x=> x.machineId == id).ToList();
            return (Ok(n));
        }
        [HttpPost]
        public IActionResult AddMachinePart(machinePart p)
        {
            //var c = new context();
            _context.MachineParts.Add(p);
            _context.SaveChanges();
            return Created("", p);
        }

    }
}
