using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cod.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public bool isPrivate { get; set; }
        [Required(ErrorMessage = "Trebuie sa introduceti prenumele!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Trebuie sa introduceti numele de familie!")]
        public string LastName { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }

        public virtual ICollection<ProfileFollowsProfile>? Follows { get; set; }
        public virtual ICollection<ProfileRequestsProfile>? Requests { get; set; }
        public virtual ICollection<ProfileGroup>? Groups { get; set; }
    }
}
