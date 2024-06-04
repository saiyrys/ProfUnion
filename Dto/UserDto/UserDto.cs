namespace Profunion.Dto.UserDto
{
    public class UserDto
    {
        public string? userId { get; set; }
        public string? userName { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? middleName { get; set; }
        public string email { get; set; }
        public string? password { get; set; }
        public string role { get; set; }
        public DateTime createdAt { get; set; }
    }
}
