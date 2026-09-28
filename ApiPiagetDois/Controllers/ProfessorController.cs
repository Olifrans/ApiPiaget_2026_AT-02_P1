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
    public class ProfessorController : ControllerBase
    {
        private readonly ApiPiagetDoisContext _context;

        public ProfessorController(ApiPiagetDoisContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfessorDto>>> GetProfessores()
        {
            var professores = await _context.Professores
                .Include(p => p.Escola)
                .Select(p => new ProfessorDto
                {
                    Id = p.Id,
                    ProfessorNome = p.ProfessorNome,
                    ProfessorTelefone = p.ProfessorTelefone,
                    ProfessorEmail = p.ProfessorEmail,
                    ProfessorDisciplina = p.ProfessorDisciplina,
                    EscolaId = p.EscolaId,
                    EscolaNome = p.Escola != null ? p.Escola.EscolaNome : null,
                    TotalAlunosLecionados = p.AlunosLecionados != null ? p.AlunosLecionados.Count : 0,
                    DataCriacao = p.DataCriacao
                })
                .ToListAsync();

            return Ok(professores);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProfessorDto>> GetProfessor(Guid id)
        {
            var professor = await _context.Professores
                .Include(p => p.Escola)
                .Where(p => p.Id == id)
                .Select(p => new ProfessorDto
                {
                    Id = p.Id,
                    ProfessorNome = p.ProfessorNome,
                    ProfessorTelefone = p.ProfessorTelefone,
                    ProfessorEmail = p.ProfessorEmail,
                    ProfessorDisciplina = p.ProfessorDisciplina,
                    EscolaId = p.EscolaId,
                    EscolaNome = p.Escola != null ? p.Escola.EscolaNome : null,
                    TotalAlunosLecionados = p.AlunosLecionados != null ? p.AlunosLecionados.Count : 0,
                    DataCriacao = p.DataCriacao
                })
                .FirstOrDefaultAsync();

            if (professor == null)
                return NotFound();

            return Ok(professor);
        }

        [HttpPost]
        public async Task<ActionResult<Professor>> PostProfessor(CreateProfessorDto dto)
        {
            var escola = await _context.Escolas.FindAsync(dto.EscolaId);
            if (escola == null)
                return BadRequest("Escola não encontrada");

            var professor = new Professor
            {
                Id = Guid.NewGuid(),
                ProfessorNome = dto.ProfessorNome,
                ProfessorTelefone = dto.ProfessorTelefone,
                ProfessorEmail = dto.ProfessorEmail,
                ProfessorDisciplina = dto.ProfessorDisciplina,
                EscolaId = dto.EscolaId
            };

            _context.Professores.Add(professor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProfessor), new { id = professor.Id }, professor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfessor(Guid id, UpdateProfessorDto dto)
        {
            var professor = await _context.Professores.FindAsync(id);
            if (professor == null)
                return NotFound();

            if (dto.ProfessorNome != null) professor.ProfessorNome = dto.ProfessorNome;
            if (dto.ProfessorTelefone != null) professor.ProfessorTelefone = dto.ProfessorTelefone;
            if (dto.ProfessorEmail != null) professor.ProfessorEmail = dto.ProfessorEmail;
            if (dto.ProfessorDisciplina != null) professor.ProfessorDisciplina = dto.ProfessorDisciplina;
            if (dto.EscolaId.HasValue) professor.EscolaId = dto.EscolaId.Value;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfessorExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpPost("{id}/alunos/{alunoId}")]
        public async Task<IActionResult> AddAlunoToProfessor(Guid id, Guid alunoId)
        {
            var professor = await _context.Professores
                .Include(p => p.AlunosLecionados)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (professor == null)
                return NotFound("Professor não encontrado");

            var aluno = await _context.Alunos.FindAsync(alunoId);
            if (aluno == null)
                return NotFound("Aluno não encontrado");

            if (professor.AlunosLecionados == null)
                professor.AlunosLecionados = new List<Aluno>();

            if (!professor.AlunosLecionados.Any(a => a.Id == alunoId))
            {
                professor.AlunosLecionados.Add(aluno);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        [HttpDelete("{id}/alunos/{alunoId}")]
        public async Task<IActionResult> RemoveAlunoFromProfessor(Guid id, Guid alunoId)
        {
            var professor = await _context.Professores
                .Include(p => p.AlunosLecionados)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (professor == null)
                return NotFound("Professor não encontrado");

            var aluno = await _context.Alunos.FindAsync(alunoId);
            if (aluno == null || professor.AlunosLecionados == null)
                return NotFound("Aluno não encontrado");

            professor.AlunosLecionados.Remove(aluno);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfessor(Guid id)
        {
            var professor = await _context.Professores.FindAsync(id);
            if (professor == null)
                return NotFound();

            _context.Professores.Remove(professor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProfessorExists(Guid id)
        {
            return _context.Professores.Any(e => e.Id == id);
        }
    }
}