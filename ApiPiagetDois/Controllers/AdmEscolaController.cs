using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiPiagetDois.Data;
using ApiPiagetDois.Models;

namespace ApiPiagetDois.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdmEscolaController : ControllerBase
    {
        private readonly ApiPiagetDoisContext _context;

        public AdmEscolaController(ApiPiagetDoisContext context)
        {
            _context = context;
        }

        // GET: api/AdmEscola
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdmEscola>>> GetAdmEscola()
        {
            return await _context.AdmEscolas.ToListAsync();
        }

        // GET: api/AdmEscola/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdmEscola>> GetAdmEscola(Guid id)
        {
            var admEscola = await _context.AdmEscolas.FindAsync(id);

            if (admEscola == null)
            {
                return NotFound();
            }

            return admEscola;
        }

        // PUT: api/AdmEscola/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdmEscola(Guid id, AdmEscola admEscola)
        {
            if (id != admEscola.Id)
            {
                return BadRequest();
            }

            _context.Entry(admEscola).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdmEscolaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AdmEscola
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AdmEscola>> PostAdmEscola(AdmEscola admEscola)
        {
            _context.AdmEscolas.Add(admEscola);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAdmEscola", new { id = admEscola.Id }, admEscola);
        }

        // DELETE: api/AdmEscola/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmEscola(Guid id)
        {
            var admEscola = await _context.AdmEscolas.FindAsync(id);
            if (admEscola == null)
            {
                return NotFound();
            }

            _context.AdmEscolas.Remove(admEscola);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdmEscolaExists(Guid id)
        {
            return _context.AdmEscolas.Any(e => e.Id == id);
        }
    }
}
