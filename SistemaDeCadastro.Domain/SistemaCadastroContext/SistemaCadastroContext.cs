using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Domain.SistemaCadastroContext;

public partial class SistemaDeCadastroContext : DbContext
{
    public SistemaDeCadastroContext()
    {
    }

    public SistemaDeCadastroContext(DbContextOptions<SistemaDeCadastroContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Patient> Patients { get; set; }
    public virtual DbSet<Responsible> Responsibles { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<ClinicalCondition> ClinicalConditions { get; set; }
    public virtual DbSet<PatientClinicalCondition> PatientClinicalConditions { get; set; }
    public virtual DbSet<BloodType> BloodTypes { get; set; }
    public virtual DbSet<Illness> Illnesses { get; set; }
    public virtual DbSet<PatientIllness> PatientIllnesses { get; set; }

    public virtual DbSet<Medicine> Medicines { get; set; }
    public virtual DbSet<MedicinePatientClinicalCondition> MedicinePatientClinicalConditions { get; set; }

    public virtual DbSet<PatientEmployee> PatientEmployees { get; set; }

    public virtual DbSet<LoginAccount> LoginAccounts { get; set; }
    public virtual DbSet<AvailablePermission> AvailablePermissions { get; set; }
    public virtual DbSet<UserPermission> UserPermissions { get; set; }
    public virtual DbSet<AccessLog> AccessLogs { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }
    public virtual DbSet<CareService> CareServices { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(
            "server=127.0.0.1;port=3306;database=sistema_cadastro_tcc;user=sistema_user;password=irmaos03;",
                new MySqlServerVersion(new Version(8, 0, 46))
            );
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("acolhidos");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_acolhido")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("nome")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(e => e.Phone)
                .HasColumnName("telefone")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.Document)
                .HasColumnName("documento")
                .HasMaxLength(12)
                .IsRequired();

            entity.Property(e => e.BirthDate)
                .HasColumnName("data_nascimento")
                .HasColumnType("date");

            entity.Property(e => e.Gender)
                .HasColumnName("sexo")
                .HasMaxLength(20);

            entity.Property(e => e.Cpf)
                .HasColumnName("cpf")
                .HasMaxLength(14);

            entity.HasIndex(e => e.Cpf)
                .IsUnique();

            entity.Property(e => e.Observations)
                .HasColumnName("observacoes")
                .HasColumnType("text");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("data_cadastro")
                .HasColumnType("datetime");

            entity.Property(e => e.BloodTypeId)
                .HasColumnName("id_blood_type");

            entity.HasOne(e => e.BloodType)
                .WithMany(e => e.Patients)
                .HasForeignKey(e => e.BloodTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<BloodType>(entity =>
        {
            entity.ToTable("blood_types");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_blood_type")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(4)
                .IsRequired();
        });

        modelBuilder.Entity<Responsible>(entity =>
        {
            entity.ToTable("responsaveis");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_responsavel")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.PatientId)
                .HasColumnName("id_acolhido");

            entity.Property(e => e.Name)
                .HasColumnName("nome")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(e => e.Phone)
                .HasColumnName("telefone")
                .HasMaxLength(20);

            entity.Property(e => e.Relationship)
                .HasColumnName("parentesco")
                .HasMaxLength(100);

            entity.Property(e => e.Address)
                .HasColumnName("endereco")
                .HasColumnType("text");

            entity.HasOne(e => e.Patient)
                .WithMany(e => e.Responsibles)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("departamentos");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_departamento")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("nome_departamento")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasColumnName("descricao")
                .HasColumnType("text");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("funcionarios");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_funcionario")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("nome")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(e => e.Cpf)
                .HasColumnName("cpf")
                .HasMaxLength(14);

            entity.HasIndex(e => e.Cpf)
                .IsUnique();

            entity.Property(e => e.Position)
                .HasColumnName("cargo")
                .HasMaxLength(100);

            entity.Property(e => e.Phone)
                .HasColumnName("telefone")
                .HasMaxLength(20);

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(150);

            entity.Property(e => e.AdmissionDate)
                .HasColumnName("data_admissao")
                .HasColumnType("date");

            entity.Property(e => e.DepartmentId)
                .HasColumnName("id_departamento");

            entity.HasOne(e => e.Department)
                .WithMany(e => e.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PatientEmployee>(entity =>
        {
            entity.ToTable("acolhido_funcionario");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_acolhido_funcionario")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.PatientId)
                .HasColumnName("id_acolhido");

            entity.Property(e => e.EmployeeId)
                .HasColumnName("id_funcionario");

            entity.Property(e => e.ResponsibilityFunction)
                .HasColumnName("funcao_responsavel")
                .HasMaxLength(100);

            entity.Property(e => e.StartDate)
                .HasColumnName("data_inicio")
                .HasColumnType("date");

            entity.Property(e => e.EndDate)
                .HasColumnName("data_fim")
                .HasColumnType("date");

            entity.HasOne(e => e.Patient)
                .WithMany(e => e.PatientEmployees)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Employee)
                .WithMany(e => e.PatientEmployees)
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ClinicalCondition>(entity =>
        {
            entity.ToTable("condicoesclinicas");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_condicao")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("nome_condicao")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasColumnName("descricao")
                .HasColumnType("text");

            entity.Property(e => e.Type)
                .HasColumnName("tipo")
                .HasMaxLength(100);
        });

        modelBuilder.Entity<PatientClinicalCondition>(entity =>
        {
            entity.ToTable("acolhido_condicaoclinica");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_acolhido_condicao")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.PatientId)
                .HasColumnName("id_acolhido");

            entity.Property(e => e.ClinicalConditionId)
                .HasColumnName("id_condicao");

            entity.Property(e => e.DiagnosisDate)
                .HasColumnName("data_diagnostico")
                .HasColumnType("date");

            entity.Property(e => e.Observations)
                .HasColumnName("observacoes")
                .HasColumnType("text");

            entity.HasOne(e => e.Patient)
                .WithMany(e => e.PatientClinicalConditions)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ClinicalCondition)
                .WithMany(e => e.PatientClinicalConditions)
                .HasForeignKey(e => e.ClinicalConditionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Illness>(entity =>
        {
            entity.ToTable("doencas");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_doenca")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("nome")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasColumnName("descricao")
                .HasColumnType("text");

            entity.Property(e => e.Cid)
                .HasColumnName("cid")
                .HasMaxLength(20);
        });

        modelBuilder.Entity<PatientIllness>(entity =>
        {
            entity.ToTable("acolhido_doenca");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_acolhido_doenca")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.PatientId)
                .HasColumnName("id_acolhido");

            entity.Property(e => e.IllnessId)
                .HasColumnName("id_doenca");

            entity.Property(e => e.DiagnosisDate)
                .HasColumnName("data_diagnostico")
                .HasColumnType("date");

            entity.Property(e => e.Observations)
                .HasColumnName("observacoes")
                .HasColumnType("text");

            entity.HasOne(e => e.Patient)
                .WithMany(e => e.PatientIllnesses)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Illness)
                .WithMany(e => e.PatientIllnesses)
                .HasForeignKey(e => e.IllnessId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.ToTable("medicamentos");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_medicamento")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("nome")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(e => e.Dosage)
                .HasColumnName("dosagem")
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasColumnName("descricao")
                .HasColumnType("text");

            entity.Property(e => e.AdministrationRoute)
                .HasColumnName("via_administracao")
                .HasMaxLength(100);
        });

        modelBuilder.Entity<MedicinePatientClinicalCondition>(entity =>
        {
            entity.ToTable("medicamento_acolhido_condicaoclinica");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_medicamento_acond")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.MedicineId)
                .HasColumnName("id_medicamento");

            entity.Property(e => e.PatientClinicalConditionId)
                .HasColumnName("id_acolhido_condicao");

            entity.Property(e => e.PrescribedDosage)
                .HasColumnName("dosagem_prescrita")
                .HasMaxLength(100);

            entity.Property(e => e.Frequency)
                .HasColumnName("frequencia")
                .HasMaxLength(100);

            entity.Property(e => e.StartDate)
                .HasColumnName("data_inicio")
                .HasColumnType("date");

            entity.Property(e => e.EndDate)
                .HasColumnName("data_fim")
                .HasColumnType("date");

            entity.Property(e => e.Observations)
                .HasColumnName("observacoes")
                .HasColumnType("text");

            entity.HasOne(e => e.Medicine)
                .WithMany(e => e.PatientClinicalConditionMedicines)
                .HasForeignKey(e => e.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PatientClinicalCondition)
                .WithMany(e => e.Medicines)
                .HasForeignKey(e => e.PatientClinicalConditionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LoginAccount>(entity =>
        {
            entity.ToTable("contas_login");

            entity.HasKey(e => e.Id);

            entity.HasAlternateKey(e => e.UserId);

            entity.Property(e => e.Id)
                .HasColumnName("id_conta_login")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UserId)
                .HasColumnName("id_usuario");

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(150)
                .IsRequired();

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.Property(e => e.PasswordHash)
                .HasColumnName("senha_hash")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.UserType)
                .HasColumnName("tipo_usuario")
                .HasMaxLength(50);

            entity.Property(e => e.LastLogin)
                .HasColumnName("ultimo_login")
                .HasColumnType("datetime");

            entity.Property(e => e.Active)
                .HasColumnName("ativo")
                .HasDefaultValue(true);
        });

        modelBuilder.Entity<AvailablePermission>(entity =>
        {
            entity.ToTable("permissoes_disponiveis");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_permissao")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("nome_permissao")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasColumnName("descricao")
                .HasColumnType("text");

            entity.Property(e => e.Module)
                .HasColumnName("modulo")
                .HasMaxLength(100);

            entity.Property(e => e.Active)
                .HasColumnName("ativo")
                .HasDefaultValue(true);
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.ToTable("usuarios_permissoes");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_usuario_permissao")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UserId)
                .HasColumnName("id_usuario");

            entity.Property(e => e.PermissionId)
                .HasColumnName("id_permissao");

            entity.Property(e => e.GrantedAt)
                .HasColumnName("data_concessao")
                .HasColumnType("datetime");

            entity.HasOne(e => e.User)
                .WithMany(e => e.UserPermissions)
                .HasForeignKey(e => e.UserId)
                .HasPrincipalKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Permission)
                .WithMany(e => e.UserPermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AccessLog>(entity =>
        {
            entity.ToTable("log_acesso");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_log")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UserId)
                .HasColumnName("id_usuario");

            entity.Property(e => e.Action)
                .HasColumnName("acao")
                .HasMaxLength(255);

            entity.Property(e => e.DateTime)
                .HasColumnName("data_hora")
                .HasColumnType("datetime");

            entity.Property(e => e.IpAddress)
                .HasColumnName("ip_acesso")
                .HasMaxLength(45);

            entity.Property(e => e.Observation)
                .HasColumnName("observacao")
                .HasColumnType("text");

            entity.HasOne(e => e.User)
                .WithMany(e => e.AccessLogs)
                .HasForeignKey(e => e.UserId)
                .HasPrincipalKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("agendamentos");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_agendamento")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.PatientId)
                .HasColumnName("id_acolhido");

            entity.Property(e => e.UserId)
                .HasColumnName("id_usuario");

            entity.Property(e => e.AppointmentType)
                .HasColumnName("tipo_atendimento")
                .HasMaxLength(100);

            entity.Property(e => e.DateTime)
                .HasColumnName("data_hora")
                .HasColumnType("datetime");

            entity.Property(e => e.Responsible)
                .HasColumnName("responsavel")
                .HasMaxLength(150);

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(50);

            entity.Property(e => e.Observations)
                .HasColumnName("observacoes")
                .HasColumnType("text");

            entity.HasOne(e => e.Patient)
                .WithMany(e => e.Appointments)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.User)
                .WithMany(e => e.Appointments)
                .HasForeignKey(e => e.UserId)
                .HasPrincipalKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CareService>(entity =>
        {
            entity.ToTable("atendimentos");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_atendimento")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.AppointmentId)
                .HasColumnName("id_agendamento");

            entity.Property(e => e.PatientId)
                .HasColumnName("id_acolhido");

            entity.Property(e => e.UserId)
                .HasColumnName("id_usuario");

            entity.Property(e => e.Description)
                .HasColumnName("descricao")
                .HasColumnType("text");

            entity.Property(e => e.ServiceDate)
                .HasColumnName("data_atendimento")
                .HasColumnType("datetime");

            entity.Property(e => e.Referral)
                .HasColumnName("encaminhamento")
                .HasColumnType("text");

            entity.Property(e => e.Observations)
                .HasColumnName("observacoes")
                .HasColumnType("text");

            entity.HasOne(e => e.Appointment)
                .WithMany(e => e.CareServices)
                .HasForeignKey(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Patient)
                .WithMany(e => e.CareServices)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.User)
                .WithMany(e => e.CareServices)
                .HasForeignKey(e => e.UserId)
                .HasPrincipalKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("pagamentos");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_pagamento")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.AppointmentId)
                .HasColumnName("id_agendamento");

            entity.Property(e => e.UserId)
                .HasColumnName("id_usuario");

            entity.Property(e => e.TotalAmount)
                .HasColumnName("valor_total")
                .HasColumnType("decimal(10,2)");

            entity.Property(e => e.PaymentMethod)
                .HasColumnName("metodo_pagamento")
                .HasMaxLength(50);

            entity.Property(e => e.PaymentDate)
                .HasColumnName("data_pagamento")
                .HasColumnType("datetime");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(50);

            entity.Property(e => e.ExternalTransactionId)
                .HasColumnName("id_transacao_externa")
                .HasMaxLength(255);

            entity.HasIndex(e => e.ExternalTransactionId)
                .IsUnique();

            entity.HasOne(e => e.Appointment)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.User)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.UserId)
                .HasPrincipalKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}