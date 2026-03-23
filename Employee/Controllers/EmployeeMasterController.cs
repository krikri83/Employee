using Employee.Api.DTO;
using Employee.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;

namespace Employee.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EmployeeMasterController : ControllerBase
	{
		private readonly EmployeeDbContext _context;
		private readonly IConfiguration _configuration;

		public EmployeeMasterController(EmployeeDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		// GET: api/employee => CRUD + FILTRE + TRI + PAGINATION
		[HttpGet("FiltreAllEmployee")]
		public async Task<IActionResult> Filtre([FromQuery] EmployeeQueryParameters query)
		{
			try
			{
				var employees = _context.Employees.AsQueryable();

				// --- FILTRE ---
				if (!string.IsNullOrWhiteSpace(query.Name))
					employees = employees.Where(e => e.name.Contains(query.Name));

				if (!string.IsNullOrWhiteSpace(query.Email))
					employees = employees.Where(e => e.email.Contains(query.Email));

				if (!string.IsNullOrWhiteSpace(query.City))
					employees = employees.Where(e => e.city.Contains(query.City));

				if (query.DesignationId.HasValue)
					employees = employees.Where(e => e.designationId == query.DesignationId.Value);

				// --- TRI ---
				var sortDirection = query.SortDesc ? "descending" : "ascending";
				employees = employees.OrderBy($"{query.SortBy} {sortDirection}");

				// --- PAGINATION ---
				var totalRecords = await employees.CountAsync();
				var pagedData = await employees
					.Skip((query.PageNumber - 1) * query.PageSize)
					.Take(query.PageSize)
					.Include(e => e.Designation)
					.ToListAsync();

				var response = new
				{
					TotalRecords = totalRecords,
					PageNumber = query.PageNumber,
					PageSize = query.PageSize,
					Data = pagedData
				};

				return Ok(response);
			}
			catch
			{
				return StatusCode(500, "Internal server error.");
			}
		}

		// GET: api/employee 
		[HttpGet("GetAllEmployees")]
		public IActionResult GetAll()
		{
			try
			{
				var employees = _context.Employees.AsQueryable();

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

		// POST: api/employee
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] EmployeeDTO employee)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);
				//Mapping
				EmployeeModel newModel = new EmployeeModel();
				newModel.employeeId = employee.employeeId;
				newModel.name = employee.name;
				newModel.email = employee.email;
				newModel.address = employee.address;
				newModel.pincode = employee.pincode;
				newModel.role = employee.role;
				newModel.contactNo = employee.contactNo;
				newModel.altContactNo = employee.altContactNo;
				newModel.city = employee.city;
				newModel.createdDate = employee.createdDate;
				newModel.Designation = null;
				newModel.designationId = employee.Designation != null ? employee.Designation.designationId : 0;
				newModel.designationName = employee.Designation != null ? employee.Designation.designationName : "" ;
				newModel.modifiedDate = employee.modifiedDate;


				// FK check
				if (!await _context.Designations.AnyAsync(d => d.designationId == newModel.designationId))
					return BadRequest("Invalid designationId.");

				// Unicité contactNo
				if (await _context.Employees.AnyAsync(e => e.contactNo == employee.contactNo))
					return Conflict("contactNo already exists.");

				// Unicité email
				if (await _context.Employees.AnyAsync(e => e.email.ToLower() == employee.email.ToLower()))
					return Conflict("Email already exists.");

				employee.createdDate = DateTime.UtcNow;
				employee.modifiedDate = DateTime.UtcNow;

				await _context.Employees.AddAsync(newModel);
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
		public async Task<IActionResult> Update(int id, [FromBody] EmployeeModel employee)
		{
			try
			{
				if (id != employee.employeeId)
					return BadRequest("ID mismatch.");

				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var existing = await _context.Employees.FirstOrDefaultAsync(e => e.employeeId == id);
				if (existing == null)
					return NotFound("Employee not found.");

				// FK check
				if (!await _context.Designations.AnyAsync(d => d.designationId == employee.designationId))
					return BadRequest("Invalid designationId.");

				// contactNo unique pour les autres
				if (await _context.Employees.AnyAsync(e => e.contactNo == employee.contactNo && e.employeeId != id))
					return Conflict("contactNo already exists.");

				// email unique pour les autres
				if (await _context.Employees.AnyAsync(e => e.email.ToLower() == employee.email.ToLower() && e.employeeId != id))
					return Conflict("Email already exists.");

				// Update
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
			catch (DbUpdateException)
			{
				return Conflict("Employee contactNo and email must be unique.");
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

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequest request)
		{
			try
			{
				if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.ContactNo))
				return BadRequest("Email or ContactNo is required.");

				var employee = await _context.Employees
					.FirstOrDefaultAsync(e =>
						(request.Email != null && e.email == request.Email) ||
						(request.ContactNo != null && e.contactNo == request.ContactNo));

				if (employee == null)
					return Unauthorized("Invalid credentials.");

				var token = GenerateToken(employee.email, employee.name);

				EmployeeDTO employeeDTO = new EmployeeDTO();
				employeeDTO.employeeId = employee.employeeId;
				employeeDTO.name = employee.name;
				employeeDTO.email = employee.email;
				employeeDTO.contactNo = employee.contactNo;
				employeeDTO.Designation = new Designation();
				employeeDTO.Designation.departmentId = employee.designationId;
				employeeDTO.Designation.designationName = employee.designationName;
				employeeDTO.role = employee.role; 

				LoginInfo loginInfo = new LoginInfo(token, employeeDTO);

				return Ok(new
			{
				message= "Login Successful",
				
				data = new
				{
					loginInfo.Token,
					loginInfo.employeeDTO
				}
			});
			}
			catch (Exception ex)
			{

				return StatusCode(500, ex.Message);
			}
			
		}
		private string GenerateToken(string email, string name)
		{
			var claims = new[]
			{
		new Claim(ClaimTypes.Name, name),
		new Claim(ClaimTypes.Email, email)
	};

			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
			);

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddHours(2),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
