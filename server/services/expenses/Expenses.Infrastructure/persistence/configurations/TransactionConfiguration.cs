using Expenses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Infrastructure.persistence.configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.OwnsOne(x => x.Amount, money =>
            {
                money.Property(m => m.Amount)
                        .HasColumnName("Amount")
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();

                money.Property(m => m.Currency)
                     .HasColumnName("Currency")
                     .HasMaxLength(3)
                     .IsRequired();
            });

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.ExpenseType)
                .IsRequired();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.TenantId)
                .IsRequired();

            builder.Property(x => x.CategoryId)
                .IsRequired();

            builder.Property(x => x.Date)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder
                .HasOne(x => x.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}