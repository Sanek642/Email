using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Email
{
    public partial class emailContext : DbContext
    {
        public emailContext()
        {
        }

        public emailContext(DbContextOptions<emailContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Emailtb> Emailtbs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var spath = Application.StartupPath + @"DB\email.db";
                optionsBuilder.UseSqlite($" Data Source= {spath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Emailtb>(entity =>
            {
                entity.ToTable("Emailtb");

                entity.HasIndex(e => e.Email, "IX_Emailtb_Email")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("Integer")
                    .HasColumnName("ID");

                entity.Property(e => e.DataMes)
                    .HasColumnType("DateTime")
                    .HasColumnName("Data_mes");

                entity.Property(e => e.Email).HasColumnType("Text");

                entity.Property(e => e.Fio)
                    .HasColumnType("Text")
                    .HasColumnName("FIO");

                entity.Property(e => e.Pr).HasColumnType("Integer");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
