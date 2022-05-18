using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rpg_Api.Data;
using Rpg_Api.Models;
using Rpg_Api.Utils;

using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Authorization;

namespace Rpg_Api.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]

    public class UsuariosController : ControllerBase
    {
        
        private readonly  DataContext _context;

        private readonly IConfiguration _configuration;

        public UsuariosController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        public async Task<bool> UsuarioExistente(string username)
        {
            if(await _context.Usuarios.AnyAsync(x => x.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;

        }

        private string CriarToken(Usuario usuario)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Role, usuario.Perfil)
            };
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_configuration.GetSection("ConfiguracaoToken:Chave").Value));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


        [AllowAnonymous]
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

        [AllowAnonymous]
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
                    usuario.DataAcesso = DateTime.Now;
                    _context.Usuarios.Update(usuario);
                    int linhasAfetadas = await _context.SaveChangesAsync();


                    return Ok(CriarToken(usuario));
                }
            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        
        
        [HttpPut("AlterarSenha")]
        public async Task<IActionResult> AlterarSenha(Usuario ModUser)
        {
            try
            {
                Usuario modUsuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(ModUser.Username.ToLower()));

                if(modUsuario == null)
                {
                    throw new System.Exception("Usuário não encontrado. ");
                }

                Criptografia.CriarPasswordHash(ModUser.PasswordString, out byte[] hash, out byte[] salt);
                modUsuario.PasswordHash = hash;
                modUsuario.PasswordSalt = salt;

                _context.Usuarios.Update(modUsuario);
                int linhasAfetadas = await _context.SaveChangesAsync();
                return Ok(linhasAfetadas);
                
            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                List<Usuario> AllUser = await _context.Usuarios.ToListAsync();
                return Ok(AllUser);

            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        
        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> GetUsuario(int usuarioId)
        {
            try
            {
                //List exigirá o using System.Collections.Generic
                Usuario usuario = await _context.Usuarios //Busca o usuário no banco através do Id
                    .FirstOrDefaultAsync(x => x.Id == usuarioId);

                return Ok(usuario);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } 

        [HttpGet("GetByLogin/{login}")]
        public async Task<IActionResult> GetUsuario(string login)
        {
            try
            {
            //List exigirá o using System.Collections.Generic
                Usuario usuario = await _context.Usuarios //Busca o usuário no banco através do login 
                    .FirstOrDefaultAsync(x => x.Username.ToLower() == login.ToLower());  
                
                return Ok(usuario);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //Método para alteração da geolocalização
        [HttpPut("AtualizarLocalizacao")]
        public async Task<IActionResult> AtualizarLocalizacao(Usuario u)
        {
            try
            {
                Usuario usuario = await _context.Usuarios //Busca o usuário no banco através do Id 
                    .FirstOrDefaultAsync(x => x.Id == u.Id);


                usuario.Latitude = u.Latitude;
                usuario.Longitude = u.Longitude;


                var attach = _context.Attach(usuario);
                attach.Property(x => x.Id).IsModified = false;
                attach.Property(x => x.Latitude).IsModified = true;
                attach.Property(x => x.Longitude).IsModified = true;


                int linhasAfetadas = await _context.SaveChangesAsync(); //Confirma a alteração no banco 
                return Ok(linhasAfetadas); //Retorna as linhas afetadas (Geralmente sempre 1 linha msm) 
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //Método para alteração do e-mail
        [HttpPut("AtualizarEmail")]
        public async Task<IActionResult> AtualizarEmail(Usuario u)
        {
            try
            {
                Usuario usuario = await _context.Usuarios //Busca o usuário no banco através do Id 
                    .FirstOrDefaultAsync(x => x.Id == u.Id);
                
                
                usuario.Email = u.Email;


                var attach = _context.Attach(usuario);
                attach.Property(x => x.Id).IsModified = false;
                attach.Property(x => x.Email).IsModified = true;


                int linhasAfetadas = await _context.SaveChangesAsync(); //Confirma a alteração no banco 
                return Ok(linhasAfetadas); //Retorna as linhas afetadas (Geralmente sempre 1 linha msm) 
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Método para alteração da foto
        [HttpPut("AtualizarFoto")]
        public async Task<IActionResult> AtualizarFoto(Usuario u)
        {
            try
            {
                Usuario usuario = await _context.Usuarios
                .FirstOrDefaultAsync(x => x.Id == u.Id);


                usuario.Foto = u.Foto;


                var attach = _context.Attach(usuario);
                attach.Property(x => x.Id).IsModified = false;
                attach.Property(x => x.Foto).IsModified = true;


                int linhasAfetadas = await _context.SaveChangesAsync();  
                return Ok(linhasAfetadas);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
    }//Fim da classe <<<UsuariosController>>>
}