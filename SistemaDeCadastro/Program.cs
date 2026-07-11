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
//app
builder.Services.AddScoped<IPatientApp, PatientApp>();
// app services (register APP layer implementations)
builder.Services.AddScoped<IResponsibleApp, ResponsibleApp>();
builder.Services.AddScoped<IBloodTypeApp, BloodTypeApp>();
builder.Services.AddScoped<IEmployeeApp, EmployeeApp>();
builder.Services.AddScoped<IDepartmentApp, DepartmentApp>();
builder.Services.AddScoped<IClinicalConditionApp, ClinicalConditionApp>();
builder.Services.AddScoped<IPatientClinicalConditionApp, PatientClinicalConditionApp>();
builder.Services.AddScoped<IIllnessApp, IllnessApp>();
builder.Services.AddScoped<IPatientIllnessApp, PatientIllnessApp>();
builder.Services.AddScoped<IMedicineApp, MedicineApp>();
builder.Services.AddScoped<IMedicinePatientClinicalConditionApp, MedicinePatientClinicalConditionApp>();
builder.Services.AddScoped<IPatientEmployeeApp, PatientEmployeeApp>();
builder.Services.AddScoped<ILoginAccountApp, LoginAccountApp>();
builder.Services.AddScoped<IAvailablePermissionApp, AvailablePermissionApp>();
builder.Services.AddScoped<IUserPermissionApp, UserPermissionApp>();
builder.Services.AddScoped<IAccessLogApp, AccessLogApp>();
builder.Services.AddScoped<IAppointmentApp, AppointmentApp>();
builder.Services.AddScoped<ICareServiceApp, CareServiceApp>();
//builder.Services.AddScoped<IPaymentApp, PaymentApp>();

//repositorys
builder.Services.AddScoped<SistemaDeCadastroContext>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IResponsibleRepository, ResponsibleRepository>();
builder.Services.AddScoped<IBloodTypeRepository, BloodTypeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IClinicalConditionRepository, ClinicalConditionRepository>();
builder.Services.AddScoped<IPatientClinicalConditionRepository, PatientClinicalConditionRepository>();
builder.Services.AddScoped<IIllnessRepository, IllnessRepository>();
builder.Services.AddScoped<IPatientIllnessRepository, PatientIllnessRepository>();
builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();
builder.Services.AddScoped<IMedicinePatientClinicalConditionRepository, MedicinePatientClinicalConditionRepository>();
builder.Services.AddScoped<IPatientEmployeeRepository, PatientEmployeeRepository>();
builder.Services.AddScoped<ILoginAccountRepository, LoginAccountRepository>();
builder.Services.AddScoped<IAvailablePermissionRepository, AvailablePermissionRepository>();
builder.Services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
builder.Services.AddScoped<IAccessLogRepository, AccessLogRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<ICareServiceRepository, CareServiceRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
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
