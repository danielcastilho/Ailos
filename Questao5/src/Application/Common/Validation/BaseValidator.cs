using System.Linq.Expressions;
using FluentValidation;

namespace Ailos.Application.Common.Validation
{
    public abstract class BaseValidator<T> : AbstractValidator<T>
    {
        protected void ValidateRequired(Expression<Func<T, string>> expression, string fieldName)
        {
            RuleFor(expression).NotEmpty().WithMessage(ValidationMessages.Required(fieldName));
        }

        protected void ValidateGuid(Expression<Func<T, string>> expression, string fieldName)
        {
            RuleFor(expression)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required(fieldName))
                .Must(BeAValidGuid)
                .WithMessage(ValidationMessages.InvalidGuid(fieldName));
        }

        protected void ValidateMaxLength(
            Expression<Func<T, string>> expression,
            string fieldName,
            int maxLength
        )
        {
            RuleFor(expression)
                .MaximumLength(maxLength)
                .WithMessage(ValidationMessages.MaxLength(fieldName, maxLength));
        }

        protected void ValidateDecimalRange(
            Expression<Func<T, decimal>> expression,
            string fieldName,
            decimal min,
            decimal max
        )
        {
            RuleFor(expression)
                .InclusiveBetween(min, max)
                .WithMessage(ValidationMessages.Range(fieldName, min, max));
        }

        private bool BeAValidGuid(string value)
        {
            return !string.IsNullOrEmpty(value) && Guid.TryParse(value, out _);
        }
    }
}
