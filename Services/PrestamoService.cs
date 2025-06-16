using LibreriaApi.Models.DTOs;
using LibreriaApi.Models.Entities;
using LibreriaApi.Repositories.Interfaces;
using LibreriaApi.Services.Interfaces;

namespace LibreriaApi.Services
{
    public class PrestamoService : IPrestamoService
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly ILibroRepository _libroRepository;
        private readonly ILibroService _libroService;
   public PrestamoService(
            IPrestamoRepository prestamoRepository,
            ILibroRepository libroRepository, ILibroService libroService)
        {
            _prestamoRepository = prestamoRepository;
            _libroRepository = libroRepository;
            _libroService = libroService;
        }

        public async Task<List<PrestamoDTO>> ObtenerTodosAsync(bool incluirDevueltos = false)
        {
            var prestamos = await _prestamoRepository.ObtenerPrestamosAsync(incluirDevueltos);
            return prestamos.Select(ConvertirAPrestamoDTO).ToList();
        }

        public async Task<PrestamoDTO?> ObtenerPorIdAsync(int id)
        {
            var prestamo = await _prestamoRepository.ObtenerPrestamoPorIdAsync(id);
            return prestamo == null ? null : ConvertirAPrestamoDTO(prestamo);
        }

        public async Task<List<PrestamoDTO>> ObtenerPorLibroAsync(int libroId)
        {
            var prestamos = await _prestamoRepository.ObtenerPrestamosPorLibroAsync(libroId);
            return prestamos.Select(ConvertirAPrestamoDTO).ToList();
        }

        public async Task<List<PrestamoDTO>> ObtenerPorEstadoAsync(string estado)
        {
            var prestamos = await _prestamoRepository.ObtenerPrestamosPorEstadoAsync(estado);
            return prestamos.Select(ConvertirAPrestamoDTO).ToList();
        }

        public async Task<PrestamoDTO> RegistrarPrestamoAsync(RegistrarPrestamoDTO prestamoDto)
        {
           
            var libro = await _libroRepository.ObtenerLibroPorIdAsync(prestamoDto.LibroId);
            if (libro == null || !libro.Activo)
            {
                throw new InvalidOperationException("El libro no existe o no está disponible");
            }

            
            if (await _prestamoRepository.ExistePrestamoActivoAsync(prestamoDto.LibroId))
            {
                throw new InvalidOperationException("El libro ya está prestado actualmente");
            }

            var prestamo = new Prestamo
            {
                LibroId = prestamoDto.LibroId,
                FechaDevolucionPrevista = DateTime.Now.AddDays(prestamoDto.DiasPrestamo),
                PrestadoA = prestamoDto.PrestadoA,
                Comentarios = prestamoDto.Comentarios
            };

            var disponible = await _libroService.LibroDisponibleParaPrestamoAsync(prestamoDto.LibroId);
            if (!disponible)
            {
                throw new InvalidOperationException("El libro no está disponible para préstamo");
            }


            var id = await _prestamoRepository.RegistrarPrestamoAsync(prestamo);

           
            var prestamoCreado = await _prestamoRepository.ObtenerPrestamoPorIdAsync(id);
            if (prestamoCreado == null)
            {
                throw new Exception("Error al recuperar el préstamo recién creado");
            }

            return ConvertirAPrestamoDTO(prestamoCreado);
        }

        public async Task<bool> RegistrarDevolucionAsync(int prestamoId, DevolucionPrestamoDTO devolucionDto)
        {
            var prestamo = await _prestamoRepository.ObtenerPrestamoPorIdAsync(prestamoId);
            if (prestamo == null)
            {
                throw new KeyNotFoundException("No se encontró el préstamo especificado");
            }

            if (prestamo.Estado == "Devuelto")
            {
                throw new InvalidOperationException("El libro ya ha sido devuelto anteriormente");
            }

            
            if (!string.IsNullOrWhiteSpace(devolucionDto.Comentarios))
            {
                prestamo.Comentarios = devolucionDto.Comentarios;
                await _prestamoRepository.ActualizarPrestamoAsync(prestamo);
            }

            return await _prestamoRepository.RegistrarDevolucionAsync(prestamoId);
        }

        public async Task<bool> ActualizarPrestamoAsync(int id, PrestamoDTO prestamoDto)
        {
            var prestamoExistente = await _prestamoRepository.ObtenerPrestamoPorIdAsync(id);
            if (prestamoExistente == null)
            {
                throw new KeyNotFoundException("No se encontró el préstamo especificado");
            }

            var prestamo = new Prestamo
            {
                Id = id,
                LibroId = prestamoDto.LibroId,
                FechaPrestamo = prestamoDto.FechaPrestamo,
                FechaDevolucionPrevista = prestamoDto.FechaDevolucionPrevista,
                FechaDevolucionReal = prestamoDto.FechaDevolucionReal,
                Estado = prestamoDto.Estado,
                PrestadoA = prestamoDto.PrestadoA,
                Comentarios = prestamoDto.Comentarios
            };

            return await _prestamoRepository.ActualizarPrestamoAsync(prestamo);
        }

        public async Task<bool> LibroDisponibleAsync(int libroId)
        {
      
            var libro = await _libroRepository.ObtenerLibroPorIdAsync(libroId);
            if (libro == null || !libro.Activo)
            {
                return false;
            }

          
            return !await _prestamoRepository.ExistePrestamoActivoAsync(libroId);
        }



        private static PrestamoDTO ConvertirAPrestamoDTO(Prestamo prestamo)
        {
            return new PrestamoDTO
            {
                Id = prestamo.Id,
                LibroId = prestamo.LibroId,
                FechaPrestamo = prestamo.FechaPrestamo,
                FechaDevolucionPrevista = prestamo.FechaDevolucionPrevista,
                FechaDevolucionReal = prestamo.FechaDevolucionReal,
                Estado = prestamo.Estado,
                PrestadoA = prestamo.PrestadoA,
                Comentarios = prestamo.Comentarios,
                Libro = prestamo.Libro != null ? new LibroDTO
                {
                    Id = prestamo.Libro.Id,
                    Titulo = prestamo.Libro.Titulo,
                    ISBN = prestamo.Libro.ISBN,
                    FechaPublicacion = prestamo.Libro.FechaPublicacion,
                    Editorial = prestamo.Libro.Editorial,
                    NumeroPaginas = prestamo.Libro.NumeroPaginas,
                    Genero = prestamo.Libro.Genero,
                    Descripcion = prestamo.Libro.Descripcion
                } : null
            };
        }
    }
}
