namespace Profunion.Helper
{
    public class MappingNews : Profile
    {
        public MappingNews()
        {
            CreateMap<News, CreateNewsDto>();
            CreateMap<CreateNewsDto, News>();

            CreateMap<News, GetNewsDto>();
            CreateMap<GetNewsDto, News>();

        }

    }
}
