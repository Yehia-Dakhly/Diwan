using Diwan.DAL.Enums;
using Diwan.DAL.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diwan.PL.ViewModels
{
    public class FriendshipViewModel
    {
        public int Id { get; set; }
        public required string RequesterId { get; set; }
        public required string RequesterName { get; set; }
        public string? RequesterPictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
