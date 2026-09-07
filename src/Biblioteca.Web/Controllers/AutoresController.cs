using Biblioteca.Web.Aplicacao.Servicos;
using Biblioteca.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers;

/// <summary>
/// Controller responsável pelo CRUD de Autores e pela associação com os seus Livros.
/// </summary>
public class AutoresController : Controller
{
    private readonly IAutorServico _autorServico;
    private readonly ILogger<AutoresController> _logger;

    public AutoresController(IAutorServico autorServico, ILogger<AutoresController> logger)
    {
        _autorServico = autorServico;
        _logger = logger;
    }

    // GET: /Autores
    public async Task<IActionResult> Index()
    {
        var autores = await _autorServico.ListarComLivrosAsync();
        return View(autores);
    }

    // GET: /Autores/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var autor = await _autorServico.ObterComLivrosAsync(id);

        if (autor is null)
        {
            return NotFound();
        }

        return View(autor);
    }

    // GET: /Autores/Create
    public IActionResult Create()
    {
        return View(new AutorFormViewModel());
    }

    // POST: /Autores/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AutorFormViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        try
        {
            await _autorServico.CriarAsync(modelo.Nome, modelo.Nacionalidade);
            TempData["MensagemSucesso"] = "Autor cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Falha de validação ao criar autor.");
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(modelo);
        }
    }

    // GET: /Autores/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var autor = await _autorServico.ObterAsync(id);

        if (autor is null)
        {
            return NotFound();
        }

        var modelo = new AutorFormViewModel
        {
            Id = autor.Id,
            Nome = autor.Nome,
            Nacionalidade = autor.Nacionalidade,
        };

        return View(modelo);
    }

    // POST: /Autores/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AutorFormViewModel modelo)
    {
        if (id != modelo.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        try
        {
            await _autorServico.AtualizarAsync(id, modelo.Nome, modelo.Nacionalidade);
            TempData["MensagemSucesso"] = "Autor atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Falha de validação ao atualizar autor {AutorId}.", id);
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(modelo);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    // GET: /Autores/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var autor = await _autorServico.ObterComLivrosAsync(id);

        if (autor is null)
        {
            return NotFound();
        }

        return View(autor);
    }

    // POST: /Autores/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _autorServico.RemoverAsync(id);
        TempData["MensagemSucesso"] = "Autor removido com sucesso.";
        return RedirectToAction(nameof(Index));
    }
}
