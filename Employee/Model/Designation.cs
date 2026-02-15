using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.Model
{
	[Table("designationTbl")]
	public class Designation
	{
		public int designationId { get; set; }
		public int departmentId { get; set; }
		[Required, MaxLength(50)]
		public string designationName { get; set; } = string.Empty;
		
	}
}
