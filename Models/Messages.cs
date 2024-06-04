using System.ComponentModel.DataAnnotations.Schema;
using Profunion.Models.UserModels;

namespace Profunion.Models
{
    public class Messages
    {
        public int ID { get; set; }
        public string Texts { get; set; }
        public string InitiatorID { get; set; }
        public string RecipientID { get; set; }

        [ForeignKey("InitiatorID")]
        public User Initiator { get; set; }

        [ForeignKey("RecipientID")]
        public User Recipient { get; set; }
        public byte[] File { get; set; }

    }
}
