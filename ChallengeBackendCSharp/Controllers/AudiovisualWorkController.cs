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

        // Obtiene todas las obras audiovisuales de la base de datos, y también permite filtrar mediante parámetros query.
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

        // Obtiene una obra audiovisual mediante su ID.
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

        // Añadir una nueva obra audiovisual.
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

        // Elimina una obra audiovisual existente.
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
