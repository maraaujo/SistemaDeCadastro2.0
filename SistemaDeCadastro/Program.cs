using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Infra.Repository;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader();
    });
});

// Connection String (lê senha de variável de ambiente se ausente)
var baseConnection = builder.Configuration
    .GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(baseConnection))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' não encontrada."
    );
}

var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
// Se a senha não estiver na connection string, exige variável de ambiente
if (!baseConnection.Contains("password=", StringComparison.OrdinalIgnoreCase) &&
    !baseConnection.Contains("pwd=", StringComparison.OrdinalIgnoreCase))
{
    if (string.IsNullOrWhiteSpace(dbPassword))
    {
        throw new InvalidOperationException(
            "Senha do banco não encontrada. Defina a variável de ambiente 'DB_PASSWORD'."
        );
    }
    if (!baseConnection.EndsWith(";")) baseConnection += ";";
    baseConnection += $"password={dbPassword};";
}

var connectionString = baseConnection;

// DbContext
builder.Services.AddDbContext<SistemaDeCadastroContext>(options =>
{
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    );

    options.LogTo(
        Console.WriteLine,
        LogLevel.Information
    );
});
builder.Services.AddHttpContextAccessor();
// APPs
builder.Services.AddScoped<IAuthApp, AuthApp>();
builder.Services.AddScoped<IPatientApp, PatientApp>();
builder.Services.AddScoped<IResponsibleApp, ResponsibleApp>();
builder.Services.AddScoped<IBloodTypeApp, BloodTypeApp>();
builder.Services.AddScoped<IEmployeeApp, EmployeeApp>();
builder.Services.AddScoped<IDepartmentApp, DepartmentApp>();
builder.Services.AddScoped<IClinicalConditionApp, ClinicalConditionApp>();
builder.Services.AddScoped<IPatientClinicalConditionApp, PatientClinicalConditionApp>();
builder.Services.AddScoped<IMedicineApp, MedicineApp>();
builder.Services.AddScoped<IMedicinePatientClinicalConditionApp, MedicinePatientClinicalConditionApp>();
builder.Services.AddScoped<IPatientEmployeeApp, PatientEmployeeApp>();
builder.Services.AddScoped<ILoginAccountApp, LoginAccountApp>();
builder.Services.AddScoped<IAvailablePermissionApp, AvailablePermissionApp>();
builder.Services.AddScoped<IUserPermissionApp, UserPermissionApp>();
builder.Services.AddScoped<IAccessLogApp, AccessLogApp>();
builder.Services.AddScoped<IAppointmentApp, AppointmentApp>();
builder.Services.AddScoped<IPaymentApp, PaymentApp>();
builder.Services.AddScoped<ISubscriptionApp, SubscriptionApp>();
builder.Services.AddScoped<ISubscriptionPaymentApp, SubscriptionPaymentApp>();
builder.Services.AddScoped<IPlanApp, PlanApp>();
builder.Services.AddScoped<IInstitutionApp, InstitutionApp>();
builder.Services.AddScoped<IMedicationAdministrationApp, MedicationAdministrationApp>();
builder.Services.AddScoped<IAdminOverviewApp, AdminOverviewApp>();
builder.Services.AddScoped<ICurrentUserServiceContext, CurrentUserServiceContext>();
builder.Services.AddScoped<IInternalAssistantApp, InternalAssistantApp>();
// Repositories
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IResponsibleRepository, ResponsibleRepository>();
builder.Services.AddScoped<IBloodTypeRepository, BloodTypeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IClinicalConditionRepository, ClinicalConditionRepository>();
builder.Services.AddScoped<IPatientClinicalConditionRepository, PatientClinicalConditionRepository>();
builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();
builder.Services.AddScoped<IMedicinePatientClinicalConditionRepository, MedicinePatientClinicalConditionRepository>();
builder.Services.AddScoped<IPatientEmployeeRepository, PatientEmployeeRepository>();
builder.Services.AddScoped<ILoginAccountRepository, LoginAccountRepository>();
builder.Services.AddScoped<IAvailablePermissionRepository, AvailablePermissionRepository>();
builder.Services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
builder.Services.AddScoped<IAccessLogRepository, AccessLogRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<ISubscriptionPaymentRepository, SubscriptionPaymentRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IInstitutionRepository, InstitutionRepository>();
builder.Services.AddScoped<IMedicationAdministrationRepository, MedicationAdministrationRepository>();
builder.Services.AddScoped<IInternalAssistantRepository, InternalAssistantRepository>();
builder.Services.AddScoped<IAdminOverviewRepository, AdminOverviewRepository>();

// Base Repository gen�rico
builder.Services.AddScoped(
    typeof(IBaseRepository<>),
    typeof(BaseRepository<>)
);

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Controllers
builder.Services.AddControllers();

// JWT Config
var jwtSection = builder.Configuration.GetSection("Jwt");

var jwtKey = jwtSection.GetValue<string>("Key");
var jwtIssuer = jwtSection.GetValue<string>("Issuer");
var jwtAudience = jwtSection.GetValue<string>("Audience");

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("JWT Key n�o configurada no appsettings.json.");
}

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new InvalidOperationException("JWT Issuer n�o configurado no appsettings.json.");
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException("JWT Audience n�o configurado no appsettings.json.");
}

// Authentication JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("JWT ERRO: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("JWT CHALLENGE: " + context.ErrorDescription);
                return Task.CompletedTask;
            }
        };
    });

// Policy do back-office: somente o administrador do SaaS (usuario autenticado,
// com papel administrativo E sem vinculo a uma instituicao) enxerga a visao global.
// Os papeis aceitos sao configuraveis (AdminBackoffice:Roles); o padrao e "Administrador".
var adminRoles = builder.Configuration
    .GetSection("AdminBackoffice:Roles")
    .Get<string[]>();

if (adminRoles == null || adminRoles.Length == 0)
{
    adminRoles = new[] { "Administrador" };
}

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SaasAdmin", policy =>
        policy.RequireAssertion(context =>
        {
            var user = context.User;

            if (user?.Identity?.IsAuthenticated != true)
                return false;

            // Precisa possuir um dos papeis administrativos configurados.
            if (!adminRoles.Any(role => user.IsInRole(role)))
                return false;

            // Usuarios vinculados a uma instituicao NAO acessam a visao global
            // (preserva o isolamento entre tenants). Admin da plataforma nao tem instituicao.
            var institutionId =
                user.FindFirst("institutionId")?.Value ??
                user.FindFirst("InstitutionId")?.Value;

            return string.IsNullOrWhiteSpace(institutionId);
        }));
});

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SistemaDeCadastro API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Cole apenas o token JWT. N�o precisa escrever Bearer."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAllOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();