using AutoMapper;
using Diwan.DAL.Models;
using Diwan.PL.ViewModels;

namespace Diwan.PL.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<DiwanUser, DiwanUserViewModel>().ReverseMap();
            CreateMap<DiwanUser, EditProfileViewModel>().ReverseMap();
        }
    }
}
