using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cod.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name required!")]
        public string ProfileId { get; set; }
        public ApplicationUser Profile { get; set; }
        public string CommentContent { get; set; }
        public DateTime CommentDate { get; set; }
    }
}
