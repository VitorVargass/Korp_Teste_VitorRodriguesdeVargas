using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using FaturamentoService.Data;
using FaturamentoService.Models;

namespace FaturamentoService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotasController : ControllerBase
{
    private readonly IHttpClientFactory _http;
    private readonly Db _db;

    public NotasController(IHttpClientFactory http, Db db)
    {
        _http = http;
        _db = db;
    }

    
    public record AddItemDto(Guid ProdutoId, string Codigo, string Descricao, int Quantidade);

    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Nota>>> Get()
    {
        var notas = await _db.Notas
            .AsNoTracking()
            .Include(n => n.Itens)
            .OrderByDescending(n => n.Numero)
            .ToListAsync();

        return Ok(notas);
    }

    
    [HttpPost]
    public async Task<ActionResult<Nota>> Create()
    {
        
        var max = await _db.Notas.MaxAsync(n => (int?)n.Numero) ?? 0;
        var nota = new Nota { Numero = max + 1, Status = "ABERTA" };

        _db.Notas.Add(nota);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = nota.Id }, nota);
    }

    
    [HttpPost("{id:guid}/itens")]
    public async Task<ActionResult<Nota>> AddItem(Guid id, [FromBody] AddItemDto dto)
    {
        var n = await _db.Notas.Include(x => x.Itens).FirstOrDefaultAsync(x => x.Id == id);
        if (n is null) return NotFound();

        if (!string.Equals(n.Status, "ABERTA", StringComparison.OrdinalIgnoreCase))
            return Conflict(new { erro = "STATUS_INVALIDO" });

        if (dto.ProdutoId == Guid.Empty || dto.Quantidade <= 0)
            return BadRequest(new { message = "Dados inválidos (produtoId/quantidade)." });

        var ex = n.Itens.FirstOrDefault(i => i.ProdutoId == dto.ProdutoId);
        if (ex is null)
        {
            n.Itens.Add(new NotaItem
            {
                NotaId = n.Id,
                ProdutoId = dto.ProdutoId,
                Codigo = dto.Codigo,
                Descricao = dto.Descricao,
                Quantidade = dto.Quantidade
            });
        }
        else
        {
            ex.Quantidade += dto.Quantidade;
        }

        n.TotalItens = n.Itens.Sum(i => i.Quantidade);
        await _db.SaveChangesAsync();

        
        n = await _db.Notas.Include(x => x.Itens).FirstAsync(x => x.Id == id);
        return Ok(n);
    }

    
    [HttpPost("{id:guid}/imprimir")]
    public async Task<ActionResult<Nota>> Imprimir(Guid id)
    {
        var n = await _db.Notas.Include(x => x.Itens).FirstOrDefaultAsync(x => x.Id == id);
        if (n is null) return NotFound();
        if (!string.Equals(n.Status, "ABERTA", StringComparison.OrdinalIgnoreCase))
            return Conflict(new { erro = "STATUS_INVALIDO" });
        if (!n.Itens.Any()) return BadRequest(new { erro = "SEM_ITENS" });

        
        var dict = n.Itens
            .GroupBy(i => i.ProdutoId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantidade));

        var client = _http.CreateClient("estoque");
        var resp = await client.PostAsJsonAsync("api/estoque/confirmar", dict);
        if (!resp.IsSuccessStatusCode) return StatusCode((int)resp.StatusCode);

        n.Status = "FECHADA";
        await _db.SaveChangesAsync();

        
        n = await _db.Notas.Include(x => x.Itens).FirstAsync(x => x.Id == id);
        return Ok(n);
    }

    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var n = await _db.Notas.FirstOrDefaultAsync(x => x.Id == id);
        if (n is null) return NotFound();

        if (!string.Equals(n.Status, "ABERTA", StringComparison.OrdinalIgnoreCase))
            return Conflict(new { erro = "STATUS_INVALIDO", message = "Só é possível excluir notas ABERTAS." });

        _db.Notas.Remove(n);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    
    [HttpDelete("{id:guid}/itens/{produtoId:guid}")]
    public async Task<ActionResult<Nota>> RemoverItem(Guid id, Guid produtoId)
    {
        var n = await _db.Notas.Include(x => x.Itens).FirstOrDefaultAsync(x => x.Id == id);
        if (n is null) return NotFound();
        if (!string.Equals(n.Status, "ABERTA", StringComparison.OrdinalIgnoreCase))
            return Conflict(new { erro = "STATUS_INVALIDO" });

        var item = n.Itens.FirstOrDefault(i => i.ProdutoId == produtoId);
        if (item is null) return NotFound(new { message = "Item não encontrado" });

        _db.NotaItens.Remove(item);
        await _db.SaveChangesAsync();

       
        n = await _db.Notas.Include(x => x.Itens).FirstAsync(x => x.Id == id);
        n.TotalItens = n.Itens.Sum(i => i.Quantidade);
        await _db.SaveChangesAsync();

        return Ok(n);
    }
}
