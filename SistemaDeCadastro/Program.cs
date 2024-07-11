using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.APP;
using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Data.Interface;
using SistemaDeCadastro.Data.Repository;
using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policyBuilder =>
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

builder.Services.AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<SistemaDeCadastroContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IIdosoRepository, IdosoRepository>();
builder.Services.AddScoped<IIdosoApp, IdosoApp>();
builder.Services.AddScoped<IAdminApp, AdminApp>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();

var app = builder.Build();


// Configure the HTTP request pipeline.
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
