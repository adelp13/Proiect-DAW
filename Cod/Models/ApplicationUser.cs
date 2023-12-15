using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cod.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public bool isPrivate { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
        // TODO: create self M-M tables
        //public virtual ICollection<Profile> Follows { get; set; }
        //public virtual ICollection<Profile> Follow_Requests { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        [Required(ErrorMessage = "First Name Required")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Last Name Required")]
        public string? LastName { get; set; }
    }
}
