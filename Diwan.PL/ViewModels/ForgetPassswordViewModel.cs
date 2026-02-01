using System.ComponentModel.DataAnnotations;

namespace Diwan.PL.ViewModels
{
    public class ForgetPassswordViewModel
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "بريد إلكتروني غير صالح")]
        public string Email { get; set; }
    }
}
