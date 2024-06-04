namespace Profunion.Interfaces.NewsInterface
{
    public interface INewsRepository
    {
        Task<ICollection<GetNewsDto>> GetNews();

        Task<News> GetNewsById(string newsId);

        Task<ICollection<GetNewsDto>> SearchAndSort(string search = null, string sort = null, string type = null);
        Task<bool> CreateNews(News news);
        Task<bool> UpdateNews(News news);
        Task<bool> DeleteNews(News news);

        Task<bool> SaveNews();
    }
}
