using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rpg_Api.Data;
using Rpg_Api.Models;
using Rpg_Api.Utils;

namespace Rpg_Api.Controllers
{

    [ApiController]
    [Route("[controller]")]

    public class UsuariosController : ControllerBase
    {
        
        private readonly  DataContext _context;

        public UsuariosController(DataContext context)
        {
            _context = context;
        }


        public async Task<bool> UsuarioExistente(string username)
        {
            if(await _context.Usuarios.AnyAsync(x => x.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;

        }



        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarUsuario(Usuario user)
        {
            try
            {
                if(await UsuarioExistente(user.Username))
                    throw new System.Exception("Nome de usuário já existente");
                
                Criptografia.CriarPasswordHash(user.PasswordString, out byte[] hash, out byte[] salt);
                user.PasswordString = string.Empty;
                user.PasswordHash = hash;
                user.PasswordSalt = salt;
                await _context.Usuarios.AddAsync(user);
                await _context.SaveChangesAsync();

                return Ok(user.Id);
            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("Autenticar")]
        public async Task<IActionResult> AutenticarUsuario(Usuario credenciais)
        {
            try
            {
                Usuario usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(credenciais.Username.ToLower()));

                if(usuario == null)
                {
                    throw new System.Exception("Usuário não encontrado. ");
                }
                else if(!Criptografia.VerificarPasswordHash(credenciais.PasswordString, usuario.PasswordHash, usuario.PasswordSalt))
                {
                    throw new System.Exception("Senha incorreta. ");
                }
                else
                {
                    return Ok(usuario.Id);
                }
            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        
    }//Fim da classe <<<UsuariosController>>>
}