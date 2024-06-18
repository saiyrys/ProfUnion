using DocumentFormat.OpenXml.Vml.Office;

namespace Profunion.Services.AdditionalServices
{

    public class Helpers
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Helpers(DataContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Tuple<List<T>, int>> ApplyPaginations<T>(List<T> items, int page, int pageSize)
        {
            int totalItems = items.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            int skip = (page - 1) * pageSize;
            var itemsForPage = items.Skip(skip).Take(pageSize).ToList();

            return Tuple.Create(itemsForPage, totalPages);
        }

        
        public string VerifyByAccessToken()
        {
            string accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];

            if (accessToken.StartsWith("Bearer"))
            {
                accessToken = accessToken.Substring("Bearer".Length).Trim();
            }

            else
            {
                return ("Некорректный формат токена доступа");
            }

            return accessToken;
        }
        public async Task<bool> UpdateEntity<TEntity, TUpdateDto>(string id, TUpdateDto updateDto)
            where TEntity : class
            where TUpdateDto : class
        {
            var updateModel = await _context.Set<TEntity>().FindAsync(id);
            if (updateModel == null)
                throw new ArgumentException($"{typeof(TEntity).Name} does not exist");

            var modelType = typeof(TEntity);
            var updateDtoType = typeof(TUpdateDto);

            foreach (var property in updateDtoType.GetProperties())
            {
                var newValue = property.GetValue(updateDto);
                if (newValue != null)
                {
                    var entityProperty = modelType.GetProperty(property.Name);
                    if (entityProperty != null && entityProperty.CanWrite)
                    {
                        entityProperty.SetValue(updateModel, newValue);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        


    }
}
