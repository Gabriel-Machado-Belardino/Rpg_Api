using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rpg_Api.Data;
using Rpg_Api.Models;

using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Rpg_Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ArmasController : ControllerBase
    {
        private readonly DataContext _context;//Declaração contexto do Banco

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArmasController(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context; //inicialização do contexto do banco
            _httpContextAccessor = httpContextAccessor;
        }

        private int ObterUsuarioId()
        {
            return int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet("{id}")] //Buscar pelo id
        public async Task<IActionResult> GetSingle(int id)//using using System.Threading.Tasks;
        {
            try
            {
                Arma a = await _context.Armas                        
                       .FirstOrDefaultAsync(aBusca => aBusca.Id == id);
                //using Microsoft.EntityFrameworkCore;

                return Ok(a);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                //using System.Collections.Generic;
                List<Arma> lista = await _context.Armas                    
                    .ToListAsync();
                return Ok(lista);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(Arma novaArma)
        {
            try
            {
                if (novaArma.Dano == 0)
                {
                    throw new System.Exception("O dano da arma não pode ser 0");
                }

                Personagem p = await _context.Personagens.FirstOrDefaultAsync(p => p.Id == novaArma.PersonagemId);

                if(p == null)
                {
                    throw new System.Exception("Não existe personagem com o Id informado");
                }

                Arma buscaArma = await _context.Armas.FirstOrDefaultAsync(a => a.PersonagemId == novaArma.PersonagemId);
                if(buscaArma != null)
                    throw new System.Exception("O Personagem selecionado ja contém uma arma atribuída a ele");


                await _context.Armas.AddAsync(novaArma);
                await _context.SaveChangesAsync();

                return Ok(novaArma.Id);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(Arma novaArma)
        {
            try
            {
                if (novaArma.Dano == 0)
                {
                    throw new System.Exception("O dano da arma não pode ser 0");
                }

                _context.Armas.Update(novaArma);
                int linhaAfetadas = await _context.SaveChangesAsync();

                return Ok(linhaAfetadas);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Arma aRemover = await _context.Armas
                   .FirstOrDefaultAsync(p => p.Id == id);

                _context.Armas.Remove(aRemover);
                int linhaAfetadas = await _context.SaveChangesAsync();

                return Ok(linhaAfetadas);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}