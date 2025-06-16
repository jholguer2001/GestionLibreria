namespace LibreriaApi.Models.Entities
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public string? Editorial { get; set; }
        public int? NumeroPaginas { get; set; }
        public string? Genero { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;
    }
}
