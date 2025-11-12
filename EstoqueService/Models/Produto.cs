namespace EstoqueService.Models
{
    public class Produto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Codigo { get; set; } = "";
        public string Descricao { get; set; } = "";
        public int Saldo { get; set; }

    }
}
