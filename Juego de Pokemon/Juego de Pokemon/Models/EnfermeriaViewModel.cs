namespace Juego_de_Pokemon.Models
{
    public class EnfermeriaViewModel
    {
        public string Usuario { get; set; }
        public List<Pokemon> PokemonEnTratamiento { get; set; }
        public List<Pokemon> PokemonDisponibles { get; set; }
        public List<Pokemon> PokemonPendiente { get; set; }
        public List<Pokemon> PokemonCompletados { get; set; }

    }
}
