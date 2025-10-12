using Diwan.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diwan.DAL.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Message { get; set; }
        [Required]
        public required string URL { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required]
        public NotificationType NotificationType { get; set; }
        public required string RecipientUserId { get; set; }
        [ForeignKey("RecipientUserId")]
        public DiwanUser RecipientUser { get; set; } = null!;
        public required string ActorUserId { get; set; }
        [ForeignKey("ActorUserId")]
        public DiwanUser ActorUser { get; set; } = null!;
    }
}
