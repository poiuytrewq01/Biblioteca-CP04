using Biblioteca.Web.Models;

namespace Biblioteca.Web.Aplicacao.Repositorios;

public interface IAutorRepositorio
{
    Task<IEnumerable<Autor>> ObterTodosComLivrosAsync();

    Task<Autor?> ObterPorIdAsync(int id);

    Task<Autor?> ObterPorIdComLivrosAsync(int id);

    Task AdicionarAsync(Autor autor);

    Task AtualizarAsync(Autor autor);

    Task RemoverAsync(int id);
}
