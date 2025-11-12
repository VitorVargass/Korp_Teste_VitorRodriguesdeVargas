using Microsoft.EntityFrameworkCore;
using EstoqueService.Models;

namespace EstoqueService.Data;

public class Db : DbContext
{
    public Db(DbContextOptions<Db> opt) : base(opt) { }

    public DbSet<Produto> Produtos => Set<Produto>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        var e = b.Entity<Produto>();

        e.ToTable("produtos", schema: "estoque");

        e.HasKey(p => p.Id);
        e.Property(p => p.Id)
            .HasColumnName("id")                 // 👈 força minúsculo
            .HasDefaultValueSql("gen_random_uuid()");

        e.Property(p => p.Codigo)
            .HasColumnName("codigo")             // 👈 idem
            .IsRequired()
            .HasMaxLength(64);

        e.Property(p => p.Descricao)
            .HasColumnName("descricao")          // 👈 idem
            .IsRequired()
            .HasMaxLength(200);

        e.Property(p => p.Saldo)
            .HasColumnName("saldo")              // 👈 idem
            .HasPrecision(18, 2);

        e.HasIndex(p => p.Codigo).IsUnique();
    }
}
