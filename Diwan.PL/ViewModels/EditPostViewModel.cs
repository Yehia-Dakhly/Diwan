using Diwan.DAL.Enums;

namespace Diwan.PL.ViewModels
{
    public class EditPostViewModel
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public string? PictureURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Visibility Visibility { get; set; }
        public string? Tag { get; set; }
        public required string AuthorId { get; set; }
        public IFormFile? Picture { get; set; }
    }
}
