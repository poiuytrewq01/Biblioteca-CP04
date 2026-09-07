using Biblioteca.Web.Models;
using FluentAssertions;
using Xunit;

namespace Biblioteca.Tests.Unit.Dominio;

public class AutorTests
{
    [Fact]
    public void Deve_Criar_Autor_Com_Dados_Validos()
    {
        // Arrange
        var nome = "Machado de Assis";
        var nacionalidade = "Brasileiro";

        // Act
        var autor = new Autor(nome, nacionalidade);

        // Assert
        autor.Nome.Should().Be(nome);
        autor.Nacionalidade.Should().Be(nacionalidade);
        autor.Livros.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_Lancar_Excecao_Ao_Criar_Autor_Com_Nome_Invalido(string? nomeInvalido)
    {
        // Arrange
        Action acao = () => new Autor(nomeInvalido!, "Brasileiro");

        // Act & Assert
        acao.Should().Throw<ArgumentException>()
            .WithMessage("*nome do autor não pode ser vazio*");
    }

    [Fact]
    public void Deve_Atualizar_Dados_Do_Autor_Com_Sucesso()
    {
        // Arrange
        var autor = new Autor("Nome Antigo", "Brasileiro");

        // Act
        autor.Atualizar("Nome Novo", "Português");

        // Assert
        autor.Nome.Should().Be("Nome Novo");
        autor.Nacionalidade.Should().Be("Português");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Deve_Lancar_Excecao_Ao_Atualizar_Autor_Com_Nome_Invalido(string nomeInvalido)
    {
        // Arrange
        var autor = new Autor("Nome Válido", "Brasileiro");
        Action acao = () => autor.Atualizar(nomeInvalido, "Brasileiro");

        // Act & Assert
        acao.Should().Throw<ArgumentException>();
    }
}
