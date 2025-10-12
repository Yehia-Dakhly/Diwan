using Diwan.DAL.Enums;
using Diwan.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace Diwan.PL.ViewModels
{
    public class DiwanUserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? PictureURL { get; set; }
        public string? CoverPicURL { get; set; }
        public string? Bio { get; set; }
        public bool IsFriend { get; set; }
        public FriendRequestStatus? FriendshipStatus { get; set; }
        public bool IsFriendshipInitiator { get; set; }

    }
}
