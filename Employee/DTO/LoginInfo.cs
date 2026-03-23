namespace Employee.Api.DTO
{
	public class LoginInfo
	{
		public LoginInfo(string token, EmployeeDTO employeeDTO)
		{
			Token = token;
			this.employeeDTO = employeeDTO;
		}

		public string Token { get; set; }
		public EmployeeDTO employeeDTO { get; set; }
	}
}
