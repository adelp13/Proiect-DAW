using System.ComponentModel.DataAnnotations;

namespace Cod.Models
{
    public class Message
    {
        [Key]
        public int ID { get; set; }
        public DateTime CreationDate { get; set; }
        [Required(ErrorMessage = "Message content required!")]
        public string Content { get; set; }
        public int ProfileID { get; set; }
        public Profile Profile { get; set; }
        public int GroupID { get; set; }
        public Group Group { get; set; }

    }
}
