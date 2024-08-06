using API_PruebaBancoA_MiguelGonzalez.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_PruebaBancoA_MiguelGonzalez.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovimientosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MovimientosController
        [HttpGet("Tarjeta/{tarjetaId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetMovimientosByTarjetaId(int tarjetaId)
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var compras = await _context.Compras
                                        .Where(c => c.TarjetaId == tarjetaId && c.FechaCompra.Month == currentMonth && c.FechaCompra.Year == currentYear)
                                        .Select(c => new
                                        {
                                            c.Id,
                                            c.TarjetaId,
                                            c.FechaCompra,
                                            c.Monto,
                                            c.Descripcion,
                                            Tipo = "Compra"
                                        })
                                        .ToListAsync();

            var pagos = await _context.Pago
                                      .Where(p => p.TarjetaId == tarjetaId && p.FechaCompra.Month == currentMonth && p.FechaCompra.Year == currentYear)
                                      .Select(p => new
                                      {
                                          p.Id,
                                          p.TarjetaId,
                                          p.FechaCompra,
                                          p.Monto,
                                          p.Descripcion,
                                          Tipo = "Pago"
                                      })
                                      .ToListAsync();

            var movimientos = compras.Concat(pagos)
                                     .OrderBy(m => m.FechaCompra)
                                     .ToList();

            return Ok(movimientos);
        }

    }
}
