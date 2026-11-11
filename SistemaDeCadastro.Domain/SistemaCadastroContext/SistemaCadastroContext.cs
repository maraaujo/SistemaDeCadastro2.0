using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Domain.SistemaCadastroContext;

public partial class SistemaDeCadastroContext : DbContext
{
    private readonly ICurrentUserServiceContext _currentUserService;
    /// <summary>
    /// Construtor vazio (usado por ferramentas do EF, como migrations). Sem usuario atual,
    /// o filtro por instituicao fica fechado (CurrentInstitutionId = -1): consultas as entidades filtradas nao retornam nada.
    /// </summary>
    public SistemaDeCadastroContext()
    {
    }

    /// <summary>
    /// Recebe por injecao as opcoes do EF (connection string MySQL) e o usuario logado,
    /// usado nos filtros por instituicao e na auditoria.
    /// </summary>
    public SistemaDeCadastroContext( DbContextOptions<SistemaDeCadastroContext> options,
     ICurrentUserServiceContext currentUserService)
     : base(options)
    {
        _currentUserService = currentUserService;
    }
    // Id que nenhuma instituicao tem: usado quando o token nao traz instituicao e o
    // usuario nao e admin da plataforma, para o filtro nao devolver nada (fail-closed).
    private const long NoInstitution = -1;

