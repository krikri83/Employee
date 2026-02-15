using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.Model
{
	[Table("EmployeeTbl")]
	public class Employee
	{
		public int employeeId { get; set; }
		public int name { get; set; }
	}
}
