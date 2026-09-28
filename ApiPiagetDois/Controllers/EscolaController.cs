using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiPiagetDois.Data;
using ApiPiagetDois.Models;
using ApiPiagetDois.Models.DTOs;


namespace ApiPiagetDois.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EscolaController : ControllerBase
    {
        private readonly ApiPiagetDoisContext _context;

        public EscolaController(ApiPiagetDoisContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EscolaDto>>> GetEscolas()
        {
            var escolas = await _context.Escolas
                .Select(e => new EscolaDto
                {
                    Id = e.Id,
                    EscolaNome = e.EscolaNome,
                    EscolaTelefone = e.EscolaTelefone,
                    EscolaEmail = e.EscolaEmail,
                    EscolaEndereco = e.EscolaEndereco,
                    TotalProfessores = e.Professores != null ? e.Professores.Count : 0,
                    TotalAlunos = e.Alunos != null ? e.Alunos.Count : 0,
                    DataCriacao = e.DataCriacao
                })
                .ToListAsync();

            return Ok(escolas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EscolaDto>> GetEscola(Guid id)
        {
            var escola = await _context.Escolas
                .Where(e => e.Id == id)
                .Select(e => new EscolaDto
                {
                    Id = e.Id,
                    EscolaNome = e.EscolaNome,
                    EscolaTelefone = e.EscolaTelefone,
                    EscolaEmail = e.EscolaEmail,
                    EscolaEndereco = e.EscolaEndereco,
                    TotalProfessores = e.Professores != null ? e.Professores.Count : 0,
                    TotalAlunos = e.Alunos != null ? e.Alunos.Count : 0,
                    DataCriacao = e.DataCriacao
                })
                .FirstOrDefaultAsync();

            if (escola == null)
                return NotFound();

            return Ok(escola);
        }

        [HttpPost]
        public async Task<ActionResult<Escola>> PostEscola(CreateEscolaDto dto)
        {
            var escola = new Escola
            {
                Id = Guid.NewGuid(),
                EscolaNome = dto.EscolaNome,
                EscolaTelefone = dto.EscolaTelefone,
                EscolaEmail = dto.EscolaEmail,
                EscolaEndereco = dto.EscolaEndereco
            };

            _context.Escolas.Add(escola);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEscola), new { id = escola.Id }, escola);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEscola(Guid id, UpdateEscolaDto dto)
        {
            var escola = await _context.Escolas.FindAsync(id);
            if (escola == null)
                return NotFound();

            if (dto.EscolaNome != null) escola.EscolaNome = dto.EscolaNome;
            if (dto.EscolaTelefone != null) escola.EscolaTelefone = dto.EscolaTelefone;
            if (dto.EscolaEmail != null) escola.EscolaEmail = dto.EscolaEmail;
            if (dto.EscolaEndereco != null) escola.EscolaEndereco = dto.EscolaEndereco;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EscolaExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEscola(Guid id)
        {
            var escola = await _context.Escolas.FindAsync(id);
            if (escola == null)
                return NotFound();

            _context.Escolas.Remove(escola);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EscolaExists(Guid id)
        {
            return _context.Escolas.Any(e => e.Id == id);
        }
    }
}