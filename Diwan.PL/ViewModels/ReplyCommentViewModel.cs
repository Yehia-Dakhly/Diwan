using System.ComponentModel.DataAnnotations;

namespace Diwan.PL.ViewModels
{
    public class ReplyCommentViewModel
    {
        [Required]
        public string Content { get; set; }
        public int PostId { get; set; }
        public int ParentId { get; set; }
    }
}
