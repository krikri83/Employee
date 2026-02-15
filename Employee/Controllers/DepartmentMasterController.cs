using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Employee.Api.Model;

namespace Employee.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DepartmentMasterController : ControllerBase
	{
		private readonly EmployeeDbContext _context;
		public DepartmentMasterController(EmployeeDbContext employeeDbContext)
		{
			_context = employeeDbContext;
		}
		[HttpGet("GetAllDepartments")]
		public ActionResult GetDepartment() 
		{
			var deptList = _context.Departments.ToList();
			return Ok(deptList);
		}

		[HttpPost("AddDepartment")]
		public async Task<IActionResult> AddDepartmentAsync(Department department)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				// Vérifier si le nom existe déjà
				bool exists = await _context.Departments
					.AnyAsync(d => d.departmentName.ToLower() == department.departmentName.ToLower());

				if (exists)
					return Conflict("Department name already exists.");

				await _context.Departments.AddAsync(department);
				await _context.SaveChangesAsync();

				return Ok("Department added successfully.");
			}
			catch (DbUpdateException)
			{
				return Conflict("Department name must be unique.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpPut("UpdateDepartment")]
		public IActionResult UpdateDepartment(Department department)
		{
			var existingDept = _context.Departments.Find(department.departmentId);
			if(existingDept == null)
			{
				return NotFound("Department not found");
			}

			existingDept.departmentName = department.departmentName;
			existingDept.isActive = department.isActive;
			_context.SaveChanges();
			return Ok("Department Updated Successfully");
		}

		[HttpDelete("DeleteDepartmenet/{id}")]
		public IActionResult DeleteDepartment(int id)
		{
			var existingDept = _context.Departments.Find(id);
			if (existingDept == null)
			{
				return NotFound("Department not found");
			}
			_context.Departments.Remove(existingDept);
			_context.SaveChanges();
			return Ok("Department Deleted Successfully");

		}
	}
}

