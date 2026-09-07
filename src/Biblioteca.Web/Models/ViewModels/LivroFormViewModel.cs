using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Web.Models.ViewModels;

/// <summary>
/// ViewModel usada nos formulários de criação/edição de Livro.
/// </summary>
public class LivroFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o título do livro.")]
    [StringLength(200, ErrorMessage = "O título pode ter no máximo 200 caracteres.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o preço do livro.")]
    [Range(0.01, 100000, ErrorMessage = "O preço deve ser maior que zero.")]
    [Display(Name = "Preço (R$)")]
    public decimal Preco { get; set; }

    [Required(ErrorMessage = "Informe a quantidade em estoque.")]
    [Range(0, int.MaxValue, ErrorMessage = "A quantidade em estoque não pode ser negativa.")]
    [Display(Name = "Quantidade em estoque")]
    public int QuantidadeEstoque { get; set; }

    [Required(ErrorMessage = "Selecione o autor do livro.")]
    [Display(Name = "Autor")]
    public int AutorId { get; set; }

    public IEnumerable<AutorOpcaoViewModel> AutoresDisponiveis { get; set; } = new List<AutorOpcaoViewModel>();
}

public class AutorOpcaoViewModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}
