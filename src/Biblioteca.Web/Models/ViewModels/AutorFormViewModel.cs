using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Web.Models.ViewModels;

/// <summary>
/// ViewModel usada nos formulários de criação/edição de Autor.
/// </summary>
public class AutorFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do autor.")]
    [StringLength(150, ErrorMessage = "O nome pode ter no máximo 150 caracteres.")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "A nacionalidade pode ter no máximo 100 caracteres.")]
    [Display(Name = "Nacionalidade")]
    public string? Nacionalidade { get; set; }
}
