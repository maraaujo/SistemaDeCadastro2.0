using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

//builder.Services.AddDbContext<SistemaDeCadastroContext>(options =>
//{
//    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
//});
//builder.Services.AddDbContext<SistemaDeCadastroContext>(options =>
//{
//    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
//    options.LogTo(Console.WriteLine, LogLevel.Information);
//});


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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

