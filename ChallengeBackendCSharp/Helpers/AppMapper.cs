using AutoMapper;
using ChallengeBackendCSharp.Entities;
using ChallengeBackendCSharp.Models;

namespace ChallengeBackendCSharp.Helpers
{
    public class AppMapper : Profile
    {
        public AppMapper()
        {
            // Mapeo de CharacterDto y CharacterWithIdDto a Character (entidad) y viceversa, solo propiedades superficiales.
            CreateMap<CharacterDto, Character>().ReverseMap();
            CreateMap<CharacterWithIdDto, Character>().ReverseMap();

            // Mapeo de AudiovisualWorkDto y AudiovisualWorkWithIdDto a AudiovisualWork (entidad) y viceversa, solo propiedades superficiales.
            CreateMap<AudiovisualWorkDto, AudiovisualWork>().ReverseMap();
            CreateMap<AudiovisualWorkWithIdDto, AudiovisualWork>().ReverseMap();

            // Mapeo de GenreDto y GenreWithIdDto a Genre (entidad) y viceversa, solo propiedades superficiales.
            CreateMap<GenreDto, Genre>().ReverseMap();
            CreateMap<GenreWithIdDto, Genre>().ReverseMap();

            // Mapeo de AudiovisualWorkWithIdDto a CharacterAudiovisualWork (entidad)
            // => propiedad "CharacterAudiovisualWork.AudiovisualWork" se completa con "AudiovisualWorkWithIdDto".
            CreateMap<AudiovisualWorkWithIdDto, CharacterAudiovisualWork>()
                .ForMember(dest => dest.AudiovisualWork, obt => obt.MapFrom(sel => sel));

            // Mapeo de Character (entidad) a CharacterWithMoviesDto
            // => propiedad "CharacterWithMoviesDto.AudiovisualWorks" (Lista de string) se completa tomando solo la propiedad Title de "Character.CharacterAudiovisualWorks.AudiovisualWork.Title".
            CreateMap<Character, CharacterWithMoviesDto>()
                .ForMember(dest => dest.AudiovisualWorks, obt => obt.MapFrom(sel => sel.CharacterAudiovisualWorks!.Select(x => x.AudiovisualWork!.Title)));

            // Mapeo de AudiovisualWork (entidad) a AudiovisualWorkWithCharactersDto
            // => propiedad "AudiovisualWorkWithCharactersDto.Characters" (Lista de string) se completa tomando solo la propiedad Name de "AudiovisualWork.CharacterAudiovisualWorks.Character.Name".
            CreateMap<AudiovisualWork, AudiovisualWorkWithCharactersDto>()
                .ForMember(dest => dest.Characters, obt => obt.MapFrom(sel => sel.CharacterAudiovisualWorks!.Select(x => x.Character!.Name)));

            // Mapeo de AudiovisualWork (entidad) a AudiovisualWorkWithCharactersAndGenresDto
            // => propiedad "AudiovisualWorkWithCharactersAndGenresDto.Characters" (Lista de string) se completa tomando solo la propiedad Name de "AudiovisualWork.CharacterAudiovisualWorks.Character.Name".
            // => propiedad "AudiovisualWorkWithCharactersAndGenresDto.Genres" (Lista de string) se completa tomando solo la propiedad Name de "AudiovisualWork.GenreAudiovisualWorks.Genre.Name".
            CreateMap<AudiovisualWork, AudiovisualWorkWithCharactersAndGenresDto>()
                .ForMember(dest => dest.Characters, obt => obt.MapFrom(sel => sel.CharacterAudiovisualWorks!.Select(x => x.Character!.Name)))
                .ForMember(dest => dest.Genres, obt => obt.MapFrom(sel => sel.GenreAudiovisualWorks!.Select(x => x.Genre!.Name)));

            // Mapeo de Genre (entidad) a GenreWithAudiovisualWorksDto
            // => propiedad "GenreWithAudiovisualWorksDto.AudiovisualWorks" (Lista de string) se completa tomando solo la propiedad Title de "Genre.GenreAudiovisualWorks.AudiovisualWork.Title".
            CreateMap<Genre, GenreWithAudiovisualWorksDto>()
                .ForMember(dest => dest.AudiovisualWorks, obt => obt.MapFrom(sel => sel.GenreAudiovisualWorks!.Select(x => x.AudiovisualWork!.Title)));
        }
    }
}
