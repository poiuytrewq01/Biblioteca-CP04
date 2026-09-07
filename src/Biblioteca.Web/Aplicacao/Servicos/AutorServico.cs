using Biblioteca.Web.Aplicacao.Repositorios;
using Biblioteca.Web.Infraestrutura.Observabilidade;
using Biblioteca.Web.Models;

namespace Biblioteca.Web.Aplicacao.Servicos;

/// <summary>
/// Camada de aplicação responsável pelas regras de orquestração de Autor:
/// delega persistência ao repositório e produz logs/spans/métricas de observabilidade.
/// </summary>
public class AutorServico : IAutorServico
{
    private readonly IAutorRepositorio _repositorio;
    private readonly ILogger<AutorServico> _logger;

    public AutorServico(IAutorRepositorio repositorio, ILogger<AutorServico> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public Task<IEnumerable<Autor>> ListarComLivrosAsync() => _repositorio.ObterTodosComLivrosAsync();

    public Task<Autor?> ObterAsync(int id) => _repositorio.ObterPorIdAsync(id);

    public Task<Autor?> ObterComLivrosAsync(int id) => _repositorio.ObterPorIdComLivrosAsync(id);

    public async Task<Autor> CriarAsync(string nome, string? nacionalidade)
    {
        using var atividade = AplicacaoMetricas.ActivitySource.StartActivity("CriarAutor");
        atividade?.SetTag("autor.nome", nome);

        var autor = new Autor(nome, nacionalidade);
        await _repositorio.AdicionarAsync(autor);

        AplicacaoMetricas.ContadorOperacoes.Add(
            1,
            new KeyValuePair<string, object?>("operacao", "criar_autor"),
            new KeyValuePair<string, object?>("status", "sucesso"));

        _logger.LogInformation("Autor {Nome} (Id={AutorId}) criado com sucesso.", autor.Nome, autor.Id);

        return autor;
    }

    public async Task AtualizarAsync(int id, string nome, string? nacionalidade)
    {
        using var atividade = AplicacaoMetricas.ActivitySource.StartActivity("AtualizarAutor");
        atividade?.SetTag("autor.id", id);

        var autor = await _repositorio.ObterPorIdAsync(id)
            ?? throw new InvalidOperationException($"Autor com Id={id} não foi encontrado.");

        autor.Atualizar(nome, nacionalidade);
        await _repositorio.AtualizarAsync(autor);

        AplicacaoMetricas.ContadorOperacoes.Add(
            1,
            new KeyValuePair<string, object?>("operacao", "atualizar_autor"),
            new KeyValuePair<string, object?>("status", "sucesso"));

        _logger.LogInformation("Autor {AutorId} atualizado com sucesso.", id);
    }

    public async Task RemoverAsync(int id)
    {
        using var atividade = AplicacaoMetricas.ActivitySource.StartActivity("RemoverAutor");
        atividade?.SetTag("autor.id", id);

        await _repositorio.RemoverAsync(id);

        AplicacaoMetricas.ContadorOperacoes.Add(
            1,
            new KeyValuePair<string, object?>("operacao", "remover_autor"),
            new KeyValuePair<string, object?>("status", "sucesso"));

        _logger.LogInformation("Autor {AutorId} removido com sucesso.", id);
    }
}
