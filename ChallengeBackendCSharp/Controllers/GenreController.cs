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
    [Route("api/genres")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly DatabaseConnector _db;

        public GenreController(IMapper mapper, DatabaseConnector db)
        {
            _mapper = mapper;
            _db = db;
        }

        // CRUD

        /// <summary>
        ///     Obtiene todos los géneros de la tabla Genres en la base de datos.
        /// </summary>
        /// <returns>200: Una lista con todos los géneros de la base de datos, 404: si se produce algún tipo de excepción.</returns>
        [HttpGet]
        public async Task<ActionResult> GetAllGenres()
        {
            try
            {
                var allGenres = await _db.Genres!.Select(sel => new { sel.GenreID, sel.Image, sel.Name }).ToListAsync();

                return Ok(allGenres is null | !allGenres!.Any() ? throw new Exception("No hay registros en la tabla Genres de la base de datos.") : allGenres);
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        /// <summary>
        ///     Añade un nuevo genero a la tabla Genres en la base de datos.
        /// </summary>
        /// <param name="genreDto">Objeto genreDto que cuenta con todas las propiedades que pertenecen a la entidad.</param>
        /// <returns>200: Si la entidad es registrada correctamente en la base de datos, 404: si se produce algún tipo de excepción.</returns>
        [HttpPost]
        public async Task<ActionResult> PostGenre([FromBody] GenreDto genreDto)
        {
            try
            {
                var newGenre = _mapper.Map<Genre>(genreDto);

                await _db.Genres!.AddAsync(newGenre);

                await _db.SaveChangesAsync();

                return Ok(new { Message = string.Format("El genero '{0}' fue agregado correctamente a la base de datos.", genreDto.Name) });
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        /// <summary>
        ///     Actualiza un genero ya existente en la tabla Genres de la base de datos.
        /// </summary>
        /// <param name="id">Numero entero perteneciente al identificador de la entidad genre en la base de datos.</param>
        /// <param name="genreDto">Objeto genreWithIdDto que cuenta con todas las propiedades que pertenecen a la entidad.</param>
        /// <returns>200: Si la entidad es actualizada correctamente en la base de datos, 404: si se produce algún tipo de excepción.</returns>
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutGenre(int id, [FromBody] GenreWithIdDto genreDto)
        {
            try
            {
                if (genreDto.GenreID != id) { throw new Exception("El Id de la url y el cuerpo de la solicitud no coinciden."); };

                var updatedGenre = _mapper.Map<Genre>(genreDto);

                _db.Entry(updatedGenre).State = EntityState.Modified;

                await _db.SaveChangesAsync();

                return Ok(new { Message = string.Format("El genero '{0}' fue actualizado correctamente.", genreDto.Name) });
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }

        /// <summary>
        ///     Elimina un genero ya existente mediante su Id.
        /// </summary>
        /// <param name="id">Numero entero perteneciente al identificador de la entidad en la base de datos.</param>
        /// <returns>200: Si la entidad es eliminada correctamente en la base de datos, 404: si se produce algún tipo de excepción.</returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteGenre(int id)
        {
            try
            {
                var genre = await _db.Genres!.FindAsync(id);

                if (genre is null) { throw new Exception("No se encontró ningún registro con ese id."); };

                _db.Genres.Remove(genre);

                await _db.SaveChangesAsync();

                return Ok(new { Message = string.Format("El genero '{0}' fue eliminado correctamente de la base de datos.", genre.Name) });
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }
    }
}
