using Biblioteca.Web.Aplicacao.Repositorios;
using Biblioteca.Web.Infraestrutura.Observabilidade;
using Biblioteca.Web.Models;

namespace Biblioteca.Web.Aplicacao.Servicos;

/// <summary>
/// Camada de aplicação responsável pelas regras de orquestração de Livro:
/// delega persistência ao repositório e produz logs/spans/métricas de observabilidade.
/// </summary>
public class LivroServico : ILivroServico
{
    private readonly ILivroRepositorio _repositorio;
    private readonly ILogger<LivroServico> _logger;

    public LivroServico(ILivroRepositorio repositorio, ILogger<LivroServico> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public Task<IEnumerable<Livro>> ListarComAutorAsync() => _repositorio.ObterTodosComAutorAsync();

    public Task<Livro?> ObterAsync(int id) => _repositorio.ObterPorIdAsync(id);

    public Task<Livro?> ObterComAutorAsync(int id) => _repositorio.ObterPorIdComAutorAsync(id);

    public async Task<Livro> CriarAsync(string titulo, decimal preco, int quantidadeEstoque, int autorId)
    {
        using var atividade = AplicacaoMetricas.ActivitySource.StartActivity("CriarLivro");
        atividade?.SetTag("livro.titulo", titulo);
        atividade?.SetTag("livro.autorId", autorId);

        var livro = new Livro(titulo, preco, quantidadeEstoque, autorId);
        await _repositorio.AdicionarAsync(livro);

        AplicacaoMetricas.ContadorOperacoes.Add(
            1,
            new KeyValuePair<string, object?>("operacao", "criar_livro"),
            new KeyValuePair<string, object?>("status", "sucesso"));

        _logger.LogInformation("Livro {Titulo} (Id={LivroId}) criado para o autor {AutorId}.", livro.Titulo, livro.Id, autorId);

        return livro;
    }

    public async Task AtualizarAsync(int id, string titulo, decimal preco, int quantidadeEstoque, int autorId)
    {
        using var atividade = AplicacaoMetricas.ActivitySource.StartActivity("AtualizarLivro");
        atividade?.SetTag("livro.id", id);

        var livro = await _repositorio.ObterPorIdAsync(id)
            ?? throw new InvalidOperationException($"Livro com Id={id} não foi encontrado.");

        livro.Atualizar(titulo, preco, quantidadeEstoque, autorId);
        await _repositorio.AtualizarAsync(livro);

        AplicacaoMetricas.ContadorOperacoes.Add(
            1,
            new KeyValuePair<string, object?>("operacao", "atualizar_livro"),
            new KeyValuePair<string, object?>("status", "sucesso"));

        _logger.LogInformation("Livro {LivroId} atualizado com sucesso.", id);
    }

    public async Task RemoverAsync(int id)
    {
        using var atividade = AplicacaoMetricas.ActivitySource.StartActivity("RemoverLivro");
        atividade?.SetTag("livro.id", id);

        await _repositorio.RemoverAsync(id);

        AplicacaoMetricas.ContadorOperacoes.Add(
            1,
            new KeyValuePair<string, object?>("operacao", "remover_livro"),
            new KeyValuePair<string, object?>("status", "sucesso"));

        _logger.LogInformation("Livro {LivroId} removido com sucesso.", id);
    }
}
