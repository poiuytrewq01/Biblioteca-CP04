using Biblioteca.Web.Data;
using Biblioteca.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Web.Aplicacao.Repositorios;

public class LivroRepositorio : ILivroRepositorio
{
    private readonly BibliotecaContext _contexto;

    public LivroRepositorio(BibliotecaContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<Livro>> ObterTodosComAutorAsync()
    {
        return await _contexto.Livros
            .Include(l => l.Autor)
            .AsNoTracking()
            .OrderBy(l => l.Titulo)
            .ToListAsync();
    }

    public async Task<Livro?> ObterPorIdAsync(int id)
    {
        return await _contexto.Livros.FindAsync(id);
    }

    public async Task<Livro?> ObterPorIdComAutorAsync(int id)
    {
        return await _contexto.Livros
            .Include(l => l.Autor)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task AdicionarAsync(Livro livro)
    {
        _contexto.Livros.Add(livro);
        await _contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Livro livro)
    {
        _contexto.Livros.Update(livro);
        await _contexto.SaveChangesAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var livro = await _contexto.Livros.FindAsync(id);

        if (livro is not null)
        {
            _contexto.Livros.Remove(livro);
            await _contexto.SaveChangesAsync();
        }
    }
}
