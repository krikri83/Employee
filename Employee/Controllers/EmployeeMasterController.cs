using Employee.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EmployeeMasterController : ControllerBase
	{
		private readonly EmployeeDbContext _context;

		public EmployeeMasterController(EmployeeDbContext context)
		{
			_context = context;
		}

		// GET: api/employee
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var employees = await _context.Employees
					.Include(e => e.Designation)
					.ToListAsync();

				return Ok(employees);
			}
			catch
			{
				return StatusCode(500, "Internal server error.");
			}
		}

		// GET: api/employee/5
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			try
			{
				var employee = await _context.Employees
					.Include(e => e.Designation)
					.FirstOrDefaultAsync(e => e.employeeId == id);

				if (employee == null)
					return NotFound("Employee not found.");

				return Ok(employee);
			}
			catch
			{
				return StatusCode(500, "Internal server error.");
			}
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] Model.Employee employee)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				// Vérifier le DesignationId
				var designationExists = await _context.Designations
					.AnyAsync(d => d.designationId == employee.designationId);
				if (!designationExists)
					return BadRequest("Invalid designationId.");

				// Vérifier contactNo unique
				if (await _context.Employees.AnyAsync(e => e.contactNo == employee.contactNo))
					return Conflict("contactNo already exists.");

				// Vérifier email unique
				if (await _context.Employees.AnyAsync(e => e.email.ToLower() == employee.email.ToLower()))
					return Conflict("Email already exists.");

				employee.createdDate = DateTime.UtcNow;
				employee.modifiedDate = DateTime.UtcNow;

				await _context.Employees.AddAsync(employee);
				await _context.SaveChangesAsync();

				return CreatedAtAction(nameof(GetById),
					new { id = employee.employeeId },
					employee);
			}
			catch (DbUpdateException)
			{
				return Conflict("Employee contactNo and email must be unique.");
			}
			catch
			{
				return StatusCode(500, "Internal server error.");
			}
		}

		// PUT: api/employee/5
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] Model.Employee employee)
		{
			try
			{
				if (id != employee.employeeId)
					return BadRequest("ID mismatch.");

				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var existing = await _context.Employees
					.FirstOrDefaultAsync(e => e.employeeId == id);

				if (existing == null)
					return NotFound("Employee not found.");

				var designationExists = await _context.Designations
					.AnyAsync(d => d.designationId == employee.designationId);

				if (!designationExists)
					return BadRequest("Invalid designationId.");

				existing.name = employee.name;
				existing.contactNo = employee.contactNo;
				existing.email = employee.email;
				existing.city = employee.city;
				existing.state = employee.state;
				existing.pincode = employee.pincode;
				existing.altContactNo = employee.altContactNo;
				existing.designationName = employee.designationName;
				existing.address = employee.address;
				existing.designationId = employee.designationId;
				existing.modifiedDate = DateTime.UtcNow;

				await _context.SaveChangesAsync();

				return Ok("Employee updated successfully.");
			}
			catch
			{
				return StatusCode(500, "Internal server error.");
			}
		}

		// DELETE: api/employee/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var employee = await _context.Employees.FindAsync(id);

				if (employee == null)
					return NotFound("Employee not found.");

				_context.Employees.Remove(employee);
				await _context.SaveChangesAsync();

				return Ok("Employee deleted successfully.");
			}
			catch
			{
				return StatusCode(500, "Internal server error.");
			}
		}

	}
}
