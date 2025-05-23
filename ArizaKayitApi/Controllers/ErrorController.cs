﻿using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EntityLayer.Concrete;
using DataAccessLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
namespace ArizaKayitApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ErrorController : ControllerBase
	{
		private readonly context _context;

		ErrorManager _errorManager;

		public ErrorController(context context)
		{
			_context = context;
			_errorManager = new ErrorManager(new EFErrorDal(context));

		}

		[HttpGet]
		public IActionResult GetAll()
		{
			var errors = _errorManager.TGetList();
			var infos = _context.Errors.Include(e => e.machines).Include(e => e.machinePartName).Include(e => e.userName).Select(e => new
			{
				e.id,
				machineName = e.machines.name,
				machinePartName = e.machinePartName.name,
				machineId = e.machineId,
				machinePartId = e.machinePartId,
				userName = e.userName.UserName,

			}).ToList();
			var response = new
			{
				errorModelList = errors,
				errorInfoModelList = infos
			};

			return Ok(response);
		}

		// GET: api/Error/Machine/5
		[HttpGet("Machine/{machineId}")]
		public IActionResult GetByMachineId(int machineId)
		{
			var errors = _errorManager.TGetList().Where(x => x.machineId == machineId).ToList();
			var infos = _context.Errors.Include(e => e.machines).Include(e => e.machinePartName).Include(e => e.userName).Where(e => e.machineId == machineId).Select(e => new
			{
				e.id,
				machineName = e.machines.name,
				machinePartName = e.machinePartName.name,
				machineId = e.machineId,
				machinePartId = e.machinePartId,
				userName = e.userName.UserName,

			}).ToList();
			var response = new
			{
				errorModelList = errors,
				errorInfoModelList = infos
			};
			return Ok(response);
		}
		[HttpGet("MachinePart/{machinePartId}")]
		public IActionResult GetByMachinePartId(int machinePartId)
		{
			var errors = _errorManager.TGetList().Where(x => x.machinePartId == machinePartId).ToList();
			var infos = _context.Errors.Include(e => e.machines).Include(e => e.machinePartName).Include(e => e.userName).Where(e => e.machinePartId == machinePartId).Select(e => new
			{
				e.id,
				machineName = e.machines.name,
				machinePartName = e.machinePartName.name,
				machineId = e.machineId,
				machinePartId = e.machinePartId,
				userName = e.userName.UserName,

			}).ToList();
			var response = new
			{
				errorModelList = errors,
				errorInfoModelList = infos
			};
			return Ok(response);
		}

		// GET: api/Error/5
		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			var error = _errorManager.TGetById(id);
			var ee = _context.Errors.Include(e => e.machines).Include(e => e.machinePartName).Where(e => e.id == id).Select(e => new
			{
				e.id,
				machineName = e.machines.name,
				machinePartName = e.machinePartName.name,
				machineId = e.machineId,
				machinePartId = e.machinePartId,
				userName = e.userName.UserName,
			}).FirstOrDefault();

			if (error == null)
			{
				return NotFound();
			}
			var response = new
			{
				errorModel = error,
				errorInfoModel = ee,
			};
			return Ok(response);
		}

		// POST: api/Error
		[HttpPost]
		public IActionResult Add([FromBody] error error)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (error != null && !string.IsNullOrEmpty(error.errorImage))
			{
				try
				{
					// Convert Base64 string to byte array
					error.errorImageBytes = Convert.FromBase64String(error.errorImage);
				}
				catch (FormatException ex)
				{
					// Handle invalid Base64 string format
					return BadRequest("Invalid image format.");
				}
			}
			if (error != null && !string.IsNullOrEmpty(error.errorImage))
			{
				Console.WriteLine($"Image data received: {error.errorImage.Length} bytes");
				Console.WriteLine($"Image type: {error.errorImageType}");
			}

			_errorManager.TAdd(error);
			return CreatedAtAction(nameof(GetById), new { id = error.id }, error);
		}

		// PUT: api/Error/5
		[HttpPut("{id}")]
		public IActionResult Update(int id, [FromBody] error error)
		{
			if (id != error.id)
			{
				return BadRequest();
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				_errorManager.TUpdate(error);
			}
			catch
			{
				if (_errorManager.TGetById(id) == null)
				{
					return NotFound();
				}
				throw;
			}

			return NoContent();
		}

		// DELETE: api/Error/5
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var error = _errorManager.TGetById(id);
			if (error == null)
			{
				return NotFound();
			}

			_errorManager.TRemove(error);
			return NoContent();
		}

		[HttpGet("GetUserId")]
		public IActionResult getUserId(String username)
		{
			var user = _context.Users.FirstOrDefault(u => u.UserName == username);
			if (user != null)
			{

				return Ok(user.Id);
			}

			return Ok(0);
		}
		[HttpGet("GetIdName")]
		public IActionResult getNames()
		{
			var machinNames = _context.Machines.Select(e => new
			{
				e.id,
				machineNames = e.name
			}).ToList();
			return Ok(machinNames);

		}
		[HttpGet("GetPartIdName")]
		public IActionResult getPartNames(int id)
		{
			var machinNames = _context.MachineParts.Where(w => w.machineId == id).Select(e => new
			{
				e.Id,
				machineNames = e.name
			}).ToList();
			return Ok(machinNames);

		}
		[HttpGet("GetInfoNames")]
		public IActionResult getErrorInfos()
		{
			var err = _context.Errors.Include(e => e.machines).Include(e => e.machinePartName).Include(e => e.userName).Select(e => new
			{
				e.id,
				machineName = e.machines.name,
				machinePartName = e.machinePartName.name,
				machinePartId = e.machinePartId,
				machineId = e.machineId,
				userName = e.userName.UserName

			}).ToList();



			return Ok(err);
		}
		[HttpGet("FilterListView")]
		public IActionResult filterListView([FromQuery] string? s1, [FromQuery] string? s2, [FromQuery] string? s3, [FromQuery] string? s4, [FromQuery] string? s5)
		{
			// Start with IQueryable instead of materializing to list immediately
			var query = _context.Errors
				.Include(e => e.machines)
				.Include(e => e.machinePartName)
				.Include(e => e.userName)
				.AsQueryable();

			// Apply date filter
			if (s1 != null)
			{
				var today = DateTime.Today;
				if (s1 == "Bugün")
				{
					query = query.Where(e => e.errorDate.Date == today);
				}
				else if (s1 == "Bu hafta")
				{
					var dayOfWeek = today.DayOfWeek;
					var startOfWeek = today.AddDays(-(int)dayOfWeek + (int)DayOfWeek.Monday);
					var endOfWeek = startOfWeek.AddDays(6);
					query = query.Where(e => e.errorDate >= startOfWeek && e.errorDate <= endOfWeek);
				}
				else if (s1 == "Bu ay")
				{
					var month = DateTime.Now.Month;
					var year = DateTime.Now.Year;
					query = query.Where(e => e.errorDate.Year == year && e.errorDate.Month == month);
				}
			}

			// Apply machine filter
			if (s2 != null)
			{
				query = query.Where(e => e.machines.name == s2);
			}

			// Apply machine part filter
			if (s3 != null)
			{
				query = query.Where(e => e.machinePartName.name == s3);
			}

			// Apply image filter
			if (s4 != null)
			{
				if (s4 == "Var")
				{
					query = query.Where(e => e.errorImage != null);
				}
				else
				{
					query = query.Where(e => e.errorImage == null);
				}
			}

			// Apply user filter
			if (s5 != null)
			{
				query = query.Where(e => e.userName.UserName == s5);
			}

			// Finally materialize query and select desired fields
			var results = query.Select(e => new
			{
				e.id,
				machineName = e.machines.name,
				machinePartName = e.machinePartName.name,
				machinePartId = e.machinePartId,
				machineId = e.machineId,
				userName = e.userName.UserName,
				startDate = e.errorDate,
				endDate = e.errorEndDate,
				errorImage = e.errorImage,
				errorType = e.errorType,
				errorDesc = e.errorDesc,
			}).ToList();
			return Ok(results);
		}

		[HttpGet("GetMachineNameAndPart/{id}")]
		public IActionResult getMachineNameAndPart(int id)
		{
			var ee = _context.Errors.Include(e => e.machines).Include(e => e.machinePartName).Where(e => e.id == id).Select(e => new
			{
				e.id,
				machineName = e.machines.name,
				machinePartName = e.machinePartName.name,
				machineId = e.machineId,
				machinePartId = e.machinePartId,
				userName = e.userName,
			});
			return Ok();
		}



		[HttpGet("GetErrorSupModel")]
		public IActionResult getErrorSupModel()
		{
			var err = _context.Errors.Include(e => e.machines).Include(e => e.machinePartName).Include(e => e.userName).Select(e => new
			{
				e.id,
				machineName = e.machines.name,
				machinePartName = e.machinePartName.name,
				machinePartId = e.machinePartId,
				machineId = e.machineId,
				userName = e.userName.UserName,
				startDate = e.errorDate,
				endDate = e.errorEndDate,
				errorImage = e.errorImage,
				errorType = e.errorType,
				errorDesc = e.errorDesc,
			}).ToList();
			return Ok(err);
		}
		[HttpGet("GetErrorSupModelByMachine/{id}")]
		public IActionResult getErrorSupModelByMachine(int id)
		{
			var err = _context.Errors.Include(e => e.machines).Include(e => e.machinePartName).Include(e => e.userName).Where(e=> e.machineId ==id).Select(e => new
			{
				e.id,
				machineName = e.machines.name,
				machinePartName = e.machinePartName.name,
				machinePartId = e.machinePartId,
				machineId = e.machineId,
				userName = e.userName.UserName,
				startDate = e.errorDate,
				endDate = e.errorEndDate,
				errorImage = e.errorImage,
				errorType = e.errorType,
				errorDesc = e.errorDesc,
			}).ToList();
			return Ok(err);
		}
		[HttpGet("GetErrorSupModelByMachinePart/{id}")]
		public IActionResult getErrorSupModelByMachinePart(int id)
		{
			var err = _context.Errors.Include(e => e.machines).Include(e => e.machinePartName).Include(e => e.userName).Where(e=> e.machinePartId == id).Select(e => new
			{
				e.id,
				machineName = e.machines.name,
				machinePartName = e.machinePartName.name,
				machinePartId = e.machinePartId,
				machineId = e.machineId,
				userName = e.userName.UserName,
				startDate = e.errorDate,
				endDate = e.errorEndDate,
				errorImage = e.errorImage,
				errorType = e.errorType,
				errorDesc = e.errorDesc,
			}).ToList();
			return Ok(err);
		}
	}
}