    // null = sem filtro, somente para o admin da plataforma.
    private long? CurrentInstitutionId =>
        _currentUserService == null ? NoInstitution
        : _currentUserService.IsPlatformAdmin ? null
        : _currentUserService.InstitutionId ?? NoInstitution;
    public virtual DbSet<Patient> Patients { get; set; }
    public virtual DbSet<Responsible> Responsibles { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<MedicationAdministration> MedicationAdministrations { get; set; }
    public virtual DbSet<ClinicalCondition> ClinicalConditions { get; set; }
    public virtual DbSet<PatientClinicalCondition> PatientClinicalConditions { get; set; }
    public virtual DbSet<BloodType> BloodTypes { get; set; }
    public virtual DbSet<Medicine> Medicines { get; set; }
    public virtual DbSet<MedicinePatientClinicalCondition> MedicinePatientClinicalConditions { get; set; }
    public virtual DbSet<PatientEmployee> PatientEmployees { get; set; }
    public virtual DbSet<LoginAccount> LoginAccounts { get; set; }
    public virtual DbSet<AvailablePermission> AvailablePermissions { get; set; }
    public virtual DbSet<UserPermission> UserPermissions { get; set; }
    public virtual DbSet<AccessLog> AccessLogs { get; set; }
    public virtual DbSet<Appointment> Appointments { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<Institution> Institutions { get; set; }
    public virtual DbSet<Plan> Plans { get; set; }
    public virtual DbSet<Subscription> Subscriptions { get; set; }
    public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

    /// <summary>
    /// Configura o modelo do EF: (1) filtros globais por instituicao (HasQueryFilter), que fazem
    /// toda consulta trazer so dados da instituicao do usuario logado (admin da plataforma ve tudo);
    /// (2) o mapeamento de cada entidade para a tabela/colunas em portugues do MySQL, chaves,
    /// indices unicos e relacionamentos. Todas as FKs usam DeleteBehavior.Restrict: o banco nao
    /// apaga filhos em cascata, entao as Apps precisam excluir os dependentes antes.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ===== Filtros por instituicao (multi-instituicao) =====
        // Entidades com InstitutionId proprio: acolhidos, funcionarios, setores, agendamentos e medicamentos.
        // CurrentInstitutionId == null (admin da plataforma) desliga o filtro.
        modelBuilder.Entity<Patient>()
        .HasQueryFilter(x => CurrentInstitutionId == null || x.InstitutionId == CurrentInstitutionId);

        modelBuilder.Entity<Employee>()
            .HasQueryFilter(x => CurrentInstitutionId == null || x.InstitutionId == CurrentInstitutionId);

        modelBuilder.Entity<Department>()
            .HasQueryFilter(x => CurrentInstitutionId == null || x.InstitutionId == CurrentInstitutionId);

        modelBuilder.Entity<Appointment>()
            .HasQueryFilter(x => CurrentInstitutionId == null || x.InstitutionId == CurrentInstitutionId);

        modelBuilder.Entity<Medicine>()
            .HasQueryFilter(x => CurrentInstitutionId == null || x.InstitutionId == CurrentInstitutionId);
        // Administracao de medicamento: filtrada pela instituicao do acolhido.
        modelBuilder.Entity<MedicationAdministration>()
    .HasQueryFilter(x =>
        CurrentInstitutionId == null ||
        x.Patient.InstitutionId == CurrentInstitutionId
    );

        // Entidades filhas nao tem InstitutionId proprio: o isolamento vem pela
        // navegacao ate o acolhido (ou agendamento) dono do registro.
        modelBuilder.Entity<Responsible>()
            .HasQueryFilter(x => CurrentInstitutionId == null || x.Patient.InstitutionId == CurrentInstitutionId);

        modelBuilder.Entity<PatientClinicalCondition>()
            .HasQueryFilter(x => CurrentInstitutionId == null || x.Patient.InstitutionId == CurrentInstitutionId);

        modelBuilder.Entity<MedicinePatientClinicalCondition>()
            .HasQueryFilter(x =>
                CurrentInstitutionId == null ||
                x.PatientClinicalCondition.Patient.InstitutionId == CurrentInstitutionId);

        modelBuilder.Entity<PatientEmployee>()
            .HasQueryFilter(x => CurrentInstitutionId == null || x.Patient.InstitutionId == CurrentInstitutionId);

        modelBuilder.Entity<Payment>()
            .HasQueryFilter(x => CurrentInstitutionId == null || x.Appointment.InstitutionId == CurrentInstitutionId);

        // Cada instituicao so enxerga os logs dos proprios usuarios; o admin da
        // plataforma (sem instituicao) enxerga todos.
        modelBuilder.Entity<AccessLog>()
            .HasQueryFilter(x =>
                CurrentInstitutionId == null ||
                x.User.InstitutionId == CurrentInstitutionId
            );

        // ===== Mapeamento das tabelas =====
        // Tabela administracoes_medicamento (MedicationAdministration): registro de cada dose dada ou nao,
        // ligado ao medicamento programado, ao acolhido e ao funcionario.
        modelBuilder.Entity<MedicationAdministration>(entity =>
        {
            entity.ToTable("administracoes_medicamento");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_administracao")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.MedicinePatientClinicalConditionId)
                .HasColumnName("id_medicamento_acond");

            entity.Property(e => e.PatientId)
                .HasColumnName("id_acolhido");

            entity.Property(e => e.EmployeeId)
                .HasColumnName("id_funcionario");

            entity.Property(e => e.ScheduledDateTime)
                .HasColumnName("data_hora_prevista")
                .HasColumnType("datetime");

            entity.Property(e => e.AdministeredDateTime)
                .HasColumnName("data_hora_administrada")
                .HasColumnType("datetime");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(e => e.Observations)
                .HasColumnName("observacoes")
                .HasColumnType("text");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("data_cadastro")
                .HasColumnType("datetime");

            entity.HasOne(e => e.MedicinePatientClinicalCondition)
                .WithMany()
                .HasForeignKey(e => e.MedicinePatientClinicalConditionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        // Tabela acolhidos (Patient): CPF com indice unico (em todo o banco, nao por instituicao);
        // ligado ao tipo sanguineo e a instituicao.
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

            entity.Property(e => e.InstitutionId)
                .HasColumnName("id_instituicao");

            entity.HasOne(e => e.BloodType)
                .WithMany(e => e.Patients)
                .HasForeignKey(e => e.BloodTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Institution)
                .WithMany()
                .HasForeignKey(e => e.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Tabela instituicoes (Institution): clientes do SaaS; CNPJ com indice unico. Sem filtro por instituicao.
        modelBuilder.Entity<Institution>(entity =>
        {
            entity.ToTable("instituicoes");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_instituicao")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("nome")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(e => e.Cnpj)
                .HasColumnName("cnpj")
                .HasMaxLength(18);

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(150);

            entity.Property(e => e.Phone)
                .HasColumnName("telefone")
                .HasMaxLength(20);

            entity.Property(e => e.Active)
                .HasColumnName("ativa")
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("data_cadastro")
                .HasColumnType("datetime")
                .IsRequired();

            entity.HasIndex(e => e.Cnpj).IsUnique();

            entity.HasMany(e => e.Subscriptions)
                .WithOne(e => e.Institution)
                .HasForeignKey(e => e.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Tabela planos (Plan): planos de assinatura oferecidos pela plataforma.
        modelBuilder.Entity<Plan>(entity =>
        {
            entity.ToTable("planos");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_plano")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("nome")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasColumnName("descricao")
                .HasColumnType("text");

            entity.Property(e => e.MonthlyPrice)
                .HasColumnName("valor_mensal")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            entity.Property(e => e.MaxPatients)
                .HasColumnName("limite_acolhidos")
                .IsRequired();

            entity.Property(e => e.MaxUsers)
                .HasColumnName("limite_usuarios")
                .IsRequired();

            entity.Property(e => e.Active)
                .HasColumnName("ativo")
                .IsRequired();

            entity.HasMany(e => e.Subscriptions)
                .WithOne(e => e.Plan)
                .HasForeignKey(e => e.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Tabela assinaturas (Subscription): liga uma instituicao a um plano.
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.ToTable("assinaturas");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_assinatura")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.InstitutionId)
                .HasColumnName("id_instituicao")
                .IsRequired();

            entity.Property(e => e.PlanId)
                .HasColumnName("id_plano")
                .IsRequired();

            entity.Property(e => e.StartDate)
                .HasColumnName("data_inicio")
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(e => e.EndDate)
                .HasColumnName("data_fim")
                .HasColumnType("datetime");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(e => e.ExternalSubscriptionId)
                .HasColumnName("id_assinatura_externa")
                .HasMaxLength(150);

            entity.HasOne(e => e.Institution)
                .WithMany(e => e.Subscriptions)
                .HasForeignKey(e => e.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Plan)
                .WithMany(e => e.Subscriptions)
                .HasForeignKey(e => e.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.SubscriptionPayments)
                .WithOne(e => e.Subscription)
                .HasForeignKey(e => e.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Tabela pagamentos_assinatura (SubscriptionPayment): pagamentos de cada assinatura.
        modelBuilder.Entity<SubscriptionPayment>(entity =>
        {
            entity.ToTable("pagamentos_assinatura");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_pagamento_assinatura")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.SubscriptionId)
                .HasColumnName("id_assinatura")
                .IsRequired();

            entity.Property(e => e.Amount)
                .HasColumnName("valor")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            entity.Property(e => e.PaymentMethod)
                .HasColumnName("metodo_pagamento")
                .HasMaxLength(50);

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(e => e.PaymentDate)
                .HasColumnName("data_pagamento")
                .HasColumnType("datetime");

            entity.Property(e => e.ExternalPaymentId)
                .HasColumnName("id_pagamento_externo")
                .HasMaxLength(150);

            entity.Property(e => e.Observations)
                .HasColumnName("observacoes")
                .HasColumnType("text");

            entity.HasOne(e => e.Subscription)
                .WithMany(e => e.SubscriptionPayments)
                .HasForeignKey(e => e.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        // Tabela blood_types (BloodType): catalogo de tipos sanguineos, compartilhado (sem filtro).
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

        // Tabela responsaveis (Responsible): familiares/contatos de um acolhido.
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

        // Tabela departamentos (Department): setores da instituicao.
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

            entity.Property(e => e.InstitutionId)
                .HasColumnName("id_instituicao");

            entity.HasOne(e => e.Institution)
                .WithMany()
                .HasForeignKey(e => e.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Tabela funcionarios (Employee): CPF com indice unico; ligado ao setor e a instituicao.
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

            entity.Property(e => e.InstitutionId)
                .HasColumnName("id_instituicao");

            entity.HasOne(e => e.Department)
                .WithMany(e => e.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Institution)
                .WithMany()
                .HasForeignKey(e => e.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Tabela acolhido_funcionario (PatientEmployee): vinculo entre acolhido e funcionario.
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

        // Tabela condicoesclinicas (ClinicalCondition): catalogo unico de condicoes clinicas (sem filtro).
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

        // Tabela acolhido_condicaoclinica (PatientClinicalCondition): condicao clinica de um acolhido.
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

        

        

        // Tabela medicamentos (Medicine): medicamentos cadastrados por instituicao.
        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.ToTable("medicamentos");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id_medicamento")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.InstitutionId)
                .HasColumnName("id_instituicao");

            entity.HasOne(e => e.Institution)
                .WithMany()
                .HasForeignKey(e => e.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);
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

        // Tabela medicamento_acolhido_condicaoclinica (MedicinePatientClinicalCondition): medicamento
        // programado para a condicao clinica de um acolhido, com horario (coluna time) e funcionario responsavel.
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
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Frequency)
                .HasColumnName("frequencia")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.StartDate)
                .HasColumnName("data_inicio")
                .HasColumnType("date");

            entity.Property(e => e.EndDate)
                .HasColumnName("data_fim")
                .HasColumnType("date");

            entity.Property(e => e.Observations)
                .HasColumnName("observacoes")
                .HasColumnType("text")
                .IsRequired();

            entity.Property(e => e.AdministrationTime)
                .HasColumnName("horario_administracao")
                .HasColumnType("time");

            entity.Property(e => e.ResponsibleEmployeeId)
                .HasColumnName("id_funcionario_responsavel");

            entity.HasOne(e => e.Medicine)
                .WithMany(e => e.PatientClinicalConditionMedicines)
                .HasForeignKey(e => e.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PatientClinicalCondition)
                .WithMany(e => e.Medicines)
                .HasForeignKey(e => e.PatientClinicalConditionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ResponsibleEmployee)
                .WithMany()
                .HasForeignKey(e => e.ResponsibleEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Tabela contas_login (LoginAccount): contas de acesso. E-mail unico; UserId e chave alternativa
        // (referenciada por log_acesso, agendamentos, pagamentos e permissoes). Sem filtro global:
        // a restricao por instituicao e feita no LoginAccountApp.
        modelBuilder.Entity<LoginAccount>(entity =>
        {
            entity.ToTable("contas_login");

            entity.HasKey(e => e.Id);

            entity.HasAlternateKey(e => e.UserId);

            entity.Property(e => e.Id)
                .HasColumnName("id_conta_login")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UserId)
                .HasColumnName("id_usuario")
                .IsRequired();

            entity.Property(e => e.InstitutionId)
                .HasColumnName("id_instituicao");

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(150)
                .IsRequired();
            entity.Property(e => e.Name)
                     .HasColumnName("nome")
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
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.LastLogin)
                .HasColumnName("ultimo_login")
                .HasColumnType("datetime");

            entity.Property(e => e.Active)
                .HasColumnName("ativo")
                .HasDefaultValue(true)
                .IsRequired();

            entity.HasOne(e => e.Institution)
                .WithMany()
                .HasForeignKey(e => e.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Tabela permissoes_disponiveis (AvailablePermission): catalogo de permissoes.
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

        // Tabela usuarios_permissoes (UserPermission): permissoes atribuidas a cada conta (via UserId).
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
        // Tabela log_acesso (AccessLog): trilha de auditoria gravada automaticamente no SaveChanges.
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

        // Tabela agendamentos (Appointment): agendamentos de um acolhido, com usuario e instituicao.
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

            entity.Property(e => e.InstitutionId)
                .HasColumnName("id_instituicao");

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

            entity.HasOne(e => e.Institution)
                .WithMany()
                .HasForeignKey(e => e.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

      

        // Tabela pagamentos (Payment): pagamentos de agendamentos; id da transacao externa unico.
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

    /// <summary>
    /// Ponto de extensao (padrao do scaffold do EF) para configuracoes extras em outro arquivo partial;
    /// hoje nao ha implementacao, entao a chamada nao faz nada.
    /// </summary>
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}