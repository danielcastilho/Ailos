using MediatR;

namespace Ailos.Application.Common.Interfaces;

// Interface marker para identificar Queries
public interface IQuery<out TResponse> : IRequest<TResponse> { }
