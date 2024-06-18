namespace Profunion.Services.NewsServices
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IMapper _mapper;
        private readonly Helpers _helper;
        private readonly DataContext _context;
        private readonly CascadeDeleteMethods _cascade;
        public NewsService(INewsRepository newsRepository, IFileRepository fileRepository, IMapper mapper, Helpers helper, DataContext context, CascadeDeleteMethods cascade)
        {
            _newsRepository = newsRepository;
            _fileRepository = fileRepository;
            _mapper = mapper;
            _helper = helper;
            _context = context;
            _cascade = cascade;
        }

        public async Task<(IEnumerable<GetNewsDto> newses, int TotalPage)> GetNewses(int page, string search = null, string sort = null, string type = null)
        {
            int pageSize = 12;

            var newses = await _newsRepository.GetNews();

            if (search != null || sort != null || type != null)
            {
                newses = await _newsRepository.SearchAndSort(search, sort, type);
            }

            var newsesDto = _mapper.Map<List<GetNewsDto>>(newses);

            var pagination = await _helper.ApplyPaginations(newsesDto, page, pageSize);

            newsesDto = pagination.Item1;
            var totalPage = pagination.Item2;

            return (newsesDto, totalPage);
        }
        public async Task<News> GetNews(string newsId)
        {
            var newses = await _newsRepository.GetNews();

            var news = newses.Where(n => n.newsId == newsId);
            var newsDto = _mapper.Map<News>(news);
            newsDto.views += 1;

            await _newsRepository.UpdateNews(newsDto);

            return newsDto;
        }
        public async Task<bool> CreateNews(CreateNewsDto createNews)
        {
            var newsGet = await _newsRepository.GetNews();

            var existingNews = newsGet
                .FirstOrDefault(n => n.title.Trim().ToUpper() == newsGet.Select(n => n.title.FirstOrDefault()));

            if (existingNews != null)
            {
                throw new ArgumentException(" ", "News Already Exists");
            }

            var news = newsGet
              .Where(a => a.title.Trim().ToUpper() == createNews.title.ToUpper()
              && a.description.Trim().ToUpper() == createNews.description.ToUpper()
              ).FirstOrDefault();

          
            var newsMap = _mapper.Map<News>(createNews);

            if (createNews.imagesId?.Any() == true)
            {
                newsMap.NewsUploads = createNews.imagesId
                    .Select(u => new NewsUploads { fileId = u, newsId = createNews.newsId })
                    .ToList();
            }

            if (!await _newsRepository.CreateNews(newsMap))
            {
                throw new ArgumentException(" ", "что то пошло не так при сохранении новости в базе данных");
            }

            return true;
        }

        public async Task<bool> UpdateNews(string newsId, UpdateNewsDto updateNews)
        {
            await _helper.UpdateEntity<News, UpdateNewsDto>(newsId, updateNews);

            if (updateNews.imagesId != null && updateNews.imagesId.Any())
            {
                foreach (var uploadId in updateNews.imagesId)
                {

                    var newsUpload = await _context.NewsUploads.FirstOrDefaultAsync(up => up.newsId == newsId && up.fileId == uploadId);
                    if (newsUpload != null)
                    {
                        _context.NewsUploads.Remove(newsUpload);
                        await _fileRepository.DeleteFile(newsId, uploadId);
                    }
                }
                _context.NewsUploads.RemoveRange(_context.NewsUploads.Where(up => up.newsId == newsId));
                foreach (var uploadId in updateNews.imagesId)
                {
                    _context.NewsUploads.Add(new NewsUploads
                    {
                        newsId = newsId,
                        fileId = uploadId
                    });
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteNews(string newsId)
        {
            var newsToDelete = await _newsRepository.GetNewsById(newsId);

            if (newsToDelete == null)
                throw new ArgumentException();

            await _cascade.CascadeDeletedNewsContext(newsId);

            if (!await _newsRepository.DeleteNews(newsToDelete))
            {
                throw new ArgumentException("Ошибка удаления новости");
            }

            return true;
        }
    }
}
