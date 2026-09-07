using Biblioteca.Web.Aplicacao.Repositorios;
using Biblioteca.Web.Aplicacao.Servicos;
using Biblioteca.Web.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Biblioteca.Tests.Unit.Servicos;

public class LivroServicoTests
{
    private readonly Mock<ILivroRepositorio> _repositorioMock;
    private readonly LivroServico _servico;

    public LivroServicoTests()
    {
        _repositorioMock = new Mock<ILivroRepositorio>();
        _servico = new LivroServico(_repositorioMock.Object, NullLogger<LivroServico>.Instance);
    }

    [Fact]
    public async Task Deve_Criar_Livro_E_Chamar_Repositorio_Uma_Vez()
    {
        // Arrange
        _repositorioMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Livro>()))
            .Returns(Task.CompletedTask);

        // Act
        var livroCriado = await _servico.CriarAsync("Título Teste", 25.5m, 3, 1);

        // Assert
        livroCriado.Titulo.Should().Be("Título Teste");
        livroCriado.AutorId.Should().Be(1);
        _repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Livro>()), Times.Once);
    }

    [Fact]
    public async Task Deve_Listar_Livros_Delegando_Para_O_Repositorio()
    {
        // Arrange
        var livrosEsperados = new List<Livro>
        {
            new("Livro 1", 10m, 1, 1),
            new("Livro 2", 20m, 2, 1),
        };

        _repositorioMock
            .Setup(r => r.ObterTodosComAutorAsync())
            .ReturnsAsync(livrosEsperados);

        // Act
        var resultado = await _servico.ListarComAutorAsync();

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().BeEquivalentTo(livrosEsperados);
        _repositorioMock.Verify(r => r.ObterTodosComAutorAsync(), Times.Once);
    }

    [Fact]
    public async Task Deve_Atualizar_Livro_Existente_E_Chamar_Repositorio_Uma_Vez()
    {
        // Arrange
        var livroExistente = new Livro("Título Original", 15m, 4, 1);

        _repositorioMock
            .Setup(r => r.ObterPorIdAsync(1))
            .ReturnsAsync(livroExistente);
        _repositorioMock
            .Setup(r => r.AtualizarAsync(It.IsAny<Livro>()))
            .Returns(Task.CompletedTask);

        // Act
        await _servico.AtualizarAsync(1, "Título Atualizado", 18m, 6, 2);

        // Assert
        _repositorioMock.Verify(r => r.ObterPorIdAsync(1), Times.Once);
        _repositorioMock.Verify(r => r.AtualizarAsync(It.IsAny<Livro>()), Times.Once);
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Ao_Atualizar_Livro_Inexistente()
    {
        // Arrange
        _repositorioMock
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Livro?)null);

        // Act
        Func<Task> acao = async () => await _servico.AtualizarAsync(99, "Título", 10m, 1, 1);

        // Assert
        await acao.Should().ThrowAsync<InvalidOperationException>();
        _repositorioMock.Verify(r => r.AtualizarAsync(It.IsAny<Livro>()), Times.Never);
    }

    [Fact]
    public async Task Deve_Remover_Livro_Chamando_Repositorio_Uma_Vez()
    {
        // Arrange
        _repositorioMock
            .Setup(r => r.RemoverAsync(1))
            .Returns(Task.CompletedTask);

        // Act
        await _servico.RemoverAsync(1);

        // Assert
        _repositorioMock.Verify(r => r.RemoverAsync(1), Times.Once);
    }
}
