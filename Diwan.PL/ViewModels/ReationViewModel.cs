using Diwan.DAL.Enums;
using Diwan.DAL.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diwan.PL.ViewModels
{
    public class ReationViewModel
    {
        public ReactionType ReactionType { get; set; }
        public required int PostId { get; set; }
    }
}
