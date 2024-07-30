using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.APP;
using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Data.Interface;
using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Infra.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader();
    });
});

var connectionString = builder.Configuration.GetConnectionString("SistemaDeCadastro");

builder.Services.AddDbContext<SistemaDeCadastroContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});
builder.Services.AddDbContext<SistemaDeCadastroContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    options.LogTo(Console.WriteLine, LogLevel.Information);
});
builder.Services
    .AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<SistemaDeCadastroContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IIdosoRepository, IdosoRepository>();
builder.Services.AddScoped<IIdosoApp, IdosoApp>();
builder.Services.AddScoped<IAdminApp, AdminApp>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IUsuarioApp, UsuarioApp>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ITokenServiceRepository, TokenServiceRepository>();
builder.Services.AddScoped<IFuncionarioRepository,  FuncionarioRepository>();
builder.Services.AddScoped<IFuncionarioApp, FuncionarioApp>();
builder.Services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
builder.Services.AddScoped<IDepartamentoApp, DepartamentoApp>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseAuthorization();
app.MapControllers();
app.Run();

