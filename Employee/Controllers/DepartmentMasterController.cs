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
	}
}

