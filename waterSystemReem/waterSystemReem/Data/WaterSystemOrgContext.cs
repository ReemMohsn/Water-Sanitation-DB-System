using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using waterSystemReem.Models;

namespace waterSystemReem.DATA
{
    public partial class WaterSystemOrgContext : DbContext
    {
        public WaterSystemOrgContext()
        {
        }

        public WaterSystemOrgContext(DbContextOptions<WaterSystemOrgContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Counter> Counters { get; set; } = null!;
        public virtual DbSet<CounterDetailsView> CounterDetailsViews { get; set; } = null!;
        public virtual DbSet<CounterType> CounterTypes { get; set; } = null!;
        public virtual DbSet<GetCounterDatesFirstStage> GetCounterDatesFirstStages { get; set; } = null!;
        public virtual DbSet<Invoice> Invoices { get; set; } = null!;
        public virtual DbSet<InvoiceCounterView> InvoiceCounterViews { get; set; } = null!;
        public virtual DbSet<Subscriber> Subscribers { get; set; } = null!;
        public virtual DbSet<SubscriberCountersView> SubscriberCountersViews { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=REEM\\G2;database= WaterSystemOrg;trusted_connection=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.ToTable("counters");

                entity.HasIndex(e => e.CounterLocation, "IX_Counters_CounterLocation");

                entity.HasIndex(e => e.CounterNumber, "IX_Counters_CounterNumber");

                entity.Property(e => e.CounterId).HasColumnName("counter_id");

                entity.Property(e => e.CounterLocation)
                    .HasMaxLength(50)
                    .HasColumnName("counter_location");

                entity.Property(e => e.CounterNumber).HasColumnName("counter_number");

                entity.Property(e => e.CounterTypeId).HasColumnName("counter_type_id");

                entity.Property(e => e.DateLastReading)
                    .HasColumnType("date")
                    .HasColumnName("date_last_reading");

                entity.Property(e => e.PreviseBalance).HasColumnName("previse_balance");

                entity.Property(e => e.SubscriberId).HasColumnName("subscriber_id");

                entity.HasOne(d => d.CounterType)
                    .WithMany(p => p.Counters)
                    .HasForeignKey(d => d.CounterTypeId)
                    .HasConstraintName("FK_counters_counter_type");

                entity.HasOne(d => d.Subscriber)
                    .WithMany(p => p.Counters)
                    .HasForeignKey(d => d.SubscriberId)
                    .HasConstraintName("FK_counters_subscribers");
            });

            modelBuilder.Entity<CounterDetailsView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("CounterDetailsView");

                entity.Property(e => e.CounterId).HasColumnName("counter_id");

                entity.Property(e => e.CounterLocation)
                    .HasMaxLength(50)
                    .HasColumnName("counter_location");

                entity.Property(e => e.CounterNumber).HasColumnName("counter_number");

                entity.Property(e => e.CounterTypeName)
                    .HasMaxLength(50)
                    .HasColumnName("counter_type_name");

                entity.Property(e => e.DateLastReading)
                    .HasColumnType("date")
                    .HasColumnName("date_last_reading");

                entity.Property(e => e.LocalComcils).HasColumnName("local_comcils");

                entity.Property(e => e.MeterRang).HasColumnName("meter_rang");

                entity.Property(e => e.PreviseBalance).HasColumnName("previse_balance");

