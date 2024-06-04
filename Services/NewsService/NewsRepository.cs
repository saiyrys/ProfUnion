namespace Profunion.Services.NewsService
{
    public class NewsRepository : INewsRepository
    {
        private readonly DataContext _context;
        public NewsRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<ICollection<GetNewsDto>> GetNews()
        {
            string baseUrl = "http://profunions.ru/api/upload/";

            var newses = await _context.News
                .Include(n => n.NewsUploads)
                    .ThenInclude(nu => nu.Uploads)
                .Select(n => new GetNewsDto
                {
                    newsId = n.newsId,
                    title = n.title,
                    content = n.content,
                    description = n.description,
                    images = n.NewsUploads.Select(nu => new GetUploadsDto
                    {
                        id = nu.fileId,
                        Url = $"{baseUrl}{_context.Uploads.FirstOrDefault(u => u.id == nu.fileId).fileName}"
                    }).ToList(),
                    createdAt = n.createdAt.ToString("yyyy-MM-dd"),
                    updatedAt = n.updatedAt.ToString("yyyy-MM-dd"),
                    views = n.views
                }).ToListAsync();

            return newses;
        }
        public async Task<News> GetNewsById(string newsId)
        {
            return await _context.News.Where(n => n.newsId == newsId).FirstOrDefaultAsync();
        }

        public async Task<ICollection<GetNewsDto>> SearchAndSort(string search = null, string sort = null, string type = null)
        {
            var newses = await GetNews();
            IQueryable<GetNewsDto> query = newses.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                query = query.Where(n =>
                n.title.ToLower().Contains(search) ||
                n.description.ToLower().Contains(search) ||
                n.content.ToLower().Contains(search)
                );
            }

            if (!string.IsNullOrEmpty(sort) || !string.IsNullOrEmpty(type))
            {
                switch (sort.ToLower())
                {
                    case "alphabetic":
                        if (type.ToLower() == "inc")
                            query = query.OrderBy(n => n.title);
                        else if (type.ToLower() == "dec")
                            query = query.OrderByDescending(n => n.title);
                        break;
                    case "date":
                        if (type.ToLower() == "inc")
                            query = query.OrderBy(n => n.createdAt);
                        else if (type.ToLower() == "dec")
                            query = query.OrderByDescending(n => n.createdAt);
                        break;
                }
            }

            return await query.ToListAsync();
        }
        public async Task<bool> CreateNews(News news)
        {
            news.newsId = Guid.NewGuid().ToString();

            _context.Add(news);

            await _context.SaveChangesAsync();

            return await SaveNews();
        }
        public async Task<bool> UpdateNews(News news)
        {
            _context.News.Update(news);

            return await SaveNews();
        }
       
        public async Task<bool> DeleteNews(News news)
        {
            _context.Remove(news);

            return await SaveNews();
        }

        public async Task<bool> SaveNews()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
