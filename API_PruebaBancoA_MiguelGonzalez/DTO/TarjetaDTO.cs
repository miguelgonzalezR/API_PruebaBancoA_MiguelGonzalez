namespace API_PruebaBancoA_MiguelGonzalez.DTO
{
    public class TarjetaDTO
    {
        public int Id { get; set; }
        public string Trajeta { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public decimal LimiteCredito { get; set; }
        public decimal SaldoActual { get; set; }
        public decimal SaldoDisponible { get; set; }

    }
}
