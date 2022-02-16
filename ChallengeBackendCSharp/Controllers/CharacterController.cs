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
        private readonly IMapper _mapper;
        private readonly DatabaseConnector _db;

        public CharacterController(IMapper mapper, DatabaseConnector db)
        {
            _mapper = mapper;
            _db = db;
        }

        // CRUD

        /// <summary>
        ///     Obtener todos los personajes de la base de datos, y también permite filtrar mediante parámetros query.
        /// </summary>
        /// <param name="queryDto">Se pueden pasar como parámetros query "name", "age" y "idMovie", los primeros 2 pueden ser enviados en conjunto.</param>
        /// <returns>Una lista con todos los personajes de la base de datos, o los filtrados por los parámetros query.</returns>
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

                    // Prueba
                    var filteredListDto = _mapper.Map<IList<Character>, IList<CharacterWithIdDto>>(filteredList);

                    //return Ok(filteredList is null | filteredList!.Count == 0 ? throw new Exception("No hay registros en la base de datos de personajes que coincidan con los parámetros recibidos.") : filteredList);
                    return Ok(filteredListDto is null | !filteredListDto!.Any() ? throw new Exception("No hay registros en la base de datos de personajes que coincidan con los parámetros recibidos.") : filteredListDto);
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

        /// <summary>
        ///     Obtiene un personaje mediante su ID.
        /// </summary>
        /// <param name="id">Numero entero perteneciente al Identificador de la entidad en la base de datos.</param>
        /// <returns>200: Si el id se encuentra registrado con una entidad, 404: si se produce algún tipo de excepción.</returns>
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

        /// <summary>
        ///     Añade un nuevo personaje a la base de datos.
        /// </summary>
        /// <param name="character">Objeto personaje que cuenta con todas las propiedades que pertenecen a la entidad.</param>
        /// <returns>200: Si la entidad es registrada correctamente en la base de datos, 404: si se produce algún tipo de excepción.</returns>
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

        /// <summary>
        ///     Actualiza un personaje ya existente en la base de datos.
        /// </summary>
        /// <param name="id">Numero entero perteneciente al Identificador de la entidad en la base de datos.</param>
        /// <param name="character">Objeto personaje que cuenta con todas las propiedades que pertenecen a la entidad.</param>
        /// <returns>200: Si la entidad es actualizada correctamente en la base de datos, 404: si se produce algún tipo de excepción.</returns>
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

        /// <summary>
        ///      Asocia películas ya existentes a un personaje ya existente.
        /// </summary>
        /// <param name="id">Numero entero perteneciente al identificador de la entidad character en la base de datos.</param>
        /// <param name="idMovie">Numero entero perteneciente al identificador de la entidad audiovisualwork en la base de datos.</param>
        /// <returns></returns>
        [HttpPut("{id:int}/audiovisualworks/{idMovie:int}")]
        public async Task<ActionResult> UpdateCharacterAudiovisualWorks(int id, int idMovie)
        {
            try
            {
                var character = await _db.Characters!.Include(inc => inc.CharacterAudiovisualWorks).FirstOrDefaultAsync(frs => frs.CharacterID == id);

                var audiovisualWork = await _db.AudiovisualWorks!.FindAsync(idMovie);

                if (character is null || audiovisualWork is null) { throw new Exception(string.Format("No se encontró ningún registro en la tabla {0}{1}{2} con el id señalado.",
                    (character is null) ? "Character" : "",
                    (character is null && audiovisualWork is null) ? " y " : "",
                    (audiovisualWork is null) ? "AudiovisualWork" : "")); };

                character.CharacterAudiovisualWorks!.Add(new CharacterAudiovisualWork { AudiovisualWork = audiovisualWork });

                await _db.SaveChangesAsync();

                return Ok(new { Message = string.Format("El personaje '{0}' fue actualizado agregando una nueva obra audiovisual correctamente.", character.Name) });
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        /// <summary>
        ///     Elimina un personaje de la base de datos mediante su Id.
        /// </summary>
        /// <param name="id">Numero entero perteneciente al Identificador de la entidad en la base de datos.</param>
        /// <returns>200: Si la entidad es eliminada correctamente en la base de datos, 404: si se produce algún tipo de excepción.</returns>
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
    }
}
