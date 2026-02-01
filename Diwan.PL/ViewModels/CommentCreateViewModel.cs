using System.ComponentModel.DataAnnotations;

namespace Diwan.PL.ViewModels
{
    public class CommentCreateViewModel
    {
        [Required]
        public string Content { get; set; }
        public int PostId { get; set; }
    }
}
