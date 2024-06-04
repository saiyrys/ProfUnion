namespace Profunion.Models.CommunicationsModels
{
    public class Report
    {
        public int Id { get; }
        public string content { get; set; }
        public string userID { get; set; }
        [ForeignKey("UserID")]
        public virtual User User { get; set; }
    }
}
