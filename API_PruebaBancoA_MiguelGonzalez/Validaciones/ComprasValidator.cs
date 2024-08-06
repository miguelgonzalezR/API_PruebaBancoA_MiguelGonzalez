using API_PruebaBancoA_MiguelGonzalez.Models;
using FluentValidation;

namespace API_PruebaBancoA_MiguelGonzalez.Validaciones
{
    public class ComprasValidator : AbstractValidator<Pagos>
    {
        public ComprasValidator()
        {
            RuleFor(c => c.TarjetaId).NotEmpty().WithMessage("El ID de la tarjeta es obligatorio.");
            RuleFor(c => c.FechaCompra).NotEmpty().WithMessage("La fecha de compra es obligatoria.");
            RuleFor(c => c.Monto).GreaterThan(0).WithMessage("El monto debe ser mayor a 0.");
            RuleFor(c => c.Descripcion).NotEmpty().WithMessage("La descripción es obligatoria.");
        }

    }
}
