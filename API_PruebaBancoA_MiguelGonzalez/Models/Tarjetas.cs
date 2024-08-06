using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API_PruebaBancoA_MiguelGonzalez.Models;

namespace API_PruebaBancoA_MiguelGonzalez.Models
{
    public class Tarjetas
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Trajeta { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(50)]
        public string Apellido { get; set; }

        public decimal LimiteCredito { get; set; }

        [NotMapped]
        public decimal SaldoActual { get; set; }

        [NotMapped]
        public decimal SaldoDisponible { get; set; }


        public ICollection<Compras> Compras { get; set; }

        public ICollection<Pagos> Pagos { get; set; }

    }
}
