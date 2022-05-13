using Microsoft.AspNetCore.Mvc;
using System;
using Rpg_Api.Data;
using Rpg_Api.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Rpg_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DisputasController : ControllerBase
    {
        //Construtores e métodos aqui.

        private DataContext _context;

        public DisputasController(DataContext context)
        {
            _context = context;
        }


        [HttpPost("Arma")]
        public async Task<IActionResult> AtaqueComArmaAsync(Disputa d)
        {
            try
            {
                //Programe aqui por favor.
                Personagem atacante = await _context.Personagens.Include(a => a.Arma).FirstOrDefaultAsync(p => p.Id == d.AtacanteId);                    

                Personagem oponente = await _context.Personagens.FirstOrDefaultAsync(p => p.Id == d.OponenteId);

                int dano = atacante.Arma.Dano + (new Random().Next(atacante.Forca));

                dano = dano - new Random().Next(oponente.Defesa);

                if(dano > 0)
                {
                    oponente.PontosVida = oponente.PontosVida - (int)dano;
                }
                if(oponente.PontosVida <= 0)
                    d.Narracao = $"{oponente.Nome} foi derrotado";


                StringBuilder dados = new StringBuilder();
                dados.AppendFormat(" Atacante: {0}. ", atacante);
                dados.AppendFormat(" Oponente: {0}. ", oponente);
                dados.AppendFormat(" Pontos de vida do atacante: {0}. ", atacante.PontosVida);
                dados.AppendFormat(" Pontos de vida do oponente: {0}. ", oponente.PontosVida);
                dados.AppendFormat(" Arma utilizada: {0}. ", oponente.Arma.Nome);
                dados.AppendFormat(" Dano: {0}. ", dano);

                d.Narracao += dados.ToString();
                d.DataDisputa = DateTime.Now;
                _context.Disputas.Add(d);
                _context.Personagens.Update(oponente);
                await _context.SaveChangesAsync();

                return Ok(d);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }//Fim da classe DisputasController
}