using AutoMapper;
using Diwan.DAL.Models;
using Diwan.PL.ViewModels;

namespace Diwan.PL.MappingProfiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentViewModel>()
                .ForMember(C => C.AuthorId, O=>O.MapFrom(C => C.User.Id))
                .ForMember(C => C.AuthorName, O => O.MapFrom(C => $"{C.User.FirstName} {C.User.LastName}"))
                .ForMember(C => C.AuthorPictureUrl, O => O.MapFrom(C => C.User.PictureURL));
        }
    }
}
