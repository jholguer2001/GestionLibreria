using LibreriaApi.Models.DTOs;
using LibreriaApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LibreriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrestamosController : Controller
    {
        private readonly IPrestamoService _prestamoService;

        public PrestamosController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }

       
        /// <param name="incluirDevueltos">Indica si se deben incluir los préstamos ya devueltos</param>
        [HttpGet]
        public async Task<ActionResult<List<PrestamoDTO>>> GetPrestamos(bool incluirDevueltos = false)
        {
            try
            {
                var prestamos = await _prestamoService.ObtenerTodosAsync(incluirDevueltos);
                return Ok(prestamos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

   
        [HttpGet("{id}")]
        public async Task<ActionResult<PrestamoDTO>> GetPrestamo(int id)
        {
            try
            {
                var prestamo = await _prestamoService.ObtenerPorIdAsync(id);
                if (prestamo == null)
                {
                    return NotFound($"No se encontró el préstamo con ID {id}");
                }
                return Ok(prestamo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }


        [HttpGet("libro/{libroId}")]
        public async Task<ActionResult<List<PrestamoDTO>>> GetPrestamosPorLibro(int libroId)
        {
            try
            {
                var prestamos = await _prestamoService.ObtenerPorLibroAsync(libroId);
                return Ok(prestamos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

 
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<List<PrestamoDTO>>> GetPrestamosPorEstado(string estado)
        {
            try
            {
                var prestamos = await _prestamoService.ObtenerPorEstadoAsync(estado);
                return Ok(prestamos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("disponible/{libroId}")]
        public async Task<ActionResult<bool>> LibroDisponible(int libroId)
        {
            try
            {
                var disponible = await _prestamoService.LibroDisponibleAsync(libroId);
                return Ok(disponible);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

       
        [HttpPost]
        public async Task<ActionResult<PrestamoDTO>> PostPrestamo(RegistrarPrestamoDTO prestamoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var nuevoPrestamo = await _prestamoService.RegistrarPrestamoAsync(prestamoDto);
                return CreatedAtAction(nameof(GetPrestamo), new { id = nuevoPrestamo.Id }, nuevoPrestamo);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("{id}/devolucion")]
        public async Task<ActionResult> DevolverPrestamo(int id, DevolucionPrestamoDTO devolucionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var devuelto = await _prestamoService.RegistrarDevolucionAsync(id, devolucionDto);
                if (!devuelto)
                {
                    return NotFound($"No se encontró el préstamo con ID {id}");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

       
        [HttpPut("{id}")]
        public async Task<ActionResult<PrestamoDTO>> PutPrestamo(int id, PrestamoDTO prestamoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != prestamoDto.Id)
                {
                    return BadRequest("El ID del préstamo no coincide con el ID de la URL");
                }

                var actualizado = await _prestamoService.ActualizarPrestamoAsync(id, prestamoDto);
                if (!actualizado)
                {
                    return NotFound($"No se encontró el préstamo con ID {id}");
                }

                return Ok(prestamoDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
