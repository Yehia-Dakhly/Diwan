using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diwan.DAL.Enums
{
    public enum NotificationType
    {
        None = 0,
        NewFriendRequest = 1,
        FriendRequestAccepted = 2,
        NewPostReaction = 3,
        NewPostComment = 4,
        NewCommentReply = 5,
        NewSharedPost = 6
    }
}
