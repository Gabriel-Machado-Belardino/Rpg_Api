using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rpg_Api.Data;
using Rpg_Api.Models;

namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DisputaController : ControllerBase
    {
        //Construtores e métodos aqui.

        private readonly DataContext _context;

        public DisputaController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("Arma")]

        public async Task<IActionResult> AtaqueComArmaAsync(Disputa d)
        {
            try
            {
                Personagem atacante = await _context.Personagens
                    .Include(p => p.Arma)
                    .FirstOrDefaultAsync(p => p.Id == d.AtacanteId);

                Personagem oponente = await _context.Personagens
                    .FirstOrDefaultAsync(p => p.Id == d.OponenteId);

                int dano = atacante.Arma.Dano + (new Random().Next(atacante.Forca));
                dano = dano - new Random().Next(oponente.Defesa);

                if (dano > 0)
                {
                    oponente.PontosVida = oponente.PontosVida - dano;
                }
                if (oponente.PontosVida <= 0)
                {
                    d.Narracao = $"{oponente.Nome} foi derrotado!";
                }

                _context.Personagens.Update(oponente);
                await _context.SaveChangesAsync();

                StringBuilder dados = new StringBuilder();
                dados.AppendFormat(" Atacante : {0}. ", atacante.Nome);
                dados.AppendFormat(" Oponente: {0}. ", oponente.Nome);
                dados.AppendFormat(" Pontos de Vida do Atacante: {0}. ", atacante.PontosVida);
                dados.AppendFormat("Arma Utilizada: {0}. ", atacante.Arma.Nome);
                dados.AppendFormat(" Dano: {0}. ", dano);

                d.Narracao += dados.ToString();
                d.DataDisputa = DateTime.Now;
                _context.Disputas.Add(d);
                _context.SaveChanges();

                return Ok(d);

            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("Habilidade")]
        public async Task<IActionResult> AtaqueComHabilidadeAsync(Disputa d)
        {
            try
            {
                Personagem atacante = await _context.Personagens
                    .Include(p => p.PersonagemHabilidade)
                    .ThenInclude(ph => ph.Habilidade)
                    .FirstOrDefaultAsync(p => p.Id == d.AtacanteId);

                Personagem oponente = await _context.Personagens
                    .FirstOrDefaultAsync(p => p.Id == d.OponenteId);

                PersonagemHabilidade ph = await _context.PersonagemHabilidades
                    .Include(p => p.Habilidade)
                    .FirstOrDefaultAsync(phBusca => phBusca.HabilidadeId == d.HabilidadeId);

                if (ph == null)
                    d.Narracao = $"{atacante.Nome} não possui esta habilidade";
                else
                {
                    int dano = ph.Habilidade.Dano + (new Random().Next(atacante.Inteligencia));
                    dano = dano - new Random().Next(oponente.Defesa);

                    if (dano > 0)
                    {
                        //oponente.PontosVida = oponente.PontosVida - dano;
                        oponente.PontosVida -= dano;
                    }
                    if (oponente.PontosVida <= 0)
                    {
                        d.Narracao += $"{oponente.Nome} foi derrotado!";
                    }

                    _context.Personagens.Update(oponente);
                    await _context.SaveChangesAsync();

                    StringBuilder dados = new StringBuilder();
                    dados.AppendFormat(" Atacante : {0}. ", atacante.Nome);
                    dados.AppendFormat(" Pontos de Vida do Atacante: {0}. ", atacante.PontosVida);
                    dados.AppendFormat(" Oponente : {0}. ", oponente.Nome);
                    dados.AppendFormat(" Pontos de Vida do Oponente: {0}. ", oponente.PontosVida);
                    dados.AppendFormat(" Habilidade Utilizada: {0}. ", ph.Habilidade.Nome);
                    dados.AppendFormat(" Dano: {0}. ", dano);

                    d.Narracao += dados.ToString();
                    d.DataDisputa = DateTime.Now;
                    _context.Disputas.Add(d);
                    _context.SaveChanges();
                }
                return Ok(d);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("PersonagemRandom")]

        public async Task<IActionResult> Sorteio()
        {
            List<Personagem> personagens = await _context.Personagens.ToListAsync();

            //Sorteio com numero da quantidade de personagens
            int sorteio = new Random().Next(personagens.Count);

            //busca na lista pelo indice sorteado (Não é o ID)
            Personagem p = personagens[sorteio];

            string msg = string.Format("N Sorteado {0}. Personagem: {1}", sorteio, p.Nome);

            return Ok(msg);
        }


        [HttpPost("DisputaEmGrupo")]
        public async Task<IActionResult> DisputaEmGrupoAsync(Disputa d)
        {
            try
            {
                //Busca na base dos personagens informados no parametro incluindo Armas e Habilidades
                List<Personagem> personagems = await _context.Personagens
                    .Include(p => p.Arma)
                    .Include(p => p.PersonagemHabilidade).ThenInclude(ph => ph.Habilidade)
                    .Where(p => d.ListaIdPersonagens.Contains(p.Id)).ToListAsync();

                //Contagem de personagens vivos na lista obtida do banco de dados
                int qtdPersonagensVivos = personagems.FindAll(p => p.PontosVida > 0).Count;

                //Enquanto houver mais de um personagem vivo haverá disputa
                while (qtdPersonagensVivos > 1)
                {
                    //Seleciona personagens com pontos vida positivos e depois faz sorteio
                    List<Personagem> atacantes = personagems.Where(p => p.PontosVida > 0).ToList();
                    Personagem atacante = atacantes[new Random().Next(atacantes.Count)];
                    d.AtacanteId = atacante.Id;

                    //Seleciona personagens com pontos de vida positivos, exceto o atacante escolhido e depois faz sorteio
                    List<Personagem> oponentes = personagems.Where(p => p.Id != atacante.Id && p.PontosVida > 0).ToList();
                    Personagem oponente = oponentes[new Random().Next(oponentes.Count)];
                    d.OponenteId = oponente.Id;

                    //declara e redefine a cada passagem do while o valor das variaveis que serão usadas
                    int dano = 0;
                    string ataqueUsado = string.Empty;
                    string resultado = string.Empty;

                    //sorteira entre 0 e 1:  0 é ataque com arma   1 ataque com habilidade
                    bool ataqueUsaArma = (new Random().Next(1) == 0);

                    if (ataqueUsaArma && atacante.Arma != null)
                    {
                        //programação do ataque com arma caso o atacante possua arma  (!=null)
                        //Sorteio da força
                        dano = atacante.Arma.Dano + (new Random().Next(atacante.Forca));
                        dano = dano - new Random().Next(oponente.Defesa); // sorteio da defesa
                        ataqueUsado = atacante.Arma.Nome;

                        if (dano > 0)
                        {
                            oponente.PontosVida = oponente.PontosVida - (int)dano;
                        }
                        //formata a mensagem
                        resultado = string.Format("{0} atacou {1} usando {2} com o dano {3}.", atacante.Nome, oponente.Nome, ataqueUsado, dano);
                        d.Narracao += resultado; // concatena o resultado com as narrações existentes
                        d.Resultados.Add(resultado); //add o resultado atual na lista de resultados
                    }
                    else if (atacante.PersonagemHabilidade.Count != 0)//verifica se o personagem tem habilidades
                    {
                        //programação do ataque com habilidade
                        //realiza o sorteio entre as habilidades existentes e na linha seguinte a seleciona
                        int sorteioHabilidadeId = new Random().Next(atacante.PersonagemHabilidade.Count);
                        Habilidade habilidadeEscolhida = atacante.PersonagemHabilidade[sorteioHabilidadeId].Habilidade;
                        ataqueUsado = habilidadeEscolhida.Nome;

                        //sorteio da inteligencia somada ao dano
                        dano = habilidadeEscolhida.Dano + (new Random().Next(atacante.Inteligencia));
                        dano = dano - new Random().Next(oponente.Defesa);//sorteio da defesa

                        if (dano > 0)
                            oponente.PontosVida = oponente.PontosVida - (int)dano;

                        resultado = string.Format("{0} atacou {1} usando {2} com o dano {3}.", atacante.Nome, oponente.Nome, ataqueUsado, dano);
                        d.Narracao += resultado;
                        d.Resultados.Add(resultado);
                    }
                    //ATENÇÂO: aqui ficará a programação da verificação do ataque usado e verificação se existe mais de um personagem vivo
                    if (!string.IsNullOrEmpty(ataqueUsado))
                    {
                        //incrementa os dados dos combates
                        atacante.Vitorias++;
                        oponente.Derrotas++;
                        atacante.Disputas++;
                        oponente.Disputas++;

                        d.Id = 0; //zera o id para poder salvar os dados de disputa sem erro de chave
                        d.DataDisputa = DateTime.Now;
                        _context.Disputas.Add(d);
                        await _context.SaveChangesAsync();
                    }

                    qtdPersonagensVivos = personagems.FindAll(p => p.PontosVida > 0).Count;

                    if (qtdPersonagensVivos == 1)//havendo só um personagem vivo, existe um CAMPEÃO
                    {
                        string resultadoFinal = $"{atacante.Nome.ToUpper()} é o CAMPEÃO com {atacante.PontosVida} pontos de vida restantes!";
                        d.Narracao += resultadoFinal; //concatena o resultado final com as demais narrações
                        d.Resultados.Add(resultadoFinal); //concatena o resultado final com os demais resultados

                        break; //break vai parar o while.
                    }
                }//FIM DO WHILE
                //Código após o fechamento do While. Atualizará os pontos de vida,
                //disputas, vitórias e derrotas de todos os personagens ao final das batalhas
                _context.Personagens.UpdateRange(personagems);
                await _context.SaveChangesAsync();

                return Ok(d);  //retorna os dados de disputa
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("ApagarDisputas")]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                List<Disputa> disputas = await _context.Disputas.ToListAsync();

                _context.Disputas.RemoveRange(disputas);
                await _context.SaveChangesAsync();

                return Ok("Disputas apagadas");

            }
            catch (System.Exception ex)
            { return BadRequest(ex.Message); }
        }



        [HttpPut("RestaurarPontosVida")]
        public async Task<IActionResult> RestaurarPontosVidaAsync(Personagem p)
        {
            try
            {
                int linhasAfetadas = 0;
                Personagem pEncontrado =
                    await _context.Personagens.FirstOrDefaultAsync(pBusca => pBusca.Id == p.Id);
                pEncontrado.PontosVida = 100;

                bool atualizou = await TryUpdateModelAsync<Personagem>(pEncontrado, "p",
                    pAtualizar => pAtualizar.PontosVida);
                //EF vai detectar e atualizar apenas as colunas que forma alteradas
                if (atualizou)
                    linhasAfetadas = await _context.SaveChangesAsync();

                return Ok(linhasAfetadas);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPut("ZerarRanking")]
        public async Task<IActionResult> ZerarRankingAsync(Personagem p)
        {
            try
            {
                Personagem pEncontrado =
                await _context.Personagens.FirstOrDefaultAsync(pBusca => pBusca.Id == p.Id);
                pEncontrado.Disputas = 0;
                pEncontrado.Vitorias = 0;
                pEncontrado.Derrotas = 0;
                int linhasAfetadas = 0;

                bool atualizou = await TryUpdateModelAsync<Personagem>(pEncontrado, "p",
                pAtualizar => pAtualizar.Disputas,
                pAtualizar => pAtualizar.Vitorias,
                pAtualizar => pAtualizar.Derrotas);

                // EF vai detectar e atualizar apenas as colunas que foram alteradas.
                if (atualizou)
                    linhasAfetadas = await _context.SaveChangesAsync();

                return Ok(linhasAfetadas);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPut("ZerarRankingRestaurarVidas")]
        public async Task<IActionResult> ZerarRankingRestaurarVidasAsync()
        {
            try
            {
                List<Personagem> lista =
                await _context.Personagens.ToListAsync();
                foreach (Personagem p in lista)
                {
                    await ZerarRankingAsync(p);
                    await RestaurarPontosVidaAsync(p);
                }
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }





    }
}