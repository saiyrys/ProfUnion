namespace Profunion.Helper
{
    public class MappingReservation : Profile
    {
        public MappingReservation() 
        { 
            CreateMap<CreateReservationDto, ReservationList>();
            CreateMap<ReservationList, CreateReservationDto>();

            CreateMap<ReservationList, GetReservationDto>();
            CreateMap<GetReservationDto, ReservationList>();

            CreateMap<UpdateReservationDto, ReservationList>();
            CreateMap<ReservationList, UpdateReservationDto>();

        }
    }
}
