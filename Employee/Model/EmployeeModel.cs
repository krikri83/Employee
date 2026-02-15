using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Employee.Api.Model
{
	[Table("EmployeeTbl")]
	[Index(nameof(contactNo), IsUnique = true)]
	[Index(nameof(email), IsUnique = true)]
	public class EmployeeModel
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int employeeId { get; set; }

		[Required]
		public string name { get; set; } = string.Empty;

		[Required, MaxLength(10), MinLength(10)]
		public string contactNo { get; set; } = string.Empty;

		[Required, EmailAddress]
		public string email { get; set; } = string.Empty;

		public string city { get; set; } = string.Empty;
		public string state { get; set; } = string.Empty;
		public string pincode { get; set; } = string.Empty;
		public string altContactNo { get; set; } = string.Empty;

		public string designationName { get; set; } = string.Empty;

		public string address { get; set; } = string.Empty;

		public int designationId { get; set; }

		[ForeignKey("designationId")]
		public Designation? Designation { get; set; }

		public DateTime createdDate { get; set; }
		public DateTime modifiedDate { get; set; }
	}

}
