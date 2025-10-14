using Diwan.DAL.Enums;
using Diwan.DAL.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diwan.PL.ViewModels
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public required string Message { get; set; }
        public required string URL { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public NotificationType NotificationType { get; set; }
        public string? PictureURL { get; set; }
    }
}
