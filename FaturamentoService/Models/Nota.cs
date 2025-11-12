namespace FaturamentoService.Models
{
    public class Nota
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Numero { get; set; }
        public string Status { get; set; } = "ABERTA";
        public List<NotaItem> Itens { get; set; } = new();
        public int TotalItens { get; set; }
    }
    public class NotaItem
    {
        public Guid Id { get; set; }
        public Guid ProdutoId { get; set; }
        public Guid NotaId { get; set; }
        public string Codigo { get; set; } = "";
        public string Descricao { get; set; } = "";
        public int Quantidade { get; set; }
    }
}
