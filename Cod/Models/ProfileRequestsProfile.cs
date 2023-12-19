using System.ComponentModel.DataAnnotations.Schema;

namespace Cod.Models
{
    public class ProfileRequestsProfile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? RequestingProfileId { get; set; }
        public string? RequestedProfileId { get; set; }
        public virtual ApplicationUser? RequestingProfile { get; set; }
        public virtual ApplicationUser? RequestedProfile { get; set; }
    }
}
