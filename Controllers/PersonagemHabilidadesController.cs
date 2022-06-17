using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Rpg_Api.Models;
using Rpg_Api.Data;

namespace Rpg_Api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PersonagemHabilidadesController : ControllerBase
    {
        private readonly DataContext _context;

        public PersonagemHabilidadesController(DataContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        public async Task<IActionResult> AddPersonagemHabilidadeAsync(PersonagemHabilidade novoPersonagemHabilidade)
        {
            try
            {
                Personagem personagem = await _context.Personagens
                .Include(p => p.Arma)
                .Include(p => p.PersonagemHabilidade).ThenInclude(ps => ps.Habilidade)
                .FirstOrDefaultAsync(p => p.Id == novoPersonagemHabilidade.PersonagemId);

                if(personagem == null)
                    throw new System.Exception("Personagem não encontrado para o Id informado.");
                
                Habilidade habilidade = await _context.Habilidades.FirstOrDefaultAsync(h => h.Id == novoPersonagemHabilidade.HabilidadeId);

                if(habilidade == null)
                    throw new System.Exception("Habilidade não encontrada.");

                PersonagemHabilidade ph = new PersonagemHabilidade();
                ph.Personagem = personagem;
                ph.Habilidade = habilidade;
                await _context.PersonagemHabilidades.AddAsync(ph);
                int linhasAfetadas = await _context.SaveChangesAsync();

                return Ok(linhasAfetadas);
            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ListHabilitsbyCaracterId(int Id)
        {
            try
            {
                
                List<PersonagemHabilidade> ph = await _context.PersonagemHabilidades.ToListAsync();
                ph = ph.FindAll(x => x.PersonagemId == Id);
                

                if(ph == null)
                {
                    throw new System.Exception("Personagem não encontrado pelo Id");
                }
                else

                return Ok(ph);
            }
            catch (System.Exception ex)
            {
                
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("GetHabilidades")]
         public async Task<IActionResult> GetHabilidades()
         {
             try
            {
                List<Habilidade> habilits = await _context.Habilidades.ToListAsync();

                return Ok(habilits);
            }
            catch(System.Exception ex)
            {

                return BadRequest(ex.Message);
            }
         }

         [HttpPost("DeletePersonagemHabilidade")]
         public async Task<IActionResult> DeletePersonagemHabilidade(PersonagemHabilidade phDel)
         {
             try
            {
                PersonagemHabilidade phRemove = await _context.PersonagemHabilidades.FirstOrDefaultAsync(x => x.PersonagemId == phDel.PersonagemId && x.HabilidadeId == phDel.HabilidadeId);
                if(phRemove == null)
                {
                    throw new System.Exception("PersonagemHabilidade não encontrado");
                }

                _context.PersonagemHabilidades.Remove(phRemove);
                int linhasAfetadas = await _context.SaveChangesAsync();
                return Ok(linhasAfetadas);
            }
            catch(System.Exception ex)
            {

                return BadRequest(ex.Message);
            }
         }



    }
}

/*
    try
    {
        
        return Ok();
    }
    catch(System.Exception ex)
    {
        return BadRequest(ex.Message);
    }
*/