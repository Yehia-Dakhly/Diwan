using Diwan.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace Diwan.PL.ViewModels
{
    public class UserProfileViewModel
    {
        public required string Id { get; set; }

        public required string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? PictureURL { get; set; }
        public string? CoverPicURL { get; set; }
        public string? Bio { get; set; }

        public FriendRequestStatus? FriendshipStatus { get; set; }
        public bool IsFriendshipInitiator { get; set; }
        public bool IsCurrentUserProfile { get; set; }
        public ICollection<PostViewModel> Posts { get; set; } = new List<PostViewModel>();
    }
}
