namespace Profunion.Helper
{
    public class MappingUploads : Profile
    {
        public MappingUploads()
        {

            CreateMap<Uploads, CreateUploadsDto>();

            CreateMap<CreateUploadsDto, Uploads>();

        }

    }
}
