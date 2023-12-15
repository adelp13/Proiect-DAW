using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cod.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string ProfileId { get; set; }
        public virtual ApplicationUser? Profile { get; set; }
        [Required(ErrorMessage = "Comment Content Required")]
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
        public int? PostId { get; set; }
    }
}
