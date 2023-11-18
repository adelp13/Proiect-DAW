using System.ComponentModel.DataAnnotations;

namespace Cod.Models
{
    public class Group
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Name required!")]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public virtual ICollection<Profile> Profiles { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
