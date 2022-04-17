using Rpg_Api.Models.Enuns;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Rpg_Api.Models
{
    public class Personagem
    {
        
        public int Id { get; set; }
        public string Nome { get; set; }
        public int PontosVida { get; set; }
        public int Forca { get; set; }
        public int Defesa { get; set; }
        public int Inteligencia { get; set; }

        public ClassEnum Classe { get; set; }

        public byte[] FotoPersonagem { get; set;}

        [JsonIgnore]
        public Usuario Usuario { get; set;}

        [JsonIgnore]
        public Arma Arma { get; set;}

        public List<PersonagemHabilidade> PersonagemHabilidade { get; set;}
    }
}