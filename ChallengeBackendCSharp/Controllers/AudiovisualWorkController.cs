using AutoMapper;
using ChallengeBackendCSharp.Entities;
using ChallengeBackendCSharp.Models;
using ChallengeBackendCSharp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChallengeBackendCSharp.Controllers
{
    [Authorize]
    [Route("api/movies")]
    [ApiController]
    public class AudiovisualWorkController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly DatabaseConnector _db;

        public AudiovisualWorkController(IMapper mapper, DatabaseConnector db)
        {
            _mapper = mapper;
            _db = db;
        }

        // CRUD

        /// <summary>
        ///     Obtiene todas las obras audiovisuales de la base de datos, y también permite filtrar mediante parámetros query.
        /// </summary>
        /// <param name="queryDto">Se pueden pasar como parámetros query "name", "genre" y "order".</param>
        /// <returns>Una lista con todos las obras audiovisuales de la base de datos, o los filtrados por los parámetros query.</returns>
        [HttpGet]
        public async Task<ActionResult> GetAllAudiovisualWorks([FromQuery] AudiovisualWorkQueryDto queryDto)
        {
            try
            {
                if (queryDto.GetState())
                {
                    var list = await _db.AudiovisualWorks!.Select(sel => new { sel.Image, sel.Title, ReleaseData = sel.ReleaseDate.ToString("yyyy") }).ToListAsync();

                    return Ok(list is null | list!.Count == 0 ? throw new Exception("No hay registros en la base de datos de películas.") : list);
                }
                else if (queryDto.GenreID is null)
                {
                    var filteredList = _db.AudiovisualWorks!.Include(inc => inc.GenreAudiovisualWorks)!
                                            .ThenInclude(inc => inc.Genre)
                                            .Include(inc => inc.CharacterAudiovisualWorks)!
                                            .ThenInclude(inc => inc.Character)
                                            .Where(sel => (sel.Title == queryDto.Name || queryDto.Name == null));

                    switch (queryDto.Order)
                    {
                        case Order.ASC:
                            filteredList = filteredList.OrderBy(ord => ord.Title);
                            break;
                        case Order.DESC:
                            filteredList = filteredList.OrderByDescending(ord => ord.Title);
                            break;
                        default:
                            break;
                    }

                    var filterListDto = _mapper.Map<IQueryable<AudiovisualWork>, IEnumerable<AudiovisualWorkWithCharactersAndGenresDto>>(filteredList);

                    return Ok(filterListDto is null | !(filterListDto!).Any() ? throw new Exception("No hay registros en la base de datos de películas que coincidan con el nombre recibido.") : filterListDto);
                }
                else
                {
                    var filteredList = await _db.Genres!.Include(inc => inc.GenreAudiovisualWorks)!.ThenInclude(tinc => tinc.AudiovisualWork).FirstOrDefaultAsync(frs => frs.GenreID == queryDto.GenreID);
                    var genreDto = _mapper.Map<GenreWithAudiovisualWorksDto>(filteredList);

                    return Ok(genreDto is null ? throw new Exception("El genero indicado no contiene obras audiovisuales asociadas.") : genreDto);
                }

            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        /// <summary>
        ///     Obtiene una obra audiovisual mediante su ID.
        /// </summary>
        /// <param name="id">Numero entero perteneciente al identificador de la entidad audiovisualwork en la base de datos.</param>
        /// <returns>200: Si el id se encuentra registrado con una entidad, 404: si se produce algún tipo de excepción.</returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetAudiovisualWorksById(int id)
        {
            try
            {
                var audiovisualWork = await _db.AudiovisualWorks!.Include(inc => inc.CharacterAudiovisualWorks)!
                                            .ThenInclude(tinc => tinc.Character)
                                            .Include(inc => inc.GenreAudiovisualWorks)!
                                            .ThenInclude(tinc => tinc.Genre)
                                            .FirstOrDefaultAsync(frs => frs.AudiovisualWorkID == id);

                if (audiovisualWork is null) { throw new Exception("No se encontró ningún registro con ese id."); };

                var audiovisualWorkDto = _mapper.Map<AudiovisualWorkWithCharactersAndGenresDto>(audiovisualWork);

                return Ok(audiovisualWorkDto);
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        /// <summary>
        ///      Añade una nueva obra audiovisual a la base de datos.
        /// </summary>
        /// <param name="audiovisualWorkDto">Objeto audiovisualwork que cuenta con todas las propiedades que pertenecen a la entidad.</param>
        /// <returns>200: Si la entidad es registrada correctamente en la base de datos, 404: si se produce algún tipo de excepción.</returns>
        [HttpPost]
        public async Task<ActionResult> PostAudiovisualWorks([FromBody] AudiovisualWorkDto audiovisualWorkDto)
        {
            try
            {
                var newAudiovisualWork = _mapper.Map<AudiovisualWork>(audiovisualWorkDto);

                await _db.AudiovisualWorks!.AddAsync(newAudiovisualWork);
                await _db.SaveChangesAsync();

                return Ok(new { Message = string.Format("La obra '{0}' fue agregada correctamente a la base de datos.", newAudiovisualWork.Title) });
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        /// <summary>
        ///     Actualiza una obra audiovisual ya existente en la base de datos.
        /// </summary>
        /// <param name="id">Numero entero perteneciente al Identificador de la entidad en la base de datos.</param>
        /// <param name="character">Objeto audiovisualwork que cuenta con todas las propiedades que pertenecen a la entidad.</param>
        /// <returns>200: Si la entidad es actualizada correctamente en la base de datos, 404: si se produce algún tipo de excepción.</returns>
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutAudiovisualWorks(int id, [FromBody]AudiovisualWorkWithIdDto audiovisualWorkWithIdDto)
        {
            try
            {
                if (audiovisualWorkWithIdDto.AudiovisualWorkID != id) { throw new Exception("El Id de la url y el cuerpo de la solicitud no coinciden."); };

                var updatedAudiovisual = _mapper.Map<AudiovisualWork>(audiovisualWorkWithIdDto);

                _db.Entry(updatedAudiovisual).State = EntityState.Modified;

                await _db.SaveChangesAsync();

                return Ok(new { Message = string.Format("La obra '{0}' fue actualizado correctamente.", audiovisualWorkWithIdDto.Title) });
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        /// <summary>
        ///      Asocia películas ya existentes a un genero ya existente.
        /// </summary>
        /// <param name="id">Numero entero perteneciente al identificador de la entidad audiovisualwork en la base de datos.</param>
        /// <param name="idMovie">Numero entero perteneciente al identificador de la entidad genre en la base de datos.</param>
        /// <returns></returns>
        [HttpPut("{id:int}/genres/{idGenre:int}")]
        public async Task<ActionResult> PutAudiovisualWorksGenre(int id, int idGenre)
        {
            try
            {
                var audiovisualwork = await _db.AudiovisualWorks!.Include(x => x.GenreAudiovisualWorks).FirstOrDefaultAsync(frs => frs.AudiovisualWorkID == id);

                var genre = await _db.Genres!.FindAsync(idGenre);

                if (audiovisualwork is null || genre is null) { throw new Exception(string.Format("No se encontró ningún registro en la tabla {0}{1}{2} con el id señalado.", 
                    (audiovisualwork is null) ? "Audiovisualworks" : "",
                    (audiovisualwork is null && genre is null) ? " y " : "",
                    (genre is null) ? "Genres" : "" )); };

                audiovisualwork.GenreAudiovisualWorks!.Add(new GenreAudiovisualWork { GenreID = genre.GenreID, Genre = genre });

                await _db.SaveChangesAsync();

                return Ok(new { Message = string.Format("La obra '{0}' fue actualizado correctamente asociando el genero '{1}' a ella.", audiovisualwork.Title, genre.Name) });
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        /// <summary>
        ///      Elimina una obra audiovisual de la base de datos mediante su Id.
        /// </summary>
        /// <param name="id">Numero entero perteneciente al identificador de la entidad en la base de datos.</param>
        /// <returns>200: Si la entidad es eliminada correctamente en la base de datos, 404: si se produce algún tipo de excepción.</returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAudiovisualWork(int id)
        {
            try
            {
                var audiovisualWork = await _db.AudiovisualWorks!.FindAsync(id);

                if (audiovisualWork is null) { throw new Exception("No se encontró ningún registro con ese id."); };

                _db.AudiovisualWorks.Remove(audiovisualWork);

                await _db.SaveChangesAsync();

                return Ok(new { Message = string.Format("La obra '{0}' fue eliminado correctamente de la base de datos.", audiovisualWork.Title) });
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }
    }
}
