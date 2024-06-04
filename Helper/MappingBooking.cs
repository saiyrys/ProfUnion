namespace Profunion.Helper
{
    public class MappingBooking : Profile
    {
        public MappingBooking()
        {

            CreateMap<Application, CreateApplicationDto>();

            CreateMap<CreateApplicationDto, Application>();


            CreateMap<Application, GetApplicationDto>();


            CreateMap<UpdateApplicationDto, Application>();

            CreateMap<Application, UpdateApplicationDto>();


            CreateMap<RejectedApplication, CreateRejectedApplicationDto>();

            CreateMap<CreateRejectedApplicationDto, RejectedApplication>();


            CreateMap<RejectedApplication, GetRejectedApplicationDto>();
        }

    }
}
