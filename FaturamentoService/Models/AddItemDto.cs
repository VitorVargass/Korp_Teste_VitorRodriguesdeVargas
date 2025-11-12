using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FaturamentoService.Models
{
    public class AddItemDto
    {
        [JsonPropertyName("produtoId")]
        [Required]
        public Guid ProdutoId { get; set; }

        [JsonPropertyName("codigo")]
        [Required, MinLength(1)]
        public string Codigo { get; set; } = string.Empty;

        [JsonPropertyName("descricao")]
        [Required, MinLength(1)]
        public string Descricao { get; set; } = string.Empty;

        [JsonPropertyName("quantidade")]
        [Range(1, int.MaxValue)]
        public int Quantidade { get; set; }
    }
}
