using System.ComponentModel.DataAnnotations;

namespace Diwan.PL.ViewModels
{
    public class ForgetPassswordViewModel
    {
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
    }
}
