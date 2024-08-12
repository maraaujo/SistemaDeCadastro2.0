using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Model;

namespace SistemaDeCadastro.Domain.Context
{
    public class SistemaDeCadastroContext : IdentityDbContext<Usuario>
    {
        public SistemaDeCadastroContext(DbContextOptions<SistemaDeCadastroContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Idoso> Idosos { get; set; }
        public virtual DbSet<Familia> Familias { get; set; }
        public virtual DbSet<Funcionario> Funcionarios { get; set; }
        public virtual DbSet<Departamento> Departamentos { get; set; }
        public virtual DbSet<Medicamento> Medicamentos { get; set; }
        public virtual DbSet<Doenca> Doenca { get; set; }
        public virtual DbSet<MedicamentoIdosoDoenca> MedicamentoIdosoDoencas { get; set; }
        public virtual DbSet<IdosoDoenca> IdososDoenca { get; set; }
        public virtual DbSet<IdosoFuncionario> IdosoFuncionarios { get; set; }
        public virtual DbSet<Ministracao> Ministracoes { get; set; }

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
                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Nome)
                    .HasMaxLength(45)
                    .HasColumnName("Nome");

                entity.Property(e => e.Sobrenome)
                    .HasMaxLength(45)
                    .HasColumnName("Sobrenome");

                entity.Property(e => e.DataDeNascimento)
                    .HasColumnType("timestamp")
                    .HasColumnName("DataDeNascimento");

                entity.Property(e => e.Cpf)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("Cpf");

                entity.Property(e => e.TipoSanguineo)
                    .HasMaxLength(3)
                    .HasColumnName("TipoSanguineo");

                entity.Property(e => e.CodigoBarras)
                    .HasMaxLength(45)
                    .HasColumnName("CodigoBarras");

                entity.Property(e => e.RestricoesAlimentares)
                    .HasMaxLength(255)
                    .HasColumnName("RestricoesAlimentares");
            });

            modelBuilder.Entity<Familia>(entity =>
            {
                entity.ToTable("familia");
                entity.Property(e => e.Id).HasColumnName("Id");

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

                entity.Property(e => e.DepartamentoId)
                    .HasColumnName("Departamento_ID");

                entity.HasOne(d => d.Departamento)
                    .WithMany(p => p.Funcionarios)
                    .HasForeignKey(d => d.DepartamentoId);
            });

            modelBuilder.Entity<Departamento>(entity =>
            {
                entity.ToTable("Departamento");
                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Nome)
                    .HasMaxLength(45)
                    .HasColumnName("Nome");
            });

            modelBuilder.Entity<Medicamento>(entity =>
            {
                entity.ToTable("medicamento");
                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Nome)
                    .HasMaxLength(255)
                    .HasColumnName("Nome");

                entity.Property(e => e.Descricao)
                    .HasMaxLength(255)
                    .HasColumnName("Descricao");

                entity.Property(e => e.Laboratorio)
                    .HasMaxLength(255)
                    .HasColumnName("Laboratorio");

                entity.Property(e => e.UnidadeMedida)
                    .HasMaxLength(50)
                    .HasColumnName("UnidadeMedida");

                entity.Property(e => e.Valor)
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("Valor");
            });

            modelBuilder.Entity<Doenca>(entity =>
            {
                entity.ToTable("doenca");
                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Nome)
                    .HasMaxLength(45)
                    .HasColumnName("Nome");
            });

            modelBuilder.Entity<IdosoFuncionario>(entity =>
            {
                entity.ToTable("Idoso_Funcionario");
                entity.Property(e => e.Id).HasColumnName("Id");

                entity.HasOne(d => d.Idoso)
                    .WithMany(d => d.Funcionarios)
                    .HasForeignKey(d => d.IdosoID);

                entity.HasOne(d => d.Funcionario)
                    .WithMany(p => p.Idosos)
                    .HasForeignKey(d => d.FuncionarioID);
            });

            modelBuilder.Entity<MedicamentoIdosoDoenca>(entity =>
            {
                entity.ToTable("medicamento_idoso_doenca");
                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.MedicamentoDosagem).HasColumnName("Medicamento_Dosagem");

                entity.HasOne(d => d.Medicamentos)
                    .WithMany(d => d.MedicamentoIdosoDoencas)
                    .HasForeignKey(d => d.MedicamentoId);

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
                entity.Property(e => e.Id).HasColumnName("Id");

                entity.HasOne(d => d.Doencas)
                    .WithMany(d => d.IdosoDoencas)
                    .HasForeignKey(d => d.DoencaId);

                entity.HasOne(d => d.Idosos)
                    .WithMany(d => d.IdosoDoencas)
                    .HasForeignKey(d => d.IdosoId);
            });

            modelBuilder.Entity<Ministracao>(entity =>
            {
                entity.ToTable("ministracao");
                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Posologia)
                    .HasMaxLength(255)
                    .HasColumnName("Posologia");

                entity.Property(e => e.Data)
                    .HasColumnType("timestamp")
                    .HasColumnName("Data");

                entity.HasOne(d => d.Idoso)
                    .WithMany(p => p.Ministracoes)
                    .HasForeignKey(d => d.IdosoId);

                entity.HasOne(d => d.Medicamento)
                    .WithMany(p => p.Ministracoes)
                    .HasForeignKey(d => d.MedicamentoId);
            });
        }
    }
}
