using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace Cod.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name required!")]
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public DateTime CreationDate { get; set; }
        public virtual ICollection<ApplicationUser> Profiles { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
