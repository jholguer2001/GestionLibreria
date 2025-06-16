using LibreriaApi.Models.DTOs;
using LibreriaApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LibreriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutoresController : ControllerBase
    {
        private readonly IAutorService _autorService;

        public AutoresController(IAutorService autorService)
        {
            _autorService = autorService;
        }

   
        [HttpGet]
        public async Task<ActionResult<List<AutorDTO>>> GetAutores()
        {
            try
            {
                var autores = await _autorService.ObtenerAutoresAsync();
                return Ok(autores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<AutorDTO>> GetAutor(int id)
        {
            try
            {
                var autor = await _autorService.ObtenerAutorPorIdAsync(id);
                if (autor == null)
                {
                    return NotFound($"No se encontró el autor con ID {id}");
                }
                return Ok(autor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un nuevo autor
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AutorDTO>> PostAutor(AutorDTO autorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var nuevoAutor = await _autorService.CrearAutorAsync(autorDto);
                return CreatedAtAction(nameof(GetAutor), new { id = nuevoAutor.Id }, nuevoAutor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

       
        [HttpPut("{id}")]
        public async Task<ActionResult<AutorDTO>> PutAutor(int id, AutorDTO autorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var autorActualizado = await _autorService.ActualizarAutorAsync(id, autorDto);
                if (autorActualizado == null)
                {
                    return NotFound($"No se encontró el autor con ID {id}");
                }

                return Ok(autorActualizado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

      
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAutor(int id)
        {
            try
            {
                var eliminado = await _autorService.EliminarAutorAsync(id);
                if (!eliminado)
                {
                    return NotFound($"No se encontró el autor con ID {id}");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
