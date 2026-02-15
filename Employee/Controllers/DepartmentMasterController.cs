using Employee.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Controllers
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
		public IActionResult AddDepartment(Department department)
		{
			_context.Departments.Add(department);
			_context.SaveChanges();
			return Ok("Deptartement Addes Sucessfully");
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

