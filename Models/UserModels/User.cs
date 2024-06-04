namespace Profunion.Models.UserModels
{
    public class User
    {
        public User() 
        { 
            createdAt = DateTime.Now;
            updatedAt = DateTime.Now;
        }
        public string userId { get; set; }
        public string? userName { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? middleName { get; set; }
        public string email { get; set; }
        public string? password { get; set; }
        public string salt { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string role { get; set; }

/*        public virtual ICollection<Uploads> Uploads { get; set; }*/
/*        public virtual ICollection<EntityUploads> EntityUploads { get; set; }*/

    }
}
