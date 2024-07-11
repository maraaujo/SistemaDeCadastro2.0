using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Model;

namespace SistemaDeCadastro.Domain.Context
{
    public class SistemaDeCadastroContext : IdentityDbContext
    {
        public SistemaDeCadastroContext(DbContextOptions<SistemaDeCadastroContext> options)
        : base(options)
        {
        }

        public virtual DbSet<Idoso> Idosos {  get; set; }
        public virtual DbSet<Familia> Familias { get; set; }
        public virtual DbSet<Funcionario> Funcionarios { get; set; }
        public virtual DbSet<Departamento> Departamentos { get; set;}
        public virtual DbSet<Medicamento> Medicamentos { get; set;}
        public virtual DbSet<Doenca> Doenca { get; set; }
        public  virtual DbSet<MedicamentoIdosoDoenca> MedicamentoIdosoDoencas { get; set; }
        public virtual DbSet<IdosoDoenca> IdososDoenca { get; set; }
        public virtual DbSet<IdosoFuncionario> IdosoFuncionarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var conn = "Host=localhost;Database=sistemadecadastro;Username=root;Password=irmaos03;Convert Zero Datetime=True";
                var serverVersion = ServerVersion.AutoDetect(conn);
                optionsBuilder.UseMySql(conn, serverVersion);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Idoso>(entity =>
            {
                entity.ToTable("idoso");
                entity.Property(e => e.Id)
                .HasColumnName("Id");

                entity.Property(e => e.Nome)
                .HasMaxLength(45)
                .HasColumnName("Nome");

                entity.Property(e => e.Sobrenome)
                .HasMaxLength(45)
                .HasColumnName("Sobrenome");

                entity.Property(e => e.DataDeNascimento)
               .HasColumnType("timestamp(0) without time zone")
               .HasColumnName("DataDeNascimento");

                entity.Property(e => e.Cpf)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("Cpf");

               
            });

            modelBuilder.Entity<Familia>(entity =>
            {
                entity.ToTable( "familia"); 
                entity.Property(e => e.Id)  
                .HasColumnName("Id");

                entity.Property(e => e.Nome)
                 .HasMaxLength(45)
                .HasColumnName("Nome");

                entity.Property(e => e.Sobrenome)
               .HasMaxLength(45)
               .HasColumnName("Sobrenome");

                entity.Property(e => e.Numero)
                .HasMaxLength(45)
                .HasColumnName("Numero");

                entity.Property(e => e.Endereco)
                .HasMaxLength(45)
                .HasColumnName("Endereco");

                //perguntar Felipe
                entity.HasOne(d => d.Idoso)
                .WithMany(p => p.Familias)
                .HasForeignKey(d => d.IdosoId);
            });

            modelBuilder.Entity<Funcionario>(entity =>
            {
                entity.ToTable("funcionario");
                entity.Property(e => e.Id)
                .HasColumnName("Id");

                entity.Property(e => e.Nome)
                .HasMaxLength(45)
               .HasColumnName("Nome");

               entity.Property(e => e.Sobrenome)
              .HasMaxLength(45)
              .HasColumnName("Sobrenome");

               entity.Property(e => e.Documento)
              .HasMaxLength(45)
              .HasColumnName("Documento");
                entity.Property(e => e.Email)
             .HasMaxLength(255)
             .HasColumnName("email");
                entity.Property(e => e.Senha)
             .HasMaxLength(45)
             .HasColumnName("senha");

                entity.HasOne(d => d.Departamento)
                    .WithMany(d => d.Funcionarios)
                    .HasForeignKey(d => d.DepartamentoId);
                 
            });

            modelBuilder.Entity<Departamento>(entity =>
            {
                entity.ToTable("Departamento");
                entity.Property(e=>e.Id)
                .HasColumnName("Id");

                entity.Property(e => e.Nome)
                 .HasMaxLength(45)
                 .HasColumnName("Nome");
            });

            modelBuilder.Entity<Medicamento>(entity =>
            {
                entity.ToTable("medicamento");
                entity.Property(e=>e.Id) 
                .HasColumnName("Id");

                entity.Property(e => e.Nome)
                .HasMaxLength(255)
                .HasColumnName("Nome");

                entity.Property(e => e.Descricao)
                .HasMaxLength(255)
                .HasColumnName("Descricao");

            });

            modelBuilder.Entity<Doenca>(entity => {
                entity.ToTable("doenca");
                entity.Property(e=>e.Id) 
                .HasColumnName("Id");

                entity.Property(e =>e.Nome)
                .HasMaxLength(45)
                 .HasColumnName("Nome");
            });

            modelBuilder.Entity<IdosoFuncionario>(entity =>
            {
                entity.ToTable("Idoso_Funcionario");
                entity.Property(e => e.Id)
               .HasColumnName("Id");

                entity.HasOne(d => d.Idosos)
                .WithMany(d => d.Funcionarios)
                .HasForeignKey(d => d.IdosoID);

                //um idoso tem varios funcioanrio e a chave estrangeira é o IdosoId

                entity.HasOne(d => d.Funcionarios)
                .WithMany(p => p.Idosos)
                .HasForeignKey(d => d.FuncionarioID);
                //um funcionairo possui vários idosos e tem a chave estrangeira FuncionarioID
            });
            modelBuilder.Entity<MedicamentoIdosoDoenca>(entity =>
            {
                entity.ToTable("medicamento_idoso_doenca");
                entity.Property(e => e.Id)
               .HasColumnName("Id");

                entity.Property(e => e.MedicamentoDosagem)
                .HasColumnName("Medicamento_Dosagem");

                entity.HasOne(d => d.Medicamentos)
                .WithMany(d => d.MedicamentoIdosoDoencas)
                .HasForeignKey(d => d.MedicamentoId);
                //Tem um medicamento que possui várias doencas FK MedicamentoId

                entity.HasOne(d => d.Idoso)
               .WithMany(d => d.MedicamentoIdosoDoencas)
               .HasForeignKey(d => d.IdosoId);

                entity.HasOne(d => d.Doencas)
            .WithMany(d => d.MedicamentoIdosoDoencas)
            .HasForeignKey(d => d.DoencaId);
            });
            modelBuilder.Entity<IdosoDoenca>(entity =>
            {
                entity.ToTable("idoso_doenca");
                entity.Property(e => e.Id)
               .HasColumnName("Id");

                entity.HasOne(d => d.Doencas)
                  .WithMany(d => d.IdosoDoencas)
                  .HasForeignKey(d => d.DoencaId);

                entity.HasOne(d => d.Idosos)
                 .WithMany(d => d.IdosoDoencas)
                 .HasForeignKey(d => d.IdosoId);

            });
        }
    }
}
