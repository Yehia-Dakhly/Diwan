using Diwan.DAL.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diwan.DAL.Models
{
    public class Reaction
    {
        [Key]
        public int Id { get; set; }
        public ReactionType ReactionType { get; set; }
        public required string UserId { get; set; }
        [ForeignKey("UserId")]
        public DiwanUser User { get; set; } = null!;
        public required int PostId { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; } = null!;

    }
}
