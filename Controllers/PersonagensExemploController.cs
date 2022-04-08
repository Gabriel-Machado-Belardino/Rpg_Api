using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Rpg_Api.Models;
using Rpg_Api.Models.Enuns;

namespace Rpg_Api.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class PersonagensExemploController : ControllerBase
    {
        //Toda codificação sera feita aqui
        //Crição de uma lista já adicionando os objetos dentro

        private static List<Personagem> personagens = new List<Personagem>(){
            new Personagem() { Id = 1, Nome = "Frodo", PontosVida=100, Forca=17, Defesa=23, Inteligencia=33, Classe=ClassEnum.Cavaleiro}, 
            new Personagem() { Id = 2, Nome = "Sam", PontosVida=100, Forca=15, Defesa=25, Inteligencia=30, Classe=ClassEnum.Cavaleiro},
            new Personagem() { Id = 3, Nome = "Galadriel", PontosVida=100, Forca=18, Defesa=21, Inteligencia=35, Classe=ClassEnum.Clerigo },
            new Personagem() { Id = 4, Nome = "Gandalf", PontosVida=100, Forca=18, Defesa=18, Inteligencia=37, Classe=ClassEnum.Mago },
            new Personagem() { Id = 5, Nome = "Hobbit", PontosVida=100, Forca=20, Defesa=17, Inteligencia=31, Classe=ClassEnum.Cavaleiro },
            new Personagem() { Id = 6, Nome = "Celeborn", PontosVida=100, Forca=21, Defesa=13, Inteligencia=34, Classe=ClassEnum.Clerigo },
            new Personagem() { Id = 7, Nome = "Radagast", PontosVida=100, Forca=25, Defesa=11, Inteligencia=35, Classe=ClassEnum.Mago }
        };


        //============ Pegar o primeiro Personagem da Lista ===================
        [HttpGet("GetFirst")]
        public IActionResult GetFirst()
        {
            Personagem p = personagens[0];
            return Ok(p);
        }
        
        //============ Mostrar Todos os Personagens da Lista =======================
        [HttpGet("GetAll")]
        public IActionResult Get()
        {
            return Ok(personagens);
        }

        //============= Buscar o personagem pelo Id ====================
        [HttpGet("SelectId/{id}")]
        public IActionResult GetSingle(int id)
        {
            return Ok(personagens.FirstOrDefault(pe => pe.Id == id));
        }

        //================ Buscar pelos personagens de uma classe escolhida =======================
        [HttpGet("GetByClasse/{ClassChoosed}")]
        public IActionResult GetByClasse(int ClassChoosed)
        {
            List<Personagem> personagemByClass = personagens.FindAll(x => x.Classe == (ClassEnum)ClassChoosed);
            return Ok(personagemByClass);
        }

        //============== Obter pelo Nome do Personagem =============================
        [HttpGet("GetByNome-{Name}")]
        public IActionResult GetByNome(string Name)
        {
            return Ok(personagens.FindAll(x => x.Nome.ToLower().Contains(Name)));
        }

        //=============== Adicionar Personagem a lista =====================
        [HttpPost]
        public IActionResult AddPersonagem(Personagem NewCarcter)
        {   
            personagens.Add(NewCarcter);
            return Ok(personagens);
        }

        //=============== Inserir e Validar o personagem inserido segundo os padroes ==========================
        [HttpPost("Validacao-Personagem")]
        public IActionResult PostValidacao(Personagem NovoPersonagem)
        {
            if(NovoPersonagem.Defesa < 10)
            {
                return BadRequest("Minimo de 10 de defesa!!!");
                
            }
            else if(NovoPersonagem.Inteligencia > 30)
            {
                return BadRequest("Maximo de 30 de inteligencia!!!");
            }
            else
            {
                personagens.Add(NovoPersonagem);
            }
            return NotFound();
                
        }


        //==================== Inserir e Validar o Mago segundo os padrões ==========================
        [HttpPost("Validacao-Mago")]
        public IActionResult PostValidacaoMago(Personagem NovoPersonagem2)
        {
            if(NovoPersonagem2.Classe == ClassEnum.Mago && NovoPersonagem2.Inteligencia < 35)
            {
                return BadRequest("Magos nao podem possuir menos de 35 de inteligencia");
            }
            else
            {
                personagens.Add(NovoPersonagem2);
            }
            return NotFound();
        }

        //================== Mostrar a lista so com os magos e clerigos ====================
        [HttpGet("GetClerigoMago")]
        public IActionResult GetClerigoMago()
        {
           List<Personagem> CleriMago = personagens.FindAll(x => x.Classe == ClassEnum.Mago || x.Classe == ClassEnum.Clerigo);
           return Ok(CleriMago);
        }

        //================== Obter as estatisticas somadas dos personagens ==========================
        [HttpGet("GetStatistic")]
        public IActionResult GetStatistic()
        {

            return Ok("Quantidade de personagens: " + personagens.Count() + "Inteligencia somada: " + personagens.Sum(x => x.Inteligencia));
        }

        //=================== Alteração de informações de um personagem =================
        [HttpPut]
        public IActionResult UpdatePersonagem(Personagem p)
        {
            Personagem pernsagemAlterado = personagens.Find(pers => pers.Id == p.Id);
            pernsagemAlterado.Nome = p.Nome;
            pernsagemAlterado.PontosVida = p.PontosVida;
            pernsagemAlterado.Forca = p.Forca;
            pernsagemAlterado.Defesa = p.Defesa;
            pernsagemAlterado.Inteligencia = p.Inteligencia;
            pernsagemAlterado.Classe = p.Classe;
            return Ok(personagens);
        }

        //===================== Deletar um usuario pelo Id ======================
        [HttpDelete("{DeleteId}")]
        public IActionResult DeletebyId(int DeleteId)
        {
            personagens.RemoveAll(pers => pers.Id == DeleteId);
            return Ok(personagens);
        }


    }
}