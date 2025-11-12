using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstoqueService.Data;

[ApiController]
[Route("api/[controller]")]
public class EstoqueController : ControllerBase
{
    private readonly Db _db;
    public EstoqueController(Db db) => _db = db;

    [HttpPost("confirmar")]
    public async Task<IActionResult> Confirmar([FromBody] Dictionary<Guid, int> reservas, [FromQuery] bool fail = false, [FromQuery] bool slow = false)
    {

        if (fail) return StatusCode(503, new { erro = "SIMULADO" });
        if (slow) Thread.Sleep(7000);

        if (reservas is null || reservas.Count == 0) return BadRequest();

        var ids = reservas.Keys.ToList();
        var produtos = await _db.Produtos.Where(p => ids.Contains(p.Id)).ToListAsync();

        foreach (var kv in reservas)
        {
            var p = produtos.FirstOrDefault(x => x.Id == kv.Key);
            if (p is null || p.Saldo < kv.Value)
                return Conflict(new { erro = "SALDO_INSUFICIENTE", produtoId = kv.Key });
        }

        foreach (var kv in reservas)
            produtos.First(x => x.Id == kv.Key).Saldo -= kv.Value;

        await _db.SaveChangesAsync();
        return Ok();
    }
}