                entity.Property(e => e.SubscriberName)
                    .HasMaxLength(50)
                    .HasColumnName("subscriber_name");
            });

            modelBuilder.Entity<CounterType>(entity =>
            {
                entity.ToTable("counter_type");

                entity.HasIndex(e => e.CounterTypeName, "IX_CounterType_CounterTypeName");

                entity.Property(e => e.CounterTypeId).HasColumnName("counter_type_id");

                entity.Property(e => e.CounterTypeName)
                    .HasMaxLength(50)
                    .HasColumnName("counter_type_name");

                entity.Property(e => e.LocalComcils).HasColumnName("local_comcils");

                entity.Property(e => e.MeterRang).HasColumnName("meter_rang");
            });

            modelBuilder.Entity<GetCounterDatesFirstStage>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("GetCounterDatesFirstStage");

                entity.Property(e => e.CounterId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("counter_id");

                entity.Property(e => e.FromDate).HasColumnType("date");

                entity.Property(e => e.ToDate).HasColumnType("date");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("invoices");

                entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");

                entity.Property(e => e.Balance).HasColumnName("balance");

                entity.Property(e => e.CounterId).HasColumnName("counter_id");

                entity.Property(e => e.FromDate)
                    .HasColumnType("date")
                    .HasColumnName("from_date");

                entity.Property(e => e.IsPaied).HasColumnName("is_paied");

                entity.Property(e => e.ToData)
                    .HasColumnType("date")
                    .HasColumnName("to_data");

                entity.HasOne(d => d.Counter)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.CounterId)
                    .HasConstraintName("FK_invoices_counters");
            });

            modelBuilder.Entity<InvoiceCounterView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("InvoiceCounterView");

                entity.Property(e => e.Balance).HasColumnName("balance");

                entity.Property(e => e.CounterId).HasColumnName("counter_id");

                entity.Property(e => e.CounterLocation)
                    .HasMaxLength(50)
                    .HasColumnName("counter_location");

                entity.Property(e => e.CounterNumber).HasColumnName("counter_number");

                entity.Property(e => e.CounterTypeName)
                    .HasMaxLength(50)
                    .HasColumnName("counter_type_name");

                entity.Property(e => e.FromDate)
                    .HasColumnType("date")
                    .HasColumnName("from_date");

                entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");

                entity.Property(e => e.IsPaied).HasColumnName("is_paied");

                entity.Property(e => e.LocalComcils).HasColumnName("local_comcils");

                entity.Property(e => e.MeterRang).HasColumnName("meter_rang");

                entity.Property(e => e.SubscriberName)
                    .HasMaxLength(50)
                    .HasColumnName("subscriber_name");

                entity.Property(e => e.ToData)
                    .HasColumnType("date")
                    .HasColumnName("to_data");
            });

            modelBuilder.Entity<Subscriber>(entity =>
            {
                entity.ToTable("subscribers");

                entity.Property(e => e.SubscriberId).HasColumnName("subscriber_id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("date")
                    .HasColumnName("create_date");

                entity.Property(e => e.IdentificationNumber).HasColumnName("identification_number");

                entity.Property(e => e.SubscriberName)
                    .HasMaxLength(50)
                    .HasColumnName("subscriber_name");

                entity.Property(e => e.SubscriberPhone)
                    .HasMaxLength(9)
                    .HasColumnName("subscriber_phone")
                    .IsFixedLength();
            });

            modelBuilder.Entity<SubscriberCountersView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("SubscriberCountersView");

                entity.Property(e => e.CounterId).HasColumnName("counter_id");

                entity.Property(e => e.CounterLocation)
                    .HasMaxLength(50)
                    .HasColumnName("counter_location");

                entity.Property(e => e.CounterNumber).HasColumnName("counter_number");

                entity.Property(e => e.CounterTypeId).HasColumnName("counter_type_id");

                entity.Property(e => e.CounterTypeName)
                    .HasMaxLength(50)
                    .HasColumnName("counter_type_name");

                entity.Property(e => e.IdentificationNumber).HasColumnName("identification_number");

                entity.Property(e => e.PreviseBalance).HasColumnName("previse_balance");

                entity.Property(e => e.SubscriberId).HasColumnName("subscriber_id");

                entity.Property(e => e.SubscriberName)
                    .HasMaxLength(50)
                    .HasColumnName("subscriber_name");

                entity.Property(e => e.SubscriberPhone)
                    .HasMaxLength(9)
                    .HasColumnName("subscriber_phone")
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
