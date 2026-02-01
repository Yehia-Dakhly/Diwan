using Diwan.DAL.Enums;
using Diwan.DAL.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diwan.PL.ViewModels
{
    public class CreatePostViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Content is Required!")]
        public required string Content { get; set; }
        public string? PictureURL { get; set; }
        public IFormFile? Picture { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Visibility Visibility { get; set; }
        public string? Tag { get; set; }
        public required string AuthorId { get; set; }
    }
}
