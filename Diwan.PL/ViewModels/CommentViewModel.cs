namespace Diwan.PL.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PostId { get; set; }
        public required string AuthorId { get; set; }
        public required string AuthorName { get; set; }
        public string? AuthorPictureUrl { get; set; }
        public int? ParentId { get; set; }
        public ICollection<CommentViewModel> Replies { get; set; } = new List<CommentViewModel>();
    }
}
