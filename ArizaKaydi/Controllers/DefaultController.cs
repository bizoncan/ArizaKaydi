using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;

namespace ArizaKaydi.Controllers
{
    public class DefaultController : Controller
    {
        MachineManager machineManager;
		ErrorManager errorManager;

		public DefaultController(context _context)
        {
            errorManager = new ErrorManager(new EFErrorDal(_context));
			machineManager = new MachineManager(new EFMachineDal(_context));
            
        }

        public IActionResult Index()
        {
            var values = machineManager.TGetList();
            return View(values);
        }
        [HttpGet]
        public IActionResult AddMachine()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddMachine(machine p)
        {
            machineManager.TAdd(p);
            return RedirectToAction("Index");
        }
        public IActionResult RemoveMachine(int id)
        {
            var value = machineManager.TGetById(id);
            machineManager.TRemove(value);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult EditMachine(int id) 
        {
        var values= machineManager.TGetById(id);    
        return View(values);    
        }
        [HttpPost]
        public IActionResult EditMachine(machine p)
        {
            machineManager.TUpdate(p);
            return RedirectToAction("Index");
        }

        
    }
}
