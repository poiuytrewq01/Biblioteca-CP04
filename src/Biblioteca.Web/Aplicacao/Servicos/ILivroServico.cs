using Biblioteca.Web.Models;

namespace Biblioteca.Web.Aplicacao.Servicos;

public interface ILivroServico
{
    Task<IEnumerable<Livro>> ListarComAutorAsync();

    Task<Livro?> ObterAsync(int id);

    Task<Livro?> ObterComAutorAsync(int id);

    Task<Livro> CriarAsync(string titulo, decimal preco, int quantidadeEstoque, int autorId);

    Task AtualizarAsync(int id, string titulo, decimal preco, int quantidadeEstoque, int autorId);

    Task RemoverAsync(int id);
}
