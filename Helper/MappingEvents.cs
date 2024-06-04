namespace Profunion.Helper
{
    public class MappingEvents : Profile
    {
        public MappingEvents() 
        { 
            CreateMap<Event, GetEventDto>();
            CreateMap<GetEventDto, Event>();

            CreateMap<string, Categories>()
            .ConstructUsing(src => new Categories { Id = src });

            CreateMap<CreateEventDto, Event>();
            CreateMap<Event, CreateEventDto>();


            CreateMap<UpdateEventDto, Event>();
            CreateMap<Event, UpdateEventDto>();

            CreateMap<DeleteEventDto, Event>();
            CreateMap<Event, DeleteEventDto>();
        }
    }
}
