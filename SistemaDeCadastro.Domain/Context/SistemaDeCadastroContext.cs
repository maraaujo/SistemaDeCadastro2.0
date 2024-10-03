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

        public virtual DbSet<Pessoa> Pessoas { get; set; }
        public virtual DbSet<TipoPessoa> TiposPessoa { get; set; }
        public virtual DbSet<PessoaTipoPessoa> PessoaTipoPessoas { get; set; }
        public virtual DbSet<Laboratorio> Laboratorios { get; set; }
        public virtual DbSet<Medicamento> Medicamentos { get; set; }
        public virtual DbSet<Morbidade> Morbidades { get; set; }
        public virtual DbSet<Vias> Vias { get; set; }
        public virtual DbSet<Posologia> Posologias { get; set; }
        public virtual DbSet<MedicamentoMorbidade> MedicamentoMorbidades { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var conn = "Host=localhost;Database=mydb;Username=root;Password=irmaos03;Convert Zero Datetime=True";
                var serverVersion = ServerVersion.AutoDetect(conn);
                optionsBuilder.UseMySql(conn, serverVersion);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Pessoa>(entity =>
            {
                entity.ToTable("Pessoa");
                entity.HasKey(e => e.Cod);

                entity.Property(e => e.Cod).HasColumnName("cod");
                entity.Property(e => e.Nome).HasMaxLength(45).HasColumnName("nome");
                entity.Property(e => e.Cpf).HasMaxLength(45).HasColumnName("cpf");
                entity.Property(e => e.TipoSanguineo).HasMaxLength(5).HasColumnName("tipo_sanguineo");
                entity.Property(e => e.Endereco).HasMaxLength(45).HasColumnName("endereco");
                entity.Property(e => e.DataObito).HasMaxLength(45).HasColumnName("data_obito");

                entity.HasOne(e => e.PessoasPai)
                    .WithMany(p => p.PessoasFilhos)
                    .HasForeignKey(e => e.PessoaCod)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<TipoPessoa>(entity =>
            {
                entity.ToTable("Tipo_Pessoa");
                entity.HasKey(e => e.Cod);

                entity.Property(e => e.Cod).HasColumnName("cod");
                entity.Property(e => e.Nome).HasMaxLength(55).HasColumnName("nome");
            });

            modelBuilder.Entity<PessoaTipoPessoa>(entity =>
            {
                entity.ToTable("Pessoa_TipoPessoa");
                entity.HasKey(e => new { e.Cod, e.CodPessoa, e.CodTipoPessoa });

                entity.HasOne(e => e.Pessoa)
                    .WithMany(p => p.PessoaTipoPessoas)
                    .HasForeignKey(e => new { e.CodPessoa })
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.TipoPessoa)
                    .WithMany(t => t.PessoaTipoPessoas)
                    .HasForeignKey(e => e.CodTipoPessoa)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Laboratorio>(entity =>
            {
                entity.ToTable("Laboratorio");
                entity.HasKey(e => e.Cod);

                entity.Property(e => e.Cod).HasColumnName("cod");
                entity.Property(e => e.Nome).HasMaxLength(45).HasColumnName("nome");
            });

            modelBuilder.Entity<Medicamento>(entity =>
            {
                entity.ToTable("Medicamento");
                entity.HasKey(e => e.Cod);

                entity.Property(e => e.Cod).HasColumnName("cod");
                entity.Property(e => e.Nome).HasMaxLength(255).HasColumnName("nome");

                entity.HasOne(e => e.Laboratorio)
                    .WithMany(l => l.Medicamentos)
                    .HasForeignKey(e => e.LaboratorioCod)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Morbidade>(entity =>
            {
                entity.ToTable("Morbidade");
                entity.HasKey(e => e.Cod);

                entity.HasOne(e => e.Pessoa)
                    .WithMany(p => p.Morbidades)
                    .HasForeignKey(e => e.PessoaCod)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Vias>(entity =>
            {
                entity.ToTable("Vias");
                entity.HasKey(e => e.Cod);

                entity.Property(e => e.Cod).HasColumnName("cod");
                entity.Property(e => e.Nome).HasMaxLength(45).HasColumnName("nome");
            });

            modelBuilder.Entity<Posologia>(entity =>
            {
                entity.ToTable("Posologia");
                entity.HasKey(e => e.Cod);

                entity.Property(e => e.DataInicial).HasColumnName("data_inicial");
                entity.Property(e => e.Dose).HasMaxLength(45).HasColumnName("dose");
                entity.Property(e => e.DataFinal).HasColumnName("data_final");
                entity.Property(e => e.Intervalo).HasColumnName("intervalo");

                entity.HasOne(e => e.Pessoa)
                    .WithMany(p => p.Posologias)
                    .HasForeignKey(e => e.PessoaCod)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Medicamento)
                    .WithMany(m => m.Posologias)
                    .HasForeignKey(e => e.MedicamentoCod)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Vias)
                    .WithMany(v => v.Posologias)
                    .HasForeignKey(e => e.ViasCod)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<MedicamentoMorbidade>(entity =>
            {
                entity.ToTable("Medicamento_Morbidade");
                entity.HasKey(e => new { e.MedicamentoCod, e.MorbidadeCod });

                entity.HasOne(e => e.Medicamento)
                    .WithMany(m => m.MedicamentoMorbidades)
                    .HasForeignKey(e => e.MedicamentoCod)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Morbidade)
                    .WithMany(m => m.MedicamentoMorbidades)
                    .HasForeignKey(e => e.MorbidadeCod)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
