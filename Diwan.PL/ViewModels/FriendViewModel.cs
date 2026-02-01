using Diwan.DAL.Enums;

namespace Diwan.PL.ViewModels
{
    public class FriendViewModel
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public Gender? Gender { get; set; }
        public string? PictureURL { get; set; }

    }
}
