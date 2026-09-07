using Serilog.Context;

namespace Biblioteca.Web.Aplicacao.Middlewares;

/// <summary>
/// Middleware responsável por ler (ou gerar, caso ausente) o cabeçalho X-Correlation-ID
/// de cada requisição e injetá-lo no contexto de log do Serilog, garantindo rastreabilidade
/// ponta a ponta entre requisição, resposta e logs.
/// </summary>
public class CorrelationIdMiddleware
{
    private const string CabecalhoCorrelationId = "X-Correlation-ID";

    private readonly RequestDelegate _proximo;

    public CorrelationIdMiddleware(RequestDelegate proximo)
    {
        _proximo = proximo;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ObterOuGerarCorrelationId(context);

        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(CabecalhoCorrelationId))
            {
                context.Response.Headers.Append(CabecalhoCorrelationId, correlationId);
            }

            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _proximo(context);
        }
    }

    private static string ObterOuGerarCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CabecalhoCorrelationId, out var valores))
        {
            var valor = valores.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(valor))
            {
                return valor;
            }
        }

        return Guid.NewGuid().ToString();
    }
}
