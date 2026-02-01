using Diwan.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diwan.DAL.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Content { get; set; }
        public string? PictureURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt {  get; set; }
        public Visibility Visibility { get; set; }
        public string? Tag { get; set; }
        public required string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public DiwanUser Author { get; set; } = null!;
        public ICollection<Reaction> Reactions { get; set; } = new HashSet<Reaction>();
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    }
}
