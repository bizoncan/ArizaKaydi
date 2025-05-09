using ArizaKaydi.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;

namespace ArizaKaydi.Controllers
{
	
	public class DefaultController : Controller
    {
        MachineManager machineManager;
		MachinePartManager machinePartManager;
		ErrorManager errorManager;
		context _context;
		public DefaultController(context _context)
        {
            this._context = _context;
			errorManager = new ErrorManager(new EFErrorDal(_context));
			machineManager = new MachineManager(new EFMachineDal(_context));
            machinePartManager = new MachinePartManager(new EFMachinePartDal(_context));
		}

        public IActionResult Index(int? selectedMachineId)
        {
            MachinePartViewModel model = new MachinePartViewModel();
            model.machines = machineManager.TGetList();
            model.selectedMachineId = selectedMachineId;
			if (model.selectedMachineId != null)
            {
                model.machineParts  = _context.MachineParts.Where(x => x.machineId == model.selectedMachineId)
					.ToList();
			}
            return View(model);
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
        [HttpGet]
        public IActionResult AddMachinePart(int id)
		{
			ViewBag.MachineId = id;
			return View();
		}
        [HttpPost]
		public IActionResult AddMachinePart(machinePart p)
        {
			machinePartManager.TAdd(p);
			return RedirectToAction("Index", new { selectedMachineId = p.machineId});
		}
        public IActionResult RemoveMachinePart(int id)
		{
			var value = machinePartManager.TGetById(id);
			machinePartManager.TRemove(value);
			return RedirectToAction("Index" ,new { selectedMachineId = value.machineId});
		}
		[HttpGet]
		public IActionResult EditMachinePart(int id)
		{
			var values = machinePartManager.TGetById(id);
			return View(values);
		}
		[HttpPost]
		public IActionResult EditMachinePart(machinePart p)
		{
			machinePartManager.TUpdate(p);
			return RedirectToAction("Index", new { selectedMachineId = p.machineId });
		}
	}
}
