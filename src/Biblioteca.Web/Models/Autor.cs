namespace Biblioteca.Web.Models;

/// <summary>
/// Representa um autor da biblioteca. Um autor pode ter vários livros (relacionamento 1:N).
/// </summary>
public class Autor
{
    public int Id { get; private set; }

    public string Nome { get; private set; } = string.Empty;

    public string? Nacionalidade { get; private set; }

    public ICollection<Livro> Livros { get; private set; } = new List<Livro>();

    /// <summary>
    /// Construtor exigido pelo Entity Framework Core. Não deve ser usado diretamente.
    /// </summary>
    protected Autor()
    {
    }

    public Autor(string nome, string? nacionalidade = null)
    {
        ValidarNome(nome);

        Nome = nome.Trim();
        Nacionalidade = nacionalidade;
    }

    public void Atualizar(string nome, string? nacionalidade)
    {
        ValidarNome(nome);

        Nome = nome.Trim();
        Nacionalidade = nacionalidade;
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do autor não pode ser vazio.", nameof(nome));
        }
    }
}
