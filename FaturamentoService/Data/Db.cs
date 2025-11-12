using Microsoft.EntityFrameworkCore;
using FaturamentoService.Models;

namespace FaturamentoService.Data;

public class Db : DbContext
{
    public Db(DbContextOptions<Db> opt) : base(opt) { }

    public DbSet<Nota> Notas => Set<Nota>();
    public DbSet<NotaItem> NotaItens => Set<NotaItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        
        var n = b.Entity<Nota>();
        n.ToTable("notas", schema: "faturamento");
        n.HasKey(x => x.Id);

        n.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()"); 

        n.Property(x => x.Numero)
            .HasColumnName("numero")
            .IsRequired();

        n.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasMaxLength(20);

        n.Property(x => x.TotalItens)
            .HasColumnName("total_itens")
            .HasDefaultValue(0);

       
        var i = b.Entity<NotaItem>();
        i.ToTable("nota_itens", schema: "faturamento");
        i.HasKey(x => x.Id);

        i.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        i.Property(x => x.NotaId)
            .HasColumnName("nota_id")
            .IsRequired();

        i.Property(x => x.ProdutoId)
            .HasColumnName("produto_id");

        i.Property(x => x.Codigo)
            .HasColumnName("codigo")
            .HasMaxLength(64);

        i.Property(x => x.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(200);

        i.Property(x => x.Quantidade)
            .HasColumnName("quantidade")
            .IsRequired();

        
        i.HasOne<Nota>()
         .WithMany(n => n.Itens)
         .HasForeignKey(x => x.NotaId);
    }
}
