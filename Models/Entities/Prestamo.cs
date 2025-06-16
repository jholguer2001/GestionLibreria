namespace LibreriaApi.Models.Entities
{
    public class Prestamo
    {
        public int Id { get; set; }
        public int LibroId { get; set; }
        public DateTime FechaPrestamo { get; set; } = DateTime.Now;
        public DateTime FechaDevolucionPrevista { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public string Estado { get; set; } = "Prestado"; // "Prestado", "Devuelto", "Atrasado"
        public string PrestadoA { get; set; } = string.Empty;
        public string? Comentarios { get; set; }
        public Libro? Libro { get; set; }



    }
}


