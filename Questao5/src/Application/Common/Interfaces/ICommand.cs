using MediatR;

namespace Ailos.Application.Common.Interfaces;

// Interface marker para identificar Commands
public interface ICommand<out TResponse> : IRequest<TResponse> { }
