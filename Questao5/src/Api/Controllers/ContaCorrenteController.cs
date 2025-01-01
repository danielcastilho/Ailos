// Api/Controllers/ContaCorrenteController.cs
using System.Net.Mime;
using Ailos.Application.Commands.CreateMovimento;
using Ailos.Application.Common.Models;
using Ailos.Application.Queries.GetSaldoContaCorrente;
using Ailos.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ailos.Ailos.Api.Controllers
{
    [ApiController]
    [Route("api/contas-correntes")]
    [Produces(MediaTypeNames.Application.Json)]
    public class ContaCorrenteController : ControllerBase
    {
        // Definindo as dependências que serão injetadas
        private readonly IMediator _mediator;
        private readonly IValidator<CreateMovimentoRequest> _validator;
        private readonly ILogger<ContaCorrenteController> _logger;

        // Construtor com injeção de dependência
        public ContaCorrenteController(
            IMediator mediator,
            IValidator<CreateMovimentoRequest> validator,
            ILogger<ContaCorrenteController> logger
        )
        {
            _mediator = mediator;
            _validator = validator;
            _logger = logger;
        }

        /// <summary>
        /// Cria uma nova movimentação para uma conta corrente
        /// </summary>
        [HttpPost("{idContaCorrente}/movimentos")]
        [ProducesResponseType(typeof(CreateMovimentoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMovimento(
            [FromRoute] string idContaCorrente,
            [FromBody(
                EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Disallow
            )]
                CreateMovimentoRequest request
        )
        {
            // Log do corpo da requisição para diagnóstico
            _logger.LogInformation(
                "Recebendo requisição. IdContaCorrente: {IdContaCorrente}, Request: {@Request}",
                idContaCorrente,
                request
            );

            var command = new CreateMovimentoCommand
            {
                IdempotencyKey = request.IdempotencyKey,
                IdContaCorrente = idContaCorrente,
                Valor = request.Valor,
                TipoMovimento = request.TipoMovimento,
            };

            var response = await _mediator.Send(command);

            _logger.LogInformation(
                "Movimento criado com sucesso. ID: {MovimentoId}, Conta: {ContaId}",
                response.Id,
                idContaCorrente
            );

            return Ok(response);
        }

        /// <summary>
        /// Obtém o saldo atual de uma conta corrente
        /// </summary>
        [HttpGet("{idContaCorrente}/saldo")]
        [ProducesResponseType(typeof(GetSaldoContaCorrenteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSaldo([FromRoute] string idContaCorrente)
        {
            try
            {
                var query = new GetSaldoContaCorrenteQuery { IdContaCorrente = idContaCorrente };

                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Erro ao consultar saldo. Tipo: {ErrorType}, Conta: {ContaId}",
                    ex.Type,
                    idContaCorrente
                );

                return BadRequest(new ErrorResponse { Type = ex.Type, Message = ex.Message });
            }
        }
    }
}
