using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SemiConductor_Equipment.Models
{
    public partial class LogDatabaseContext : DbContext
    {
        public LogDatabaseContext()
        {
        }

        public LogDatabaseContext(DbContextOptions<LogDatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Logtable> Logtables { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=LogDatabase;Username=postgres;Password=dlsxj159");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("pg_catalog", "adminpack");

            modelBuilder.Entity<Logtable>(entity =>
            {
                entity.HasKey(e => e.LogData).HasName("gangnamgu_population_pkey"); //추가 필

                entity.ToTable("logtable");

                entity.Property(e => e.LogDateTime)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("time");
                entity.Property(e => e.LogData)
                     .HasColumnType("character varying")
                    .HasColumnName("log");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
