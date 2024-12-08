using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Domain.SistemaCadastroContext;

public partial class SistemaCadastroContext : DbContext
{
    public SistemaCadastroContext()
    {
    }

    public SistemaCadastroContext(DbContextOptions<SistemaCadastroContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BloodType> BloodTypes { get; set; }

    public virtual DbSet<Illness> Illnesses { get; set; }

    public virtual DbSet<Medicine> Medicines { get; set; }

    public virtual DbSet<MedicinePatientIllness> MedicinePatientIllnesses { get; set; }

    public virtual DbSet<MedicinePatientIllnessHistoric> MedicinePatientIllnessHistorics { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<PatientIllness> PatientIllnesses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=35.198.34.142;Database=tlcentral_dev;Username=tlpgsql;Password=f254afd0b0fb9ed7f66d142367981c37");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("dml_type", new[] { "INSERT", "UPDATE", "DELETE" })
            .HasPostgresExtension("unaccent")
            .HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<BloodType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Blood_Type_pkey");

            entity.ToTable("Blood_Type", "ngts");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Name).HasMaxLength(4);
        });

        modelBuilder.Entity<Illness>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Illness_pkey");

            entity.ToTable("Illness", "ngts");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("Name ");
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Medicine_pkey");

            entity.ToTable("Medicine", "ngts");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Name).HasColumnType("character varying");
        });

        modelBuilder.Entity<MedicinePatientIllness>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Medicine_Patient_Illness_pkey");

            entity.ToTable("Medicine_Patient_Illness", "ngts");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Dosage).HasMaxLength(255);
            entity.Property(e => e.IdMedicine).HasColumnName("Id_Medicine");
            entity.Property(e => e.IdPatientIllness).HasColumnName("Id_Patient_Illness");

            entity.HasOne(d => d.IdMedicineNavigation).WithMany(p => p.MedicinePatientIllnesses)
                .HasForeignKey(d => d.IdMedicine)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_medicine");

            entity.HasOne(d => d.IdPatientIllnessNavigation).WithMany(p => p.MedicinePatientIllnesses)
                .HasForeignKey(d => d.IdPatientIllness)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_patient_illness");
        });

        modelBuilder.Entity<MedicinePatientIllnessHistoric>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Medicine_Patient_Illness_Historic_pkey");

            entity.ToTable("Medicine_Patient_Illness_Historic", "ngts");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.IdMedicinePatientIllness).HasColumnName("Id_Medicine_Patient_Illness");
            entity.Property(e => e.LastTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("Last_Time");

            entity.HasOne(d => d.IdMedicinePatientIllnessNavigation).WithMany(p => p.MedicinePatientIllnessHistorics)
                .HasForeignKey(d => d.IdMedicinePatientIllness)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_medicine_patient_illness");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Patient_pkey");

            entity.ToTable("Patient", "ngts");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Document).HasMaxLength(11);
            entity.Property(e => e.IdBloodType).HasColumnName("Id_Blood_Type");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(255);
            entity.Property(e => e.Responsible).HasColumnType("character varying");

            entity.HasOne(d => d.IdBloodTypeNavigation).WithMany(p => p.Patients)
                .HasForeignKey(d => d.IdBloodType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_patient_blood_type");
        });

        modelBuilder.Entity<PatientIllness>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Patient_Illness_pkey");

            entity.ToTable("Patient_Illness", "ngts");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.IdIlleness).HasColumnName("Id_Illeness");
            entity.Property(e => e.IdPatient).HasColumnName("Id_Patient");

            entity.HasOne(d => d.IdIllenessNavigation).WithMany(p => p.PatientIllnesses)
                .HasForeignKey(d => d.IdIlleness)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_illness");

            entity.HasOne(d => d.IdPatientNavigation).WithMany(p => p.PatientIllnesses)
                .HasForeignKey(d => d.IdPatient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_patient");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
