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
    public class AlunoController : ControllerBase
    {
        private readonly ApiPiagetDoisContext _context;

        public AlunoController(ApiPiagetDoisContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlunoDto>>> GetAlunos()
        {
            var alunos = await _context.Alunos
                .Include(a => a.Escola)
                .Include(a => a.Professores)
                .Select(a => new AlunoDto
                {
                    Id = a.Id,
                    AlunoNome = a.AlunoNome,
                    AlunoTelefone = a.AlunoTelefone,
                    AlunoEmail = a.AlunoEmail,
                    AlunoMatricula = a.AlunoMatricula,
                    EscolaId = a.EscolaId,
                    EscolaNome = a.Escola != null ? a.Escola.EscolaNome : null,
                    TotalProfessores = a.Professores != null ? a.Professores.Count : 0,
                    DataCriacao = a.DataCriacao
                })
                .ToListAsync();

            return Ok(alunos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AlunoDto>> GetAluno(Guid id)
        {
            var aluno = await _context.Alunos
                .Include(a => a.Escola)
                .Include(a => a.Professores)
                .Where(a => a.Id == id)
                .Select(a => new AlunoDto
                {
                    Id = a.Id,
                    AlunoNome = a.AlunoNome,
                    AlunoTelefone = a.AlunoTelefone,
                    AlunoEmail = a.AlunoEmail,
                    AlunoMatricula = a.AlunoMatricula,
                    EscolaId = a.EscolaId,
                    EscolaNome = a.Escola != null ? a.Escola.EscolaNome : null,
                    TotalProfessores = a.Professores != null ? a.Professores.Count : 0,
                    DataCriacao = a.DataCriacao
                })
                .FirstOrDefaultAsync();

            if (aluno == null)
                return NotFound();

            return Ok(aluno);
        }

        [HttpPost]
        public async Task<ActionResult<Aluno>> PostAluno(CreateAlunoDto dto)
        {
            var escola = await _context.Escolas.FindAsync(dto.EscolaId);
            if (escola == null)
                return BadRequest("Escola não encontrada");

            var existingAluno = await _context.Alunos
                .AnyAsync(a => a.AlunoMatricula == dto.AlunoMatricula);

            if (existingAluno)
                return BadRequest("Já existe um aluno com esta matrícula");

            var aluno = new Aluno
            {
                Id = Guid.NewGuid(),
                AlunoNome = dto.AlunoNome,
                AlunoTelefone = dto.AlunoTelefone,
                AlunoEmail = dto.AlunoEmail,
                AlunoMatricula = dto.AlunoMatricula,
                EscolaId = dto.EscolaId
            };

            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAluno), new { id = aluno.Id }, aluno);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAluno(Guid id, UpdateAlunoDto dto)
        {
            var aluno = await _context.Alunos.FindAsync(id);
            if (aluno == null)
                return NotFound();

            if (dto.AlunoNome != null) aluno.AlunoNome = dto.AlunoNome;
            if (dto.AlunoTelefone != null) aluno.AlunoTelefone = dto.AlunoTelefone;
            if (dto.AlunoEmail != null) aluno.AlunoEmail = dto.AlunoEmail;
            if (dto.AlunoMatricula != null) aluno.AlunoMatricula = dto.AlunoMatricula;
            if (dto.EscolaId.HasValue) aluno.EscolaId = dto.EscolaId.Value;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlunoExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAluno(Guid id)
        {
            var aluno = await _context.Alunos.FindAsync(id);
            if (aluno == null)
                return NotFound();

            _context.Alunos.Remove(aluno);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AlunoExists(Guid id)
        {
            return _context.Alunos.Any(e => e.Id == id);
        }
    }
}