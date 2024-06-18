namespace Profunion.Interfaces.NewsInterface
{
    public interface INewsService
    {
        Task<(IEnumerable<GetNewsDto> newses, int TotalPage)> GetNewses (int page, string search = null, string sort = null, string type = null);

        Task<News> GetNews(string newsId);

        Task<bool> CreateNews(CreateNewsDto createNews);

        Task<bool> UpdateNews(string newsId, UpdateNewsDto updateNews);

        Task<bool> DeleteNews(string newsId);


    }
}
