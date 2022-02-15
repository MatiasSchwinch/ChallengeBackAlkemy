using AutoMapper;
using ChallengeBackendCSharp.Entities;
using ChallengeBackendCSharp.Models;

namespace ChallengeBackendCSharp.Helpers
{
    public class AppMapper : Profile
    {
        public AppMapper()
        {
            CreateMap<CharacterDto, Character>().ReverseMap();

            CreateMap<CharacterWithIdDto, Character>().ReverseMap();

            //CreateMap<Character, CharacterWithIdDto>()
            //    .ForMember(dest => dest.AudiovisualWorks, obt => obt.MapFrom(sel => sel.CharacterAudiovisualWorks!.Select(x => x.AudiovisualWork)));

            CreateMap<AudiovisualWorkWithIdDto, CharacterAudiovisualWork>()
                .ForMember(dest => dest.AudiovisualWork, obt => obt.MapFrom(sel => sel));

            CreateMap<Character, CharacterWithMoviesDto>()
                .ForMember(dest => dest.AudiovisualWorks, obt => obt.MapFrom(sel => sel.CharacterAudiovisualWorks!.Select(x => x.AudiovisualWork!.Title)));

            CreateMap<AudiovisualWorkDto, AudiovisualWork>().ReverseMap();

            CreateMap<AudiovisualWork, AudiovisualWorkWithCharactersDto>()
                .ForMember(dest => dest.Characters, obt => obt.MapFrom(sel => sel.CharacterAudiovisualWorks!.Select(x => x.Character!.Name)));

            CreateMap<AudiovisualWork, AudiovisualWorkWithCharactersAndGenresDto>()
                .ForMember(dest => dest.Characters, obt => obt.MapFrom(sel => sel.CharacterAudiovisualWorks!.Select(x => x.Character!.Name)))
                .ForMember(dest => dest.Genres, obt => obt.MapFrom(sel => sel.GenreAudiovisualWorks!.Select(x => x.Genre!.Name)));

            CreateMap<Genre, GenreWithAudiovisualWorksDto>()
                .ForMember(dest => dest.AudiovisualWorks, obt => obt.MapFrom(sel => sel.GenreAudiovisualWorks!.Select(x => x.AudiovisualWork!.Title)));
        }
    }
}
