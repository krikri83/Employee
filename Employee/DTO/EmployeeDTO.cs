using Employee.Api.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.Api.DTO
{
	public class EmployeeDTO
	{
		[Required]
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


		public string address { get; set; } = string.Empty;


		[ForeignKey("designationId")]
		public Designation? Designation { get; set; }

		public DateTime createdDate { get; set; }
		public DateTime modifiedDate { get; set; }
		public string role { get; set; } = string.Empty;
	}
}
