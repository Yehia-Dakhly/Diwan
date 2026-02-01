using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diwan.DAL.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; } = null!;
        public int PostId { get; set; }
        public required string UserId { get; set; }
        [ForeignKey("UserId")]
        public DiwanUser User { get; set; } = null!;
        public int? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public Comment? ParentComment { get; set; }
    }
}
