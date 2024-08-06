using API_PruebaBancoA_MiguelGonzalez.Models;
using FluentValidation;

namespace API_PruebaBancoA_MiguelGonzalez.Validaciones
{
    public class PagosValidator : AbstractValidator<Pagos>
    {   
        public PagosValidator()
        {
            RuleFor(p => p.TarjetaId).NotEmpty().WithMessage("El ID de la tarjeta es obligatorio.");
            RuleFor(p => p.FechaCompra).NotEmpty().WithMessage("La fecha de compra es obligatoria.");
            RuleFor(p => p.Monto).GreaterThan(0).WithMessage("El monto debe ser mayor a 0.");
            RuleFor(p => p.Descripcion).NotEmpty().WithMessage("La descripción es obligatoria.");
        }

    }
}
