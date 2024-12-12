using System.ComponentModel.DataAnnotations;

namespace Juego_de_Pokemon.Models
{
    public class Pokemon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(50)]
        public string Tipo { get; set; }

        [Required]
        [MaxLength(50)]
        public string Debilidad { get; set; }

        [Required]
        public int HP { get; set; }

        [Required]
        public int Ataque { get; set; }

        [Required]
        public int Defensa { get; set; }
        public string? Imagen { get; set; }
    }
}
