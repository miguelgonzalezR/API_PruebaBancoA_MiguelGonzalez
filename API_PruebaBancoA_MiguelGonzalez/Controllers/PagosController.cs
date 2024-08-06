using API_PruebaBancoA_MiguelGonzalez.Data;
using API_PruebaBancoA_MiguelGonzalez.DTO;
using API_PruebaBancoA_MiguelGonzalez.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_PruebaBancoA_MiguelGonzalez.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PagosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Pagos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PagosDTO>>> GetPagos()
        {
            var pagos = await _context.Pago.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<PagosDTO>>(pagos));
        }

        // GET: api/Pagos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PagosDTO>> GetPagos(int id)
        {
            var pago = await _context.Pago.FindAsync(id);

            if (pago == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PagosDTO>(pago));
        }

        [HttpGet("Tarjeta/{tarjetaId}")]
        public async Task<ActionResult<IEnumerable<PagosDTO>>> GetPagosByTarjetaId(int tarjetaId)
        {
            var pagos = await _context.Pago.Where(c => c.TarjetaId == tarjetaId).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<PagosDTO>>(pagos));
        }

        // POST: api/Pagos
        [HttpPost]
        public async Task<ActionResult<PagosDTO>> PostPagos([FromBody] PagosDTO RPagosDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tarjeta = await _context.Tarjetas.FindAsync(RPagosDTO.TarjetaId);
            if (tarjeta == null)
            {
                return BadRequest("Tarjeta no encontrada.");
            }

            var RPagos = _mapper.Map<Pagos>(RPagosDTO);
            RPagos.Id = await GenerateUniqueNumberAsync();

            _context.Pago.Add(RPagos);
            await _context.SaveChangesAsync();

            await ActualizarSaldosAsync(RPagos.TarjetaId);

            return CreatedAtAction(nameof(GetPagos), new { id = RPagos.Id }, _mapper.Map<PagosDTO>(RPagos));
        }


        private bool PagoExists(int id)
        {
            return _context.Pago.Any(e => e.Id == id);
        }

        private async Task<int> GenerateUniqueNumberAsync()
        {
            int randomNumber;
            bool isUnique;

            do
            {
                randomNumber = new Random().Next(100000, 999999); // Generar número aleatorio de 6 dígitos

                isUnique = !await _context.Pago.AnyAsync(p => p.Id == randomNumber) &&
                           !await _context.Compras.AnyAsync(c => c.Id == randomNumber);
            } while (!isUnique);

            return randomNumber;
        }

        private async Task ActualizarSaldosAsync(int tarjetaId)
        {
            var tarjeta = await _context.Tarjetas
                .Include(t => t.Compras)
                .Include(t => t.Pagos)
                .FirstOrDefaultAsync(t => t.Id == tarjetaId);

            if (tarjeta != null)
            {
                tarjeta.SaldoActual = tarjeta.Compras.Sum(c => c.Monto) - tarjeta.Pagos.Sum(p => p.Monto);
                tarjeta.SaldoDisponible = tarjeta.LimiteCredito - tarjeta.SaldoActual;

                _context.Tarjetas.Update(tarjeta);
                await _context.SaveChangesAsync();
            }
        }
    }
}
