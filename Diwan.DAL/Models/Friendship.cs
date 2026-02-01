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
    public class Friendship
    {
        [Key]
        public int Id { get; set; }
        public FriendRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string RequesterId { get; set; }
        [ForeignKey("RequesterId")]
        public DiwanUser Requester { get; set; } = null!;

        public required string AddresseeId { get; set; }
        [ForeignKey("AddresseeId")]
        public DiwanUser Addressee { get; set; } = null!;
    }
}
