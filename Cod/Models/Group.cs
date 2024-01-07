using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace Cod.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Introduceti numele grupuului!")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreationDate { get; set; }
        public virtual ICollection<ProfileGroup>? Profiles { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
    }
}
