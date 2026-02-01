using Diwan.DAL.Enums;

namespace Diwan.PL.ViewModels
{
    public class EditProfileViewModel
    {
        public string Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public IFormFile? Picture { get; set; }
        public IFormFile? CoverPic { get; set; }
        public string? Bio { get; set; }
    }
}
