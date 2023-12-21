using System.ComponentModel.DataAnnotations.Schema;

namespace Cod.Models
{
    public class ProfileFollowsProfile
    {
        public string? FollowingProfileId { get; set; }
        public string? FollowedProfileId { get; set; }
    }
}
