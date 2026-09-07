using Biblioteca.Web.Aplicacao.Servicos;
using Biblioteca.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers;

/// <summary>
/// Controller responsável pelo CRUD de Livros, sempre associados a um Autor.
/// </summary>
public class LivrosController : Controller
{
    private readonly ILivroServico _livroServico;
    private readonly IAutorServico _autorServico;
    private readonly ILogger<LivrosController> _logger;

    public LivrosController(ILivroServico livroServico, IAutorServico autorServico, ILogger<LivrosController> logger)
    {
        _livroServico = livroServico;
        _autorServico = autorServico;
        _logger = logger;
    }

    // GET: /Livros
    public async Task<IActionResult> Index()
    {
        var livros = await _livroServico.ListarComAutorAsync();
        return View(livros);
    }

    // GET: /Livros/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var livro = await _livroServico.ObterComAutorAsync(id);

        if (livro is null)
        {
            return NotFound();
        }

        return View(livro);
    }

    // GET: /Livros/Create
    public async Task<IActionResult> Create()
    {
        var modelo = new LivroFormViewModel
        {
            AutoresDisponiveis = await ObterAutoresDisponiveisAsync(),
        };

        return View(modelo);
    }

    // POST: /Livros/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LivroFormViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            modelo.AutoresDisponiveis = await ObterAutoresDisponiveisAsync();
            return View(modelo);
        }

        try
        {
            await _livroServico.CriarAsync(modelo.Titulo, modelo.Preco, modelo.QuantidadeEstoque, modelo.AutorId);
            TempData["MensagemSucesso"] = "Livro cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Falha de validação ao criar livro.");
            ModelState.AddModelError(string.Empty, ex.Message);
            modelo.AutoresDisponiveis = await ObterAutoresDisponiveisAsync();
            return View(modelo);
        }
    }

    // GET: /Livros/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var livro = await _livroServico.ObterAsync(id);

        if (livro is null)
        {
            return NotFound();
        }

        var modelo = new LivroFormViewModel
        {
            Id = livro.Id,
            Titulo = livro.Titulo,
            Preco = livro.Preco,
            QuantidadeEstoque = livro.QuantidadeEstoque,
            AutorId = livro.AutorId,
            AutoresDisponiveis = await ObterAutoresDisponiveisAsync(),
        };

        return View(modelo);
    }

    // POST: /Livros/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LivroFormViewModel modelo)
    {
        if (id != modelo.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            modelo.AutoresDisponiveis = await ObterAutoresDisponiveisAsync();
            return View(modelo);
        }

        try
        {
            await _livroServico.AtualizarAsync(id, modelo.Titulo, modelo.Preco, modelo.QuantidadeEstoque, modelo.AutorId);
            TempData["MensagemSucesso"] = "Livro atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Falha de validação ao atualizar livro {LivroId}.", id);
            ModelState.AddModelError(string.Empty, ex.Message);
            modelo.AutoresDisponiveis = await ObterAutoresDisponiveisAsync();
            return View(modelo);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    // GET: /Livros/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var livro = await _livroServico.ObterComAutorAsync(id);

        if (livro is null)
        {
            return NotFound();
        }

        return View(livro);
    }

    // POST: /Livros/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _livroServico.RemoverAsync(id);
        TempData["MensagemSucesso"] = "Livro removido com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IEnumerable<AutorOpcaoViewModel>> ObterAutoresDisponiveisAsync()
    {
        var autores = await _autorServico.ListarComLivrosAsync();

        return autores
            .Select(a => new AutorOpcaoViewModel { Id = a.Id, Nome = a.Nome })
            .ToList();
    }
}
