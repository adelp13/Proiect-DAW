using System.ComponentModel.DataAnnotations;

namespace Cod.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        [Required(ErrorMessage = "Post content required!")]
        public string Content { get; set; }
        public string? ProfileId { get; set; }
        public virtual ApplicationUser? Profile { get; set; }
        public int? GroupId { get; set; }
        public virtual Group? Group { get; set; }

    }
}
