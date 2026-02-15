using Employee.Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Employee.Api.Model
{
	public class EmployeeDbContext : DbContext
	{
		public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options)
		{

		}

		public DbSet<Employee> Employees { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<Designation> Designations { get; set; }
	}
}
