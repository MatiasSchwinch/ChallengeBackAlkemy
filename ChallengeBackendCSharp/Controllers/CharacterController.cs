using AutoMapper;
using ChallengeBackendCSharp.Entities;
using ChallengeBackendCSharp.Models;
using ChallengeBackendCSharp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChallengeBackendCSharp.Controllers
{
    [Authorize]
    [Route("api/characters")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly ILogger<CharacterController> _logger;
        private readonly IMapper _mapper;
        private readonly DatabaseConnector _db;
        private readonly Dictionary<string, string> _validQueryParameters = new Dictionary<string, string>()
        {
            { "age", "Age" },
            { "name", "Name" },
            { "movies", "AudiovisualWorks" }
        };

        public CharacterController(ILogger<CharacterController> logger, IMapper mapper, DatabaseConnector db)
        {
            _logger = logger;
            _mapper = mapper;
            _db = db;
        }

        // CRUD

        // Obtener todos los personajes de la base de datos, y también permite filtrar mediante parámetros query.
        [HttpGet]
        public async Task<ActionResult> GetAllCharacters([FromQuery] CharacterQueryDto queryDto)
        {
            try
            {
                if (queryDto.GetState())
                {
                    var list = await _db.Characters!.Select(sel => new { sel.Image, sel.Name }).ToListAsync();

                    return Ok(list is null | list!.Count == 0 ? throw new Exception("No hay registros en la base de datos de personajes.") : list);
                }
                else if (queryDto.Movie is null)
                {
                    var filteredList = await _db.Characters!.Where(filt => (filt.Name == queryDto.Name || queryDto.Name == null) && (filt.Age == queryDto.Age || queryDto.Age == null)).ToListAsync();

                    return Ok(filteredList is null | filteredList!.Count == 0 ? throw new Exception("No hay registros en la base de datos de personajes que coincidan con los parámetros recibidos.") : filteredList);
                }
                else
                {
                    var filteredList = _db.AudiovisualWorks!.Include(inc => inc.CharacterAudiovisualWorks)!.ThenInclude(tinc => tinc.Character).FirstOrDefault(frs => frs.AudiovisualWorkID == queryDto.Movie);
                    var moviesDto = _mapper.Map<AudiovisualWorkWithCharactersDto>(filteredList);

                    return Ok(moviesDto is null ? throw new Exception("La obra audiovisual no tiene personajes asociados.") : moviesDto);
                } 
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        // Obtiene un personaje mediante su ID.
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetCharacterById(int id)
        {
            try
            {
                var character = await _db.Characters!.Include(incl => incl.CharacterAudiovisualWorks)!.ThenInclude(tinc => tinc.AudiovisualWork).FirstOrDefaultAsync(iden => iden.CharacterID == id);

                if (character is null) { throw new Exception("No se encontró ningún registro con ese id."); };

                var showcharacter = _mapper.Map<CharacterWithMoviesDto>(character);

                return Ok(showcharacter);
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        // Añadir nuevo personaje.
        [HttpPost]
        public async Task<ActionResult> AddCharacter([FromBody] CharacterDto character)
        {
            try
            {
                var newCharacter = _mapper.Map<Character>(character);

                await _db.Characters!.AddAsync(newCharacter);
                await _db.SaveChangesAsync();

                return Ok(new { Message = string.Format("El personaje '{0}' fue agregado correctamente a la base de datos.", newCharacter.Name) });
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        // Eliminar personaje existente.
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteCharacter(int id)
        {
            try
            {
                var character = await _db.Characters!.FindAsync(id);

                if (character is null) { throw new Exception("No se encontró ningún registro con ese id."); };

                _db.Characters.Remove(character);

                await _db.SaveChangesAsync();

                return Ok(new { Message = string.Format("El personaje '{0}' fue eliminado correctamente de la base de datos.", character.Name) });
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        // Actualizar personaje existente.
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateCharacter(int id, [FromBody] CharacterWithIdDto character)
        {
            try
            {
                if (character.CharacterID != id) { throw new Exception("El Id de la url y el cuerpo de la solicitud no coinciden."); };

                var updatedCharacter = _mapper.Map<Character>(character);

                _db.Entry(updatedCharacter).State = EntityState.Modified;

                await _db.SaveChangesAsync();

                return Ok(new { Message = string.Format("El personaje '{0}' fue actualizado correctamente.", character.Name) });
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        // Asocia películas a un personaje ya existente.
        [HttpPut("{id:int}/audiovisualworks/{idMovie:int}")]
        public async Task<ActionResult> UpdateCharacterAudiovisualWorks(int id, int idMovie)
        {
            try
            {
                var character = await _db.Characters!.Include(inc => inc.CharacterAudiovisualWorks).FirstOrDefaultAsync(frs => frs.CharacterID == id);

                var audiovisualWork = await _db.AudiovisualWorks!.FindAsync(idMovie);

                if (character is null) { throw new Exception("No se encontró ningún registro con ese id."); };

                character.CharacterAudiovisualWorks!.Add(new CharacterAudiovisualWork { AudiovisualWork = audiovisualWork });

                await _db.SaveChangesAsync();

                return Ok(new { Message = string.Format("El personaje '{0}' fue actualizado agregando una nueva obra audiovisual correctamente.", character.Name) });
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }
    }
}
