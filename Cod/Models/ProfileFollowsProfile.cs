using System.ComponentModel.DataAnnotations.Schema;

namespace Cod.Models
{
    public class ProfileFollowsProfile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? FollowingProfileId { get; set; }
        public string? FollowedProfileId { get; set; }

        public virtual ApplicationUser? FollowingProfile { get; set; }
        public virtual ApplicationUser? FollowedProfile { get; set; }
    }
}
