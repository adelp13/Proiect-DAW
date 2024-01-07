using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cod.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        [Required(ErrorMessage = "Introduceti continutul postarii!")]
        public string Content { get; set; }
        public string? ProfileId { get; set; }
        // TODO vreau sa stiu care user a creat grupul?
        public virtual ApplicationUser? Profile { get; set; }
        [Required(ErrorMessage = "Grupul in care se face postarea trebuie selectat")]
        public int GroupId { get; set; }
        public virtual Group? Group { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? Groups { get; set; }
    }
}
