namespace Employee.Api.DTO
{
	public class EmployeeQueryParameters
	{
		public string? Name { get; set; }
		public string? Email { get; set; }
		public int? DesignationId { get; set; }
		public string? City { get; set; }

		public string SortBy { get; set; } = "createdDate";
		public bool SortDesc { get; set; } = false;

		private int _pageNumber = 1;
		public int PageNumber
		{
			get => _pageNumber;
			set => _pageNumber = (value < 1) ? 1 : value;
		}

		private int _pageSize = 10;
		public int PageSize
		{
			get => _pageSize;
			set => _pageSize = (value > 50) ? 50 : (value < 1) ? 1 : value;
		}
	}

}
