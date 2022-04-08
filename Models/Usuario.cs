using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Rpg_Api.Models
{
    public class Usuario
    {
        
        public int Id { get; set;}
        public string Username { get; set;}
        public byte[] PasswordHash { get; set;}
        public byte[] PasswordSalt { get; set;}
        public byte[] Foto { get; set;}
        public string Latitude { get; set;}
        public string Longitude { get; set;}
        public DataTime? DataAcesso { get; set;}

        [NotMapped]
        public string PasswordString { get; set;}
        public List<Personagem> Personagens { get; set;}
        public int Id { get; set;}


    }
}