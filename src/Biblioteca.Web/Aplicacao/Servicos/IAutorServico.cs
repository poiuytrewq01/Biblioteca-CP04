using Biblioteca.Web.Models;

namespace Biblioteca.Web.Aplicacao.Servicos;

public interface IAutorServico
{
    Task<IEnumerable<Autor>> ListarComLivrosAsync();

    Task<Autor?> ObterAsync(int id);

    Task<Autor?> ObterComLivrosAsync(int id);

    Task<Autor> CriarAsync(string nome, string? nacionalidade);

    Task AtualizarAsync(int id, string nome, string? nacionalidade);

    Task RemoverAsync(int id);
}
