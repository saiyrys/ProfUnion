using Profunion.Interfaces.ReservationInterface;
using Profunion.Models.Events;
using Profunion.Services.AdditionalServices;

namespace Profunion.Services.ApplicationServices
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IReservationList _reservationList;
        private readonly IRejectedApplicationRepository _rejected;
        private readonly IEmailSender _emailSender;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly Helpers _helper;
        private readonly DataContext _context;
        public ApplicationService(IApplicationRepository applicationRepository,
            IUserRepository userRepository,
            IMapper mapper,
            IEventRepository eventRepository,
            IReservationList reservationList,
            IRejectedApplicationRepository rejected,
            Helpers helper,
            IEmailSender emailSender,
            DataContext context)
        {
            _applicationRepository = applicationRepository;
            _eventRepository = eventRepository;
            _reservationList = reservationList;
            _rejected = rejected;
            _userRepository = userRepository;
            _mapper = mapper;
            _helper = helper;
            _emailSender = emailSender;
            _context = context;
        }
        public async Task<(IEnumerable<GetApplicationDto>, int TotalPages)> GetApplication(int page, string search = null, string sort = null, string type = null)
        {
            int pageSize = 12;

            var applications = await _applicationRepository.GetAllApplications();

            if (search != null || sort != null || type != null)
            {
                applications = await _applicationRepository.SearchAndSortApplications(search, sort, type);
            }

            var applicationsDto = _mapper.Map<List<GetApplicationDto>>(applications);

            foreach (var application in applicationsDto)
            {
                var user = await _userRepository.GetUserByID(application.userId);
                application.user = user;

                var events = await _eventRepository.GetEventsByID(application.eventId);
                application.events = events;
            }

            var pagination = await _helper.ApplyPaginations(applicationsDto, page, pageSize);
            applicationsDto = pagination.Item1;

            var totalPages = pagination.Item2;

            return (applicationsDto, totalPages);
        }

        public async Task<List<GetUserApplicationDto>> GetUserApplication(string userId)
        {
            var user = await _userRepository.GetUserByID(userId);
            var userMap = _mapper.Map<GetUserDto>(user);

            if (user == null)
                throw new ArgumentException("user not exist");

            var applications = _mapper.Map<List<GetUserApplicationDto>>(await _applicationRepository.GetAllApplications());

            var userApplications = applications.Where(app => app.userId == userMap.userId).ToList();

            /*if (userApplications.Any())*/
                return userApplications;

            /*throw new ArgumentNullException();*/
        }
        public async Task<bool> CreateApplication(CreateApplicationDto createApplication)
        {
            var appMap = _mapper.Map<Application>(createApplication);

            var user = await _userRepository.GetUserByID(createApplication.userId);

            if (appMap.status != "REJECTED" || appMap.status != "APPROVED")
                appMap.status = "PENDING";

            if (!await _applicationRepository.CreateApplication(appMap))
            {
               throw new ArgumentException();
            }

           /* await _emailSender.SendMessageAboutApplication(user.userId, createApplication.eventId);*/
            return true;
        }

        public async Task<bool> UpdateApplication(string Id, UpdateApplicationDto updateApplication)
        {
            var application = await _applicationRepository.GetApplicationsByID(Id);

            await _helper.UpdateEntity<Application, UpdateApplicationDto>(application.Id, updateApplication);

            var success = await _applicationRepository.UpdateApplication(application);

            if(updateApplication.status == "APPROVED")
            {
                return await CreateReservation(application);
            }

            if (updateApplication.status == "REJECTED")
            {

                return await CreateRejected(application);
            }

            return true;

        }

        private async Task<bool> CreateReservation(Application application)
        {
            var currentEvent = await _eventRepository.GetEventsByID(application.EventId);

            if (currentEvent.totalTickets != 0 && currentEvent.totalTickets >= application.ticketsCount)
            {
                currentEvent.totalTickets -= application.ticketsCount;
                await _eventRepository.UpdateEvents(currentEvent);
            }

            var reservationList = new CreateReservationDto
            {
                eventId = application.EventId,
                userId = application.UserId,
                ticketsCount = application.ticketsCount,
                createdAt = DateTime.Now
            };

            var createReservation = _mapper.Map<ReservationList>(reservationList);

            if (!await _reservationList.CreateReservation(createReservation))
            {
                throw new ArgumentNullException();  
            }

            /*await _emailSender.SendMessageAboutApply(reservationList.userId, reservationList.eventId);*/
            return true;

                     
        }
        private async Task<bool> CreateRejected(Application application)
        {
            var rejectedApplication = new CreateRejectedApplicationDto
            {
                eventId = application.EventId,
                userId = application.UserId,
                ticketsCount = application.ticketsCount,
                createdAt = DateTime.Now
            };
            var createRejected = _mapper.Map<RejectedApplication>(rejectedApplication);

            if (!await _rejected.CreateRejected(createRejected))
            {
                throw new ArgumentNullException();
            }
                        
           /* await _emailSender.SendMessageAboutRejected(rejectedApplication.userId, rejectedApplication.eventId);*/
            return true;
        }
    }
}
