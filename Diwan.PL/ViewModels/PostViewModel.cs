using Diwan.DAL.Enums;
using Diwan.DAL.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diwan.PL.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public string? PictureURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Visibility Visibility { get; set; }
        public string? Tag { get; set; }
        public required string AuthorId { get; set; }


        public string? UserPictureURL { get; set; }
        public required string FullName { get; set; }
        public Dictionary<ReactionType, int> CountByType { get; set; } = new Dictionary<ReactionType, int>();
        public int ReactionsCount { get; set; }
        public int CommentCount { get; set; }
    }
}
