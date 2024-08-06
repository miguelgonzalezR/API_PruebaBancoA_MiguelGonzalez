using API_PruebaBancoA_MiguelGonzalez.Data;
using API_PruebaBancoA_MiguelGonzalez.DTO;
using API_PruebaBancoA_MiguelGonzalez.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace API_PruebaBancoA_MiguelGonzalez.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ComprasController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Compras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComprasDTO>>> GetCompras()
        {
            var compras = await _context.Compras.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<ComprasDTO>>(compras));
        }

        // GET: api/Compras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ComprasDTO>> GetCompra(int id)
        {
            var compra = await _context.Compras.FindAsync(id);

            if (compra == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ComprasDTO>(compra));
        }

        [HttpGet("Tarjeta/{tarjetaId}")]
        public async Task<ActionResult> GetComprasByTarjetaId(int tarjetaId)
        {
            var tarjeta = await _context.Tarjetas
                .Include(t => t.Compras)
                .Include(t => t.Pagos)
                .FirstOrDefaultAsync(t => t.Id == tarjetaId);

            if (tarjeta == null)
            {
                return NotFound();
            }

            // Cálculos
            var saldoTotal = tarjeta.Compras.Sum(c => c.Monto) - tarjeta.Pagos.Sum(p => p.Monto);
            var porcentajeInteresConfigurable = 0.25m;
            var porcentajeConfigurableSaldoMinimo = 0.05m;
            var interesBonificable = saldoTotal * porcentajeInteresConfigurable;
            var cuotaMinimaAPagar = saldoTotal * porcentajeConfigurableSaldoMinimo;
            var montoTotalAPagar = saldoTotal;
            var pagoContadoConIntereses = saldoTotal + interesBonificable;

            // Compras del mes actual y anterior
            var comprasMesActual = tarjeta.Compras.Where(c => c.FechaCompra.Month == DateTime.Now.Month && c.FechaCompra.Year == DateTime.Now.Year).ToList();
            var comprasMesAnterior = tarjeta.Compras.Where(c => c.FechaCompra.Month == DateTime.Now.AddMonths(-1).Month && c.FechaCompra.Year == DateTime.Now.AddMonths(-1).Year).ToList();

            var totalComprasMesActual = comprasMesActual.Sum(c => c.Monto);
            var totalComprasMesAnterior = comprasMesAnterior.Sum(c => c.Monto);

            var estadoCuenta = new
            {
                tarjeta.Nombre,
                tarjeta.Apellido,
                tarjeta.Trajeta,
                tarjeta.LimiteCredito,
                SaldoActual = saldoTotal,
                SaldoDisponible = tarjeta.LimiteCredito - saldoTotal,
                InteresBonificable = interesBonificable,
                CuotaMinimaAPagar = cuotaMinimaAPagar,
                MontoTotalAPagar = montoTotalAPagar,
                PagoContadoConIntereses = pagoContadoConIntereses,
                ComprasMesActual = comprasMesActual.Select(c => new { c.Id, c.FechaCompra, c.Descripcion, c.Monto }),
                TotalComprasMesActual = totalComprasMesActual,
                TotalComprasMesAnterior = totalComprasMesAnterior
            };

            return Ok(estadoCuenta);
        }

        [HttpGet("TotalPorMesYAnio/Tarjeta/{tarjetaId}")]
        public async Task<ActionResult> GetTotalPorMesYAnio(int tarjetaId)
        {
            DateTime now = DateTime.Now;
            DateTime firstDayOfCurrentMonth = new DateTime(now.Year, now.Month, 1);
            DateTime firstDayOfPreviousMonth = firstDayOfCurrentMonth.AddMonths(-1);

            var totalPorMesYAnio = await _context.Compras
                .Where(c => c.TarjetaId == tarjetaId && c.FechaCompra >= firstDayOfPreviousMonth && c.FechaCompra < firstDayOfCurrentMonth.AddMonths(1))
                .GroupBy(c => new { c.FechaCompra.Year, c.FechaCompra.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalMonto = g.Sum(c => c.Monto)
                })
                .ToListAsync();

            var result = totalPorMesYAnio.Select(g => new
            {
                Año = g.Year,
                Mes = GetMonthName(g.Month),
                g.TotalMonto
            })
            .OrderBy(r => r.Año)
            .ThenBy(r => DateTime.ParseExact(r.Mes, "MMMM", CultureInfo.CreateSpecificCulture("es")).Month)
            .ToList();

            return Ok(result);
        }

        // POST: api/Compras
        [HttpPost]
        public async Task<ActionResult<ComprasDTO>> PostCompra([FromBody] ComprasDTO rcompraDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tarjeta = await _context.Tarjetas.FindAsync(rcompraDTO.TarjetaId);
            if (tarjeta == null)
            {
                return BadRequest("Tarjeta no encontrada.");
            }

            var rcompra = _mapper.Map<Compras>(rcompraDTO);
            rcompra.Id = await GenerateUniqueNumberAsync();

            _context.Compras.Add(rcompra);
            await _context.SaveChangesAsync();

            await ActualizarSaldosAsync(rcompra.TarjetaId);

            return CreatedAtAction(nameof(GetCompra), new { id = rcompra.Id }, _mapper.Map<ComprasDTO>(rcompra));
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

        private string GetMonthName(int monthNumber)
        {
            return new DateTime(1, monthNumber, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("es"));
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
