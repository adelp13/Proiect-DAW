using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cod.Models
{
    public class Profile : IdentityUser
    {
        public bool isPrivate { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
        // TODO: create self M-M tables
        //public virtual ICollection<Profile> Follows { get; set; }
        //public virtual ICollection<Profile> Follow_Requests { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
