using AutoMapper;
using Diwan.DAL.Models;
using Diwan.PL.ViewModels;

namespace Diwan.PL.MappingProfiles
{
    public class FriendshipProfile : Profile
    {
        public FriendshipProfile()
        {
            CreateMap<Friendship, FriendshipViewModel>().ForMember(F => F.RequesterPictureUrl, O => O.MapFrom(DF => DF.Requester.PictureURL))
                .ForMember(F => F.RequesterName, O => O.MapFrom(DF => $"{ DF.Requester.FirstName} {DF.Requester.LastName}"));
        }
    }
}
