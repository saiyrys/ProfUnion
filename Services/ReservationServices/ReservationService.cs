using Profunion.Interfaces.ReservationInterface;
using Profunion.Models.Events;
using Profunion.Services.AdditionalServices;

namespace Profunion.Services.ReservationServices
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationList _reservationList;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private readonly Helpers _helper;
        public ReservationService(IReservationList reservationList, IMapper mapper, IUserRepository userRepository, IEventRepository eventRepository, Helpers helper)
        {
            _reservationList = reservationList;
            _mapper = mapper;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _helper = helper;
        }

        public async Task<(IEnumerable<GetReservationDto> Reservation, int TotalPage)> GetAllReservation(int page, string search = null, string sort = null, string type = null)
        {
            int pageSize = 12;

            var reservations = await _reservationList.GetAllReservation();

            if (search != null || sort != null || type != null)
            {
                reservations = await _reservationList.SearchAndSortReservation(search, sort, type);
            }

            var reservationsDto = _mapper.Map<List<GetReservationDto>>(reservations);

            foreach (var reservation in reservationsDto)
            {
                var user = await _userRepository.GetUserByID(reservation.userId);
                reservation.user = _mapper.Map<GetUserDto>(user);

                var events = await _eventRepository.GetEvents();
                var currentEvents = events.Where(e => e.eventId == reservation.eventId).FirstOrDefault();
                reservation.events = currentEvents;
            }

            var pagination = await _helper.ApplyPaginations(reservationsDto, page, pageSize);
            reservationsDto = pagination.Item1;

            var totalPages = pagination.Item2;

       
            return (reservationsDto, totalPages);
        }

        public async Task<(IEnumerable<GetReservationDto> reservations, int totalTickets)> GetUserReservation(string userId)
        {
            var allReservations = await _reservationList.GetAllReservation();
            var reservations = _mapper.Map<List<GetReservationDto>>(allReservations);

            // Фильтруем резервации только для указанного пользователя
            var userReservations = reservations.Where(r => r.userId == userId).ToList();

            var eventsWithCategories = await _eventRepository.GetEvents();

            foreach (var reservation in userReservations)
            {
                var currentEvent = eventsWithCategories.FirstOrDefault(e => e.eventId == reservation.eventId);
                reservation.events = currentEvent;
            }

            int totalTickets = userReservations.Sum(r => r.ticketsCount);

            return (userReservations, totalTickets);
        }

        public async Task<UpdateReservationDto> UpdateReservation(string Id, UpdateReservationDto updateReservation)
        {
            var reservation = await _reservationList.GetReservationByID(Id);

            int ticketDifference = updateReservation.ticketsCount - reservation.ticketsCount;
            await _helper.UpdateEntity<ReservationList, UpdateReservationDto>(Id, updateReservation);

            var eventEntity = await _eventRepository.GetEventsByID(reservation.EventId);

            if (updateReservation.ticketsCount > reservation.ticketsCount)
            {
                // Если количество билетов увеличилось, уменьшаем их у события
                eventEntity.totalTickets -= ticketDifference;
            }
            else if (updateReservation.ticketsCount < reservation.ticketsCount)
            {
                // Если количество билетов уменьшилось, увеличиваем их у события
                eventEntity.totalTickets -= ticketDifference;
            }

            // Сохраняем обновленное событие
            await _helper.UpdateEntity<Event, Event>(eventEntity.eventId, eventEntity);

            return updateReservation;
        }
    }
}
