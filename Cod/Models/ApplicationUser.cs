using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cod.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public bool isPrivate { get; set; }
        [Required(ErrorMessage = "First Name Required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name Required")]
        public string LastName { get; set; }

        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }

        public virtual ICollection<ProfileFollowsProfile>? Follows { get; set; }
        public virtual ICollection<ProfileRequestsProfile>? Requests { get; set; }
    }
}
