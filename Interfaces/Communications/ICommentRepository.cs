namespace Profunion.Interfaces.Communications
{
    public interface ICommentRepository
    {
        Task<ICollection<Comments>> GetComments();
        Task<bool> SendComment(Comments comment);
        Task<bool> DeleteComment(Comments comment);

        Task<bool> SaveComments();
    }
}
