using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_PruebaBancoA_MiguelGonzalez.Data;
using API_PruebaBancoA_MiguelGonzalez.Models;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using API_PruebaBancoA_MiguelGonzalez.DTO;

namespace API_PruebaBancoA_MiguelGonzalez.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarjetasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TarjetasController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Tarjetas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TarjetaDTO>>> GetTarjetas()
        {
            var tarjetas = await _context.Tarjetas.ToListAsync();
            var tarjetaDTOs = _mapper.Map<List<TarjetaDTO>>(tarjetas);
            return Ok(tarjetaDTOs);
        }

        // GET: api/Tarjetas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TarjetaDTO>> GetTarjeta(int id)
        {
            var tarjeta = await _context.Tarjetas.FindAsync(id);

            if (tarjeta == null)
            {
                return NotFound();
            }

            var tarjetaDTO = _mapper.Map<TarjetaDTO>(tarjeta);
            return Ok(tarjetaDTO);
        }

        // POST: api/Tarjetas
        [HttpPost]
        public async Task<ActionResult<TarjetaDTO>> PostTarjeta(TarjetaDTO tarjetaDTO)
        {
            var tarjeta = _mapper.Map<Tarjetas>(tarjetaDTO);
            _context.Tarjetas.Add(tarjeta);
            await _context.SaveChangesAsync();

            var newTarjetaDTO = _mapper.Map<TarjetaDTO>(tarjeta);
            return CreatedAtAction(nameof(GetTarjeta), new { id = newTarjetaDTO.Id }, newTarjetaDTO);
        }



        private bool TarjetaExists(int id)
        {
            return _context.Tarjetas.Any(e => e.Id == id);
        }
    }
}
