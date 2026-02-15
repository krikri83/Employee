using Employee.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

[Route("api/[controller]")]
[ApiController]
public class DesignationController : ControllerBase
{
	private readonly EmployeeDbContext _context;

	public DesignationController(EmployeeDbContext context)
	{
		_context = context;
	}

	// GET: api/designation
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Designation>>> GetAll()
	{
		try
		{
			var data = await _context.Designations.ToListAsync();
			return Ok(data);
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex.Message}");
		}
	}

	// GET: api/designation/5
	[HttpGet("{id}")]
	public async Task<ActionResult<Designation>> GetById(int id)
	{
		try
		{
			var designation = await _context.Designations.FindAsync(id);

			if (designation == null)
				return NotFound($"Designation with id {id} not found.");

			return Ok(designation);
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex.Message}");
		}
	}

	// POST: api/designation
	[HttpPost]
	public async Task<ActionResult<Designation>> Create([FromBody] Designation designation)
	{
		try
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			await _context.Designations.AddAsync(designation);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById),
				new { id = designation.designationId },
				designation);
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex.Message}");
		}
	}

	// PUT: api/designation/5
	[HttpPut("{id}")]
	public async Task<IActionResult> Update(int id, [FromBody] Designation designation)
	{
		try
		{
			if (id != designation.designationId)
				return BadRequest("ID mismatch.");

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var existing = await _context.Designations.FindAsync(id);

			if (existing == null)
				return NotFound($"Designation with id {id} not found.");

			existing.departmentId = designation.departmentId;
			existing.designationName = designation.designationName;

			await _context.SaveChangesAsync();

			return Ok(existing);
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex.Message}");
		}
	}

	// DELETE: api/designation/5
	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		try
		{
			var designation = await _context.Designations.FindAsync(id);

			if (designation == null)
				return NotFound($"Designation with id {id} not found.");

			_context.Designations.Remove(designation);
			await _context.SaveChangesAsync();

			return Ok($"Designation with id {id} deleted successfully.");
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex.Message}");
		}
	}
}
