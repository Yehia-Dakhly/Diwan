using Diwan.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diwan.DAL.Models
{
    public class DiwanUser : IdentityUser
    {
        [Required]
        [MaxLength(30)]
        public required string FirstName { get; set; }
        [Required]
        [MaxLength(30)]
        public required string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? PictureURL { get; set; }
        public string? CoverPicURL { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsAgree { get; set; }
        public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
        public ICollection<Notification> ReceivedNotifications { get; set; } = new HashSet<Notification>();
        public ICollection<Notification> CausedNotifications { get; set; } = new HashSet<Notification>(); 
        public ICollection<Friendship> SentFriendRequests { get; set; } = new HashSet<Friendship>();
        public ICollection<Friendship> ReceivedFriendRequests { get; set; } = new HashSet<Friendship>();
        public ICollection<Reaction> Reactions { get; set; } = new HashSet<Reaction>();
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();



    }
}
