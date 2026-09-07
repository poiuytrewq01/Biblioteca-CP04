using Biblioteca.Web.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Biblioteca.Web.Infraestrutura.Health;

/// <summary>
/// Health check customizado que valida a conexão com o banco de dados Oracle
/// utilizado pela aplicação, exposto no endpoint nativo /health.
/// </summary>
public class BancoDadosHealthCheck : IHealthCheck
{
    private readonly BibliotecaContext _contexto;
    private readonly ILogger<BancoDadosHealthCheck> _logger;

    public BancoDadosHealthCheck(BibliotecaContext contexto, ILogger<BancoDadosHealthCheck> logger)
    {
        _contexto = contexto;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var conseguiuConectar = await _contexto.Database.CanConnectAsync(cancellationToken);

            if (conseguiuConectar)
            {
                return HealthCheckResult.Healthy("Conexão com o banco de dados Oracle estabelecida com sucesso.");
            }

            _logger.LogWarning("Health check reportou falha ao conectar no banco de dados Oracle.");
            return HealthCheckResult.Unhealthy("Não foi possível conectar ao banco de dados Oracle.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao verificar a saúde da conexão com o banco de dados Oracle.");
            return HealthCheckResult.Unhealthy("Erro ao verificar a conexão com o banco de dados Oracle.", ex);
        }
    }
}
