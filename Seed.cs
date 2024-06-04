using Profunion.Data;
using Profunion.Models.UserModels;
using System.Text;

namespace Profunion
{
    public class Seed
    {
        private readonly DataContext dataContext;

        public Seed(DataContext Context)
        {
            dataContext = Context;
        }

        public void SeedDataContext()
        {
            if (!dataContext.Users.Any())
            {
                var users = new List<User>();
                {
                    new User()
                    {
                        userName = "Test1",
                
                        password = "1234",
                    };
                }
                dataContext.Users.AddRange(users);
                dataContext.SaveChanges();
            }
        }
    }
}
