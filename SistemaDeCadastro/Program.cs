using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Infra.Repository;

var builder = WebApplication.CreateBuilder(args);

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader();
    });
});

// String de conexão (recomenda-se usar appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("SistemaCadastro");

// Registro do contexto DbContext - REGISTRAR PRIMEIRO
builder.Services.AddDbContext<SistemaCadastroContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    options.LogTo(Console.WriteLine, LogLevel.Information); // Habilita logs de queries
});

// Registro de repositórios e serviços - REGISTRAR DEPOIS DO DbContext
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientApp, PatientApp>();
builder.Services.AddScoped<IMedicinePatientIllnessRepository, MedicinePatientIllnessRepository>();
builder.Services.AddScoped<IMedicinePatientIllnessApp, MedicinePatientIllnessApp>();
builder.Services.AddScoped<IIllnessRepository, IllnessRepository>();
builder.Services.AddScoped<IMedicineApp, MedicineApp>();
builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();
builder.Services.AddScoped<IMedicinePatientIllnessHistoricApp, MedicinePatientIllnessHistoricApp>();
builder.Services.AddScoped<IMedicinePatientIllnessHistoricRepository, MedicinePatientIllnessHistoricRepository>();
builder.Services.AddScoped<IBloodTypeApp, BloodTypeApp>();
builder.Services.AddScoped<IBloodTypeRepository, BloodTypeRepository>();
builder.Services.AddScoped<IPatientIllnessApp, PatientIllnessApp>();
builder.Services.AddScoped<IPatientIllnessRepository, PatientIllnessRepository>();

// AutoMapper e Controllers
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração do pipeline
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

