namespace Profunion.Models.CommunicationsModels
{
    public class Comments
    {
        public Comments()
        {
            createdAt = DateTime.Now;
        }
        public int Id { get; set; }
        public string userId { get; set; }
        public string content { get; set; }
        public DateTime createdAt { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

    }
}
