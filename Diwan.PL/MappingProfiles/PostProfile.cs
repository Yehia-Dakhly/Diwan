using AutoMapper;
using Diwan.DAL.Models;
using Diwan.PL.ViewModels;

namespace Diwan.PL.MappingProfiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Post, PostViewModel>()
                .ForMember(P => P.UserPictureURL, O => O.MapFrom(P => P.Author.PictureURL))
                .ForMember(P => P.FullName, O => O.MapFrom(P => $"{P.Author.FirstName} {P.Author.LastName}"))
                .ForMember(P => P.CommentCount, O => O.MapFrom(P => P.Comments.Count))
                .ForMember(P => P.ReactionsCount, O => O.MapFrom(P => P.Reactions.Count))
                .ForMember(P => P.CountByType, O => O
                        .MapFrom(P => P.Reactions
                        .GroupBy(P => P.ReactionType).ToDictionary(G => G.Key, G => G.Count())))
                .ReverseMap();

            CreateMap<CreatePostViewModel, Post>();

        }
    }
}
