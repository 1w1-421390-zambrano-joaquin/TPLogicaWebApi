using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IFacturaRepository, FacturarRepository>();
builder.Services.AddScoped<IFacturaService, FacturaService>();
builder.Services.AddScoped<IEmpleadoRepository, EmpleadosRepository>();
builder.Services.AddScoped<IEmpleadoService, EmpleadosService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioReposiory>();
builder.Services.AddScoped<IAuthService, UsuariosService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();

builder.Services.AddControllers();
//Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("Web", policy =>
    {
        policy
            .WithOrigins(
                "http://127.0.0.1:5500",  // Live Server por IP
                "http://localhost:5500"   // Live Server por hostname            // agrega aquí otros orígenes de tu web si corresponde (https, dominio real, etc.)
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();
app.UseCors("Web");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Esto busca el index.html en la raíz
app.UseDefaultFiles();

// Esto sirve los archivos que están en wwwroot (css, js, etc.)
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
