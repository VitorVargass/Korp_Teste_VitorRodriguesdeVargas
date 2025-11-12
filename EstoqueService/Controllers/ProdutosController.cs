namespace EstoqueService.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstoqueService.Data;
using EstoqueService.Models;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly Db _db;
    public ProdutosController(Db db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> Listar() =>
        Ok(await _db.Produtos.AsNoTracking().OrderByDescending(p => p.Id).ToListAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Produto>> Obter(Guid id) =>
        await _db.Produtos.FindAsync(id) is { } p ? Ok(p) : NotFound();

    [HttpPost]
    public async Task<ActionResult<Produto>> Criar([FromBody] Produto p)
    {
        if (await _db.Produtos.AnyAsync(x => x.Codigo == p.Codigo))
            return Conflict(new { erro = "CODIGO_DUPLICADO" });

        _db.Produtos.Add(p);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Obter), new { id = p.Id }, p);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Produto>> Atualizar(Guid id, [FromBody] Produto body)
    {
        var p = await _db.Produtos.FindAsync(id);
        if (p is null) return NotFound();

        if (await _db.Produtos.AnyAsync(x => x.Codigo == body.Codigo && x.Id != id))
            return Conflict(new { erro = "CODIGO_DUPLICADO" });

        p.Codigo = body.Codigo;
        p.Descricao = body.Descricao;
        p.Saldo = body.Saldo;
        await _db.SaveChangesAsync();
        return Ok(p);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var p = await _db.Produtos.FindAsync(id);
        if (p is null) return NotFound();
        _db.Produtos.Remove(p);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
