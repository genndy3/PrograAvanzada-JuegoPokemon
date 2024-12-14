using System.ComponentModel.DataAnnotations;

namespace Juego_de_Pokemon.Models
{
    public class Pokemon
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio")]
        public string Tipo { get; set; }

        public string Debilidad { get; set; }
        public int HP { get; set; }

        public int Ataque { get; set; }
        public int Defensa { get; set; }
    }
}
