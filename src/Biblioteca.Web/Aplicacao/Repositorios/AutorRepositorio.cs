using Biblioteca.Web.Data;
using Biblioteca.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Web.Aplicacao.Repositorios;

public class AutorRepositorio : IAutorRepositorio
{
    private readonly BibliotecaContext _contexto;

    public AutorRepositorio(BibliotecaContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<Autor>> ObterTodosComLivrosAsync()
    {
        return await _contexto.Autores
            .Include(a => a.Livros)
            .AsNoTracking()
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<Autor?> ObterPorIdAsync(int id)
    {
        return await _contexto.Autores.FindAsync(id);
    }

    public async Task<Autor?> ObterPorIdComLivrosAsync(int id)
    {
        return await _contexto.Autores
            .Include(a => a.Livros)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AdicionarAsync(Autor autor)
    {
        _contexto.Autores.Add(autor);
        await _contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Autor autor)
    {
        _contexto.Autores.Update(autor);
        await _contexto.SaveChangesAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var autor = await _contexto.Autores.FindAsync(id);

        if (autor is not null)
        {
            _contexto.Autores.Remove(autor);
            await _contexto.SaveChangesAsync();
        }
    }
}
