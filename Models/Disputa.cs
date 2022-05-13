using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rpg_Api.Models
{
    public class Disputa
    {
        public int Id { get; set; }
        public DateTime? DataDisputa { get; set; }
        public int AtacanteId { get; set; }
        public int OponenteId { get; set; }
        public string Narracao { get; set; }

        [NotMapped]
        public int HabilidadeId { get; set; }
        [NotMapped]
        public List<int> ListaIdPersonagens { get; set; }
        [NotMapped]
        public List<string> Resultados { get; set; }

    }
}