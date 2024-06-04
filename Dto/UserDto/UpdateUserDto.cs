namespace Profunion.Dto.UserDto
{
    public class UpdateUserDto
    {
        public string? userName { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? middleName { get; set; }
        public string email { get; set; }
        public string? password { get; set; }
        public string role { get; set; }
    }
}
