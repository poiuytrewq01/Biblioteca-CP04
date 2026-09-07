using Biblioteca.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Web.Data;

/// <summary>
/// Contexto do Entity Framework Core responsável por mapear as entidades
/// Autor e Livro para o banco de dados Oracle.
/// </summary>
public class BibliotecaContext : DbContext
{
    public BibliotecaContext(DbContextOptions<BibliotecaContext> options)
        : base(options)
    {
    }

    public DbSet<Autor> Autores => Set<Autor>();

    public DbSet<Livro> Livros => Set<Livro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Autor>(entidade =>
        {
            entidade.ToTable("AUTORES");
            entidade.HasKey(a => a.Id);

            entidade.Property(a => a.Id)
                .HasColumnName("ID_AUTOR")
                .ValueGeneratedOnAdd();

            entidade.Property(a => a.Nome)
                .HasColumnName("NOME")
                .IsRequired()
                .HasMaxLength(150);

            entidade.Property(a => a.Nacionalidade)
                .HasColumnName("NACIONALIDADE")
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Livro>(entidade =>
        {
            entidade.ToTable("LIVROS");
            entidade.HasKey(l => l.Id);

            entidade.Property(l => l.Id)
                .HasColumnName("ID_LIVRO")
                .ValueGeneratedOnAdd();

            entidade.Property(l => l.Titulo)
                .HasColumnName("TITULO")
                .IsRequired()
                .HasMaxLength(200);

            entidade.Property(l => l.Preco)
                .HasColumnName("PRECO")
                .HasColumnType("NUMBER(10,2)");

            entidade.Property(l => l.QuantidadeEstoque)
                .HasColumnName("QUANTIDADE_ESTOQUE");

            entidade.Property(l => l.AutorId)
                .HasColumnName("ID_AUTOR");

            entidade.HasOne(l => l.Autor)
                .WithMany(a => a.Livros)
                .HasForeignKey(l => l.AutorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}
