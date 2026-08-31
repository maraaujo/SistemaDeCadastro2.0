using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Domain.Configurations
{
    public class MedicinePatientClinicalConditionConfiguration : IEntityTypeConfiguration<MedicinePatientClinicalCondition>
    {
        public void Configure(EntityTypeBuilder<MedicinePatientClinicalCondition> builder)
        {
            var saoPaulo = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(v, DateTimeKind.Unspecified), saoPaulo),
                v => TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(v, DateTimeKind.Utc), saoPaulo));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(v.Value, DateTimeKind.Unspecified), saoPaulo) : (DateTime?)null,
                v => v.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(v.Value, DateTimeKind.Utc), saoPaulo) : (DateTime?)null);

            builder.Property(e => e.AdministrationTime)
                   .HasColumnType("time");

            builder.Property(e => e.StartDate)
                   .HasConversion(dateTimeConverter)
                   .HasColumnType("timestamp with time zone");

            builder.Property(e => e.EndDate)
                   .HasConversion(nullableDateTimeConverter)
                   .HasColumnType("timestamp with time zone");
        }
    }
}
