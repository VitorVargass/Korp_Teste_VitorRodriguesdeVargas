using Microsoft.EntityFrameworkCore;
using EstoqueService.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<Db>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("pg")));


builder.Services.AddCors(p => p.AddDefaultPolicy(b =>
    b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();


app.Run("http://localhost:5080");
