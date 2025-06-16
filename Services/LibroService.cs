using LibreriaApi.Models.DTOs;
using LibreriaApi.Models.Entities;
using LibreriaApi.Repositories;
using LibreriaApi.Repositories.Interfaces;
using LibreriaApi.Services.Interfaces;

namespace LibreriaApi.Services
{
    public class LibroService : ILibroService
    {
        private readonly ILibroRepository _libroRepository;
        private readonly IAutorRepository _autorRepository;
        private readonly IPrestamoRepository _prestamoRepository;

        public LibroService(ILibroRepository libroRepository, IAutorRepository autorRepository, IPrestamoRepository prestamoRepository)
        {
            _libroRepository = libroRepository;
            _prestamoRepository = prestamoRepository;
            _autorRepository = autorRepository;
        }

        public async Task<List<LibroDTO>> ObtenerLibrosAsync()
        {
            var libros = await _libroRepository.ObtenerLibrosAsync();
            var librosDto = new List<LibroDTO>();

            foreach (var libro in libros)
            {
                var libroDto = ConvertirALibroDTO(libro);
                libroDto.Autores = await ObtenerAutoresPorLibro(libro.Id);
                librosDto.Add(libroDto);
            }

            return librosDto;
        }

        public async Task<LibroDTO?> ObtenerLibroPorIdAsync(int id)
        {
            var libro = await _libroRepository.ObtenerLibroPorIdAsync(id);
            if (libro == null)
                return null;

            var libroDto = ConvertirALibroDTO(libro);
            libroDto.Autores = await ObtenerAutoresPorLibro(libro.Id);
            return libroDto;
        }

        public async Task<LibroDTO> CrearLibroAsync(LibroDTO libroDto)
        {
            var libro = ConvertirALibro(libroDto);
            var nuevoId = await _libroRepository.CrearLibroAsync(libro);

            libroDto.Id = nuevoId;

            // Asignar autores si los hay
            foreach (var autor in libroDto.Autores)
            {
                await _libroRepository.AsignarAutorAsync(nuevoId, autor.Id);
            }

            return libroDto;
        }

        public async Task<bool> LibroDisponibleParaPrestamoAsync(int libroId)
        {
            // Verificar que el libro existe y está activo
            var libro = await _libroRepository.ObtenerLibroPorIdAsync(libroId);
            if (libro == null || !libro.Activo)
            {
                return false;
            }

            
            return !await _prestamoRepository.ExistePrestamoActivoAsync(libroId);
        }

      

        public async Task<LibroDTO?> ActualizarLibroAsync(int id, LibroDTO libroDto)
        {
            var libroExistente = await _libroRepository.ObtenerLibroPorIdAsync(id);
            if (libroExistente == null)
                return null;

            var libro = ConvertirALibro(libroDto);
            libro.Id = id;
            libro.FechaRegistro = libroExistente.FechaRegistro; // Mantener fecha original

            var actualizado = await _libroRepository.ActualizarLibroAsync(libro);
            if (!actualizado)
                return null;

            // Actualizar autores: primero obtener autores actuales
            var autoresActuales = await _libroRepository.ObtenerAutoresPorLibroAsync(id);
            var autoresNuevos = libroDto.Autores;

            // Remover autores que ya no están en la lista
            foreach (var autorActual in autoresActuales)
            {
                if (!autoresNuevos.Any(a => a.Id == autorActual.Id))
                {
                    await _libroRepository.RemoverAutorDeLibroAsync(id, autorActual.Id);
                }
            }

            // Agregar nuevos autores
            foreach (var autorNuevo in autoresNuevos)
            {
                if (!autoresActuales.Any(a => a.Id == autorNuevo.Id))
                {
                    await _libroRepository.AsignarAutorAsync(id, autorNuevo.Id);
                }
            }

            libroDto.Id = id;
            return libroDto;
        }

        public async Task<bool> EliminarLibroAsync(int id)
        {
            return await _libroRepository.EliminarLibroAsync(id);
        }

        public async Task<bool> AsignarAutorAsync(int libroId, int autorId)
        {
            // Verificar que el libro y autor existan
            var libro = await _libroRepository.ObtenerLibroPorIdAsync(libroId);
            var autor = await _autorRepository.ObtenerAutorPorIdAsync(autorId);

            if (libro == null || autor == null)
                return false;

            return await _libroRepository.AsignarAutorAsync(libroId, autorId);
        }

        public async Task<bool> RemoverAutorAsync(int libroId, int autorId)
        {
            return await _libroRepository.RemoverAutorDeLibroAsync(libroId, autorId);
        }

        private async Task<List<AutorDTO>> ObtenerAutoresPorLibro(int libroId)
        {
            var autores = await _libroRepository.ObtenerAutoresPorLibroAsync(libroId);
            return autores.Select(ConvertirAAutorDTO).ToList();
        }

        private static LibroDTO ConvertirALibroDTO(Libro libro)
        {
            return new LibroDTO
            {
                Id = libro.Id,
                Titulo = libro.Titulo,
                ISBN = libro.ISBN,
                FechaPublicacion = libro.FechaPublicacion,
                Editorial = libro.Editorial,
                NumeroPaginas = libro.NumeroPaginas,
                Genero = libro.Genero,
                Descripcion = libro.Descripcion,
                Autores = new List<AutorDTO>()
            };
        }

        private static Libro ConvertirALibro(LibroDTO libroDto)
        {
            return new Libro
            {
                Id = libroDto.Id,
                Titulo = libroDto.Titulo,
                ISBN = libroDto.ISBN,
                FechaPublicacion = libroDto.FechaPublicacion,
                Editorial = libroDto.Editorial,
                NumeroPaginas = libroDto.NumeroPaginas,
                Genero = libroDto.Genero,
                Descripcion = libroDto.Descripcion,
                FechaRegistro = DateTime.Now,
                Activo = true
            };
        }
        private static AutorDTO ConvertirAAutorDTO(Autor autor)
        {
            return new AutorDTO
            {
                Id = autor.Id,
                Nombre = autor.Nombre,
                Apellido = autor.Apellido,
                FechaNacimiento = autor.FechaNacimiento,
                Nacionalidad = autor.Nacionalidad,
                Bibliografia = autor.Bibliografia
            };
        }



       
    }
}
