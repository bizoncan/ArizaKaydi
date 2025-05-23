using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using DataAccessLayer.Concrete;
namespace ArizaKayitApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachineController : ControllerBase
    {
        private readonly context _context;
        private readonly MachineManager  _machineManager;

        public MachineController(context context)
        {
            _context = context;
            _machineManager = new MachineManager(new EFMachineDal(context));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var machines = _machineManager.TGetList();
            return Ok(machines);
        }

        // GET: api/Machine/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var machine = _machineManager.TGetById(id);
            if (machine == null)
            {
                return NotFound();
            }
            return Ok(machine);
        }

        // POST: api/Machine
        [HttpPost]
        public IActionResult Add([FromBody] machine machine)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _machineManager.TAdd(machine);
            return CreatedAtAction(nameof(GetById), new { id = machine.id }, machine);
        }

        // PUT: api/Machine/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] machine machine)
        {
            if (id != machine.id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _machineManager.TUpdate(machine);
            }
            catch
            {
                if (_machineManager.TGetById(id) == null)
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Machine/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var machine = _machineManager.TGetById(id);
            if (machine == null)
            {
                return NotFound();
            }

            _machineManager.TRemove(machine);
            return NoContent();
        }
        [HttpGet("GetMachineWithParts")]
        public IActionResult GetMachineWithParts()
        {
			var machines = _context.Machines.ToList();
            var machineParts = _context.MachineParts.ToList();


			return Ok(new { machineModels = machines, machinePartModels= machineParts });
        }
    }
}

