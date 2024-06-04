namespace Profunion.Services.CommunicationsServicres
{
    public class Comment : ICommentRepository
    {
        private readonly DataContext _context;
        public Comment(DataContext context)
        {
            _context = context;
        }
        public async Task<ICollection<Comments>> GetComments()
        {
            return await _context.Comment.OrderBy(com => com.Id).ToListAsync();
        }
        public async Task<bool> SendComment(Comments comment)
        {
            _context.Add(comment);

            await _context.SaveChangesAsync();

            return await SaveComments();
        } 
        public async Task<bool> DeleteComment(Comments comment)
        {
            _context.Remove(comment);

            return await SaveComments();
        }
        public async Task<bool> SaveComments()
        {
           var saved = await _context.SaveChangesAsync();

           return saved > 0 ? true : false;
        }
    }
}
