using Microsoft.EntityFrameworkCore;
using TPLogicaWebApi.DATA.Models;
using TPLogicaWebApi.DATA.Repositories.Implementations;
using TPLogicaWebApi.DATA.Repositories.Interfaces;
using TPLogicaWebApi.DATA.Services.Implementations;
using TPLogicaWebApi.DATA.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<FarmaciaTPLogica1Context>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// Registro de Productos (ya lo tenías)
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();

// ¡¡AQUÍ ESTÁ LA SOLUCIÓN!!
// Agrega estas dos líneas para el nuevo recurso de Clientes:
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();

builder.Services.AddControllers();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
