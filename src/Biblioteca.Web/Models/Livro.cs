namespace Biblioteca.Web.Models;

/// <summary>
/// Representa um livro da biblioteca. Cada livro pertence a um único autor (relacionamento 1:N).
/// </summary>
public class Livro
{
    public int Id { get; private set; }

    public string Titulo { get; private set; } = string.Empty;

    public decimal Preco { get; private set; }

    public int QuantidadeEstoque { get; private set; }

    public int AutorId { get; private set; }

    public Autor? Autor { get; private set; }

    /// <summary>
    /// Construtor exigido pelo Entity Framework Core. Não deve ser usado diretamente.
    /// </summary>
    protected Livro()
    {
    }

    public Livro(string titulo, decimal preco, int quantidadeEstoque, int autorId)
    {
        ValidarDados(titulo, preco, quantidadeEstoque, autorId);

        Titulo = titulo.Trim();
        Preco = preco;
        QuantidadeEstoque = quantidadeEstoque;
        AutorId = autorId;
    }

    public void Atualizar(string titulo, decimal preco, int quantidadeEstoque, int autorId)
    {
        ValidarDados(titulo, preco, quantidadeEstoque, autorId);

        Titulo = titulo.Trim();
        Preco = preco;
        QuantidadeEstoque = quantidadeEstoque;
        AutorId = autorId;
    }

    private static void ValidarDados(string titulo, decimal preco, int quantidadeEstoque, int autorId)
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new ArgumentException("O título do livro não pode ser vazio.", nameof(titulo));
        }

        if (preco <= 0)
        {
            throw new ArgumentException("O preço do livro deve ser maior que zero.", nameof(preco));
        }

        if (quantidadeEstoque < 0)
        {
            throw new ArgumentException("A quantidade em estoque não pode ser negativa.", nameof(quantidadeEstoque));
        }

        if (autorId <= 0)
        {
            throw new ArgumentException("O livro precisa estar associado a um autor válido.", nameof(autorId));
        }
    }
}
