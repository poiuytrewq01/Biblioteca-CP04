using Biblioteca.Web.Models;

namespace Biblioteca.Web.Aplicacao.Repositorios;

public interface ILivroRepositorio
{
    Task<IEnumerable<Livro>> ObterTodosComAutorAsync();

    Task<Livro?> ObterPorIdAsync(int id);

    Task<Livro?> ObterPorIdComAutorAsync(int id);

    Task AdicionarAsync(Livro livro);

    Task AtualizarAsync(Livro livro);

    Task RemoverAsync(int id);
}
