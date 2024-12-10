namespace Juego_de_Pokemon.Models
{
    public class Reto
    {
        public int Id { get; set; }
        public int RetadorId { get; set; }
        public int RetadoId { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaReto { get; set; }

        // Relaciones
        public Usuario? Retador { get; set; }
        public Usuario? Retado { get; set; }
    }
}
