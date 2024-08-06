using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_PruebaBancoA_MiguelGonzalez.Models
{
    [Table("Pagos")]
    public class Pagos
    {

        [Key]
        public int Id { get; set; }
        public int TarjetaId { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }

        public Tarjetas? Tarjeta { get; set; }

    }
}
