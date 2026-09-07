using Biblioteca.Web.Models;
using FluentAssertions;
using Xunit;

namespace Biblioteca.Tests.Unit.Dominio;

public class LivroTests
{
    [Fact]
    public void Deve_Criar_Livro_Com_Dados_Validos()
    {
        // Arrange
        var titulo = "Dom Casmurro";
        var preco = 39.90m;
        var quantidadeEstoque = 10;
        var autorId = 1;

        // Act
        var livro = new Livro(titulo, preco, quantidadeEstoque, autorId);

        // Assert
        livro.Titulo.Should().Be(titulo);
        livro.Preco.Should().Be(preco);
        livro.QuantidadeEstoque.Should().Be(quantidadeEstoque);
        livro.AutorId.Should().Be(autorId);
    }

    [Theory]
    [InlineData("", 10.0, 1, 1)]
    [InlineData("   ", 10.0, 1, 1)]
    [InlineData("Título Válido", 0, 1, 1)]
    [InlineData("Título Válido", -5.0, 1, 1)]
    [InlineData("Título Válido", 10.0, -1, 1)]
    [InlineData("Título Válido", 10.0, 1, 0)]
    public void Deve_Lancar_Excecao_Ao_Criar_Livro_Com_Dados_Invalidos(
        string titulo, decimal preco, int quantidadeEstoque, int autorId)
    {
        // Arrange
        Action acao = () => new Livro(titulo, preco, quantidadeEstoque, autorId);

        // Act & Assert
        acao.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Atualizar_Dados_Do_Livro_Com_Sucesso()
    {
        // Arrange
        var livro = new Livro("Título Antigo", 20m, 5, 1);

        // Act
        livro.Atualizar("Título Novo", 29.9m, 8, 2);

        // Assert
        livro.Titulo.Should().Be("Título Novo");
        livro.Preco.Should().Be(29.9m);
        livro.QuantidadeEstoque.Should().Be(8);
        livro.AutorId.Should().Be(2);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-10.5)]
    public void Deve_Lancar_Excecao_Ao_Atualizar_Livro_Com_Preco_Invalido(decimal precoInvalido)
    {
        // Arrange
        var livro = new Livro("Título Válido", 20m, 5, 1);
        Action acao = () => livro.Atualizar("Título Válido", precoInvalido, 5, 1);

        // Act & Assert
        acao.Should().Throw<ArgumentException>()
            .WithMessage("*preço do livro deve ser maior que zero*");
    }
}
