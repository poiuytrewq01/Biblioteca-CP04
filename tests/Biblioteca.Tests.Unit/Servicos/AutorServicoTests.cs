using Biblioteca.Web.Aplicacao.Repositorios;
using Biblioteca.Web.Aplicacao.Servicos;
using Biblioteca.Web.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Biblioteca.Tests.Unit.Servicos;

public class AutorServicoTests
{
    private readonly Mock<IAutorRepositorio> _repositorioMock;
    private readonly AutorServico _servico;

    public AutorServicoTests()
    {
        _repositorioMock = new Mock<IAutorRepositorio>();
        _servico = new AutorServico(_repositorioMock.Object, NullLogger<AutorServico>.Instance);
    }

    [Fact]
    public async Task Deve_Criar_Autor_E_Chamar_Repositorio_Uma_Vez()
    {
        // Arrange
        _repositorioMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Autor>()))
            .Returns(Task.CompletedTask);

        // Act
        var autorCriado = await _servico.CriarAsync("Clarice Lispector", "Brasileira");

        // Assert
        autorCriado.Nome.Should().Be("Clarice Lispector");
        _repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Autor>()), Times.Once);
    }

    [Fact]
    public async Task Deve_Remover_Autor_Chamando_Repositorio_Uma_Vez()
    {
        // Arrange
        _repositorioMock
            .Setup(r => r.RemoverAsync(5))
            .Returns(Task.CompletedTask);

        // Act
        await _servico.RemoverAsync(5);

        // Assert
        _repositorioMock.Verify(r => r.RemoverAsync(5), Times.Once);
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Ao_Atualizar_Autor_Inexistente()
    {
        // Arrange
        _repositorioMock
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Autor?)null);

        // Act
        Func<Task> acao = async () => await _servico.AtualizarAsync(42, "Nome", "Nacionalidade");

        // Assert
        await acao.Should().ThrowAsync<InvalidOperationException>();
        _repositorioMock.Verify(r => r.AtualizarAsync(It.IsAny<Autor>()), Times.Never);
    }
}
