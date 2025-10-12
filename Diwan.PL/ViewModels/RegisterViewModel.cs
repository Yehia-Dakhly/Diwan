using Diwan.DAL.Enums;
using Diwan.PL.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Diwan.PL.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First Name is Required")]
        [MaxLength(30, ErrorMessage ="Max Length is 30 Characters")]
        public string FName { get; set; }
        [Required(ErrorMessage = "Last Name is Required")]
        [MaxLength(30, ErrorMessage = "Max Length is 30 Characters")]
        public string LName { get; set; }
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Comfirm Password is Required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password Doesn't Match")]
        public string ComfirmPassword { get; set; }
        [Required(ErrorMessage = "DateOfBirth Required")]
        [MinAge(12, ErrorMessage ="Age Must be At Least 12 Years old!")]
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string? PictureURL { get; set; }
        public string? CoverURL { get; set; }
        public IFormFile? PictureImage { get; set; }
        public IFormFile? CoverImage { get; set; }
        public string? Bio { get; set; }
        public bool IsAgree { get; set; }
    }
}
