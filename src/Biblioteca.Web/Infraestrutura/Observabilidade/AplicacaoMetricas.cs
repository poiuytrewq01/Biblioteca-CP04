using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Biblioteca.Web.Infraestrutura.Observabilidade;

/// <summary>
/// Fonte central de métricas e traces customizados da aplicação Biblioteca.API,
/// usada pelo OpenTelemetry (Meter/Counter para métricas, ActivitySource para spans).
/// </summary>
public static class AplicacaoMetricas
{
    public const string NomeServico = "Biblioteca.API";

    private static readonly Meter Meter = new(NomeServico);

    /// <summary>
    /// Contador de operações de cadastro (criação/atualização/exclusão) realizadas
    /// nas entidades Autor e Livro, dimensionado por tags de operação e status.
    /// </summary>
    public static readonly Counter<long> ContadorOperacoes = Meter.CreateCounter<long>(
        name: "biblioteca.operacoes.total",
        unit: "operacoes",
        description: "Contagem total de operações de cadastro realizadas na Biblioteca");

    /// <summary>
    /// ActivitySource usado para criar spans manuais de tracing nos pontos de escrita da aplicação.
    /// </summary>
    public static readonly ActivitySource ActivitySource = new(NomeServico);
}
