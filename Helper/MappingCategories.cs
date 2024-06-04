namespace Profunion.Helper
{
    public class MappingCategories : Profile
    {
        public MappingCategories()
        {
            CreateMap<Categories, CategoriesDto>();

            CreateMap<CategoriesDto, Categories>();

            CreateMap<Categories, CreateCategoriesDto>();

            CreateMap<CreateCategoriesDto, Categories>();
        }
    }

}

