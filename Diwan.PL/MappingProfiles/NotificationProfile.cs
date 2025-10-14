using AutoMapper;
using Diwan.DAL.Models;
using Diwan.PL.ViewModels;

namespace Diwan.PL.MappingProfiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationViewModel>()
                .ForMember(N => N.PictureURL, O=>O.MapFrom(N =>N.ActorUser.PictureURL));
        }
    }
}
