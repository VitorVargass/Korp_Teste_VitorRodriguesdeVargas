using Microsoft.EntityFrameworkCore;
using FaturamentoService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<Db>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("pg")));
builder.Services.AddControllers();
builder.Services.AddHttpClient("estoque", c => c.BaseAddress = new Uri("http://localhost:5080"));
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
   ));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("estoque", c =>
    c.BaseAddress = new Uri("http://localhost:5080"));

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Faturamento");

        
        c.SwaggerEndpoint("http://localhost:5080/swagger/v1/swagger.json", "Estoque");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run("http://localhost:5124");
