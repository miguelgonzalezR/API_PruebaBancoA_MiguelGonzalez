namespace API_PruebaBancoA_MiguelGonzalez.DTO
{
    public class PagosDTO
    {

        public int Id { get; set; }
        public int TarjetaId { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }

    }
}
