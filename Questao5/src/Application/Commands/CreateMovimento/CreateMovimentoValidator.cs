using Ailos.Application.Common.Validation;
using Ailos.Domain.Enumerators;
using FluentValidation;

namespace Ailos.Application.Commands.CreateMovimento
{
    public class CreateMovimentoValidator : BaseValidator<CreateMovimentoRequest>
    {
        public CreateMovimentoValidator()
        {
            RuleFor(request => request)
                .NotNull()
                .WithMessage("O conteúdo da requisição não pode ser nulo");
            RuleFor(request => request.IdempotencyKey)
                .NotEmpty()
                .WithMessage("A chave de idempotência é obrigatória")
                .Must(BeAValidGuid)
                .WithMessage("A chave de idempotência deve ser um GUID válido");

            RuleFor(request => request.Valor)
                .NotEmpty()
                .WithMessage("O valor é obrigatório")
                .PrecisionScale(18, 2, false)
                .WithMessage("O valor deve ter no máximo 2 casas decimais");

            RuleFor(request => request.TipoMovimento)
                .IsInEnum()
                .WithMessage("Tipo de movimento deve ser 'C' para Crédito ou 'D' para Débito")
                .Must(tipo => tipo == TipoMovimento.Credito || tipo == TipoMovimento.Debito)
                .WithMessage("Tipo de movimento inválido. Use 'C' para Crédito ou 'D' para Débito");
        }

        private bool BeAValidGuid(string value) => Guid.TryParse(value, out _);
    }
}
