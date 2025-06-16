using LibreriaApi.Models.DTOs;
using LibreriaApi.Models.Entities;
using LibreriaApi.Repositories.Interfaces;
using LibreriaApi.Services.Interfaces;

namespace LibreriaApi.Services
{
    public class AutorService : IAutorService
    {
        private readonly IAutorRepository _autorRepository;

        public AutorService(IAutorRepository autorRepository)
        {
            _autorRepository = autorRepository;
        }

        public async Task<List<AutorDTO>> ObtenerAutoresAsync()
        {
            var autores = await _autorRepository.ObtenerAutoresAsync();
            return autores.Select(ConvertirAAutorDTO).ToList();
        }

        public async Task<AutorDTO?> ObtenerAutorPorIdAsync(int id)
        {
            var autor = await _autorRepository.ObtenerAutorPorIdAsync(id);
            return autor == null ? null : ConvertirAAutorDTO(autor);
        }

        public async Task<AutorDTO> CrearAutorAsync(AutorDTO autorDto)
        {
            var autor = ConvertirAAutor(autorDto);
            var nuevoId = await _autorRepository.CrearAutorAsync(autor);

            autorDto.Id = nuevoId;
            return autorDto;
        }

        public async Task<AutorDTO?> ActualizarAutorAsync(int id, AutorDTO autorDto)
        {
            var autorExistente = await _autorRepository.ObtenerAutorPorIdAsync(id);
            if (autorExistente == null)
                return null;

            var autor = ConvertirAAutor(autorDto);
            autor.Id = id;
            autor.FechaRegistro = autorExistente.FechaRegistro; // Mantener fecha original

            var actualizado = await _autorRepository.ActualizarAutorAsync(autor);
            return actualizado ? ConvertirAAutorDTO(autor) : null;
        }

        public async Task<bool> EliminarAutorAsync(int id)
        {
            return await _autorRepository.EliminarAutorAsync(id);
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
        private static Autor ConvertirAAutor(AutorDTO autorDto)
        {
            return new Autor
            {
                Id = autorDto.Id,
                Nombre = autorDto.Nombre,
                Apellido = autorDto.Apellido,
                FechaNacimiento = autorDto.FechaNacimiento,
                Nacionalidad = autorDto.Nacionalidad,
                Bibliografia = autorDto.Bibliografia,
                FechaRegistro = DateTime.Now,
                Activo = true
            };
        }
    }
}
