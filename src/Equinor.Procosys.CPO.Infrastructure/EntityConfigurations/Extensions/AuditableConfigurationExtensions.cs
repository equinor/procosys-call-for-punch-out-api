﻿using Equinor.Procosys.CPO.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.CPO.Domain.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.CPO.Infrastructure.EntityConfigurations.Extensions
{
    public static class AuditableConfigurationExtensions
    {
        public static void ConfigureCreationAudit<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, ICreationAuditable
        {
            builder
                .Property(x => x.CreatedAtUtc)
                .HasConversion(CPOContext.DateTimeKindConverter);

            builder
                .HasOne<Person>()
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);
        }

        public static void ConfigureModificationAudit<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IModificationAuditable
        {
            builder
                .Property(x => x.ModifiedAtUtc)
                .HasConversion(CPOContext.DateTimeKindConverter);

            builder
                .HasOne<Person>()
                .WithMany()
                .HasForeignKey(x => x.ModifiedById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
