using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
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


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");
}

builder.Services.AddDbContext<SistemaDeCadastroContext>(options =>
{
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 46))
    );

    options.LogTo(Console.WriteLine, LogLevel.Information);
});

builder.Services.AddScoped<SistemaDeCadastroContext>();
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
// Register generic base repository so any model can get a CRUD repository via IBaseRepository<T>
builder.Services.AddScoped(typeof(SistemaDeCadastro.Infra.Interface.IBaseRepository<>), typeof(SistemaDeCadastro.Infra.Repository.BaseRepository<>));


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();

// Configuração do Swagger
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
