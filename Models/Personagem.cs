using Rpg_Api.Models.Enuns;

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
        public Usuario Usuario { get; set;}
    }
}