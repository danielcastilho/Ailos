using Ailos.Application.Common.Models;

namespace Ailos.Domain.Exceptions
{
    public class BusinessException : Exception
    {
        public ErrorTypesEnum Type { get; }

        public BusinessException(string message, ErrorTypesEnum type)
            : base(message)
        {
            Type = type;
        }
    }
}
