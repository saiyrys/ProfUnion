namespace Profunion.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            
            CreateMap<User, LoginUserDto>();
            CreateMap<LoginUserDto, User>();

            CreateMap<LoginUserDto, GetUserDto>();
            CreateMap<GetUserDto, LoginUserDto>();

            CreateMap<User, RegisterDto>();
            CreateMap<RegisterDto, User>();

            CreateMap<User, GetUserDto>();

            CreateMap<UpdateUserDto, User>();
            CreateMap<User, UpdateUserDto>();

        }
    }
}
