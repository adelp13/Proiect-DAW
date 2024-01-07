using System.ComponentModel.DataAnnotations.Schema;

namespace Cod.Models
{
    public class ProfileGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? ProfileId { get; set; }
        public int? GroupId { get; set; }

        public virtual ApplicationUser? Profile { get; set; }
        public virtual Group? Group { get; set; }
    }
}
