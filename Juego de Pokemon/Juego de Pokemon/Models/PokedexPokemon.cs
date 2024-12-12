using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Juego_de_Pokemon.Models
{
    public class PokedexPokemon
    {
        [Key]
        [Column(Order = 0)]
        public int PokedexId { get; set; }

        [Key]
        [Column(Order = 1)]
        public int PokemonId { get; set; }

        public int Vida { get; set; }
        public string? Estado { get; set; }

        // Relaciones
        [ForeignKey("PokedexId")]
        public Pokedex Pokedex { get; set; }

        [ForeignKey("PokemonId")]
        public Pokemon Pokemon { get; set; }
    }
}
