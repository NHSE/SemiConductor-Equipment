using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SemiConductor_Equipment.Models;

public partial class LogDatabaseContext : DbContext
{
    public LogDatabaseContext()
    {
    }

    public LogDatabaseContext(DbContextOptions<LogDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chamberlogtable> Chamberlogtables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=LogDatabase;Username=postgres;Password=dlsxj159");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chamberlogtable>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("chamberlogtable");

            entity.Property(e => e.ChamberName)
                .HasColumnType("character varying")
                .HasColumnName("chamber_name");
            entity.Property(e => e.Logdata)
                .HasColumnType("character varying")
                .HasColumnName("logdata");
            entity.Property(e => e.LotId)
                .HasColumnType("character varying")
                .HasColumnName("lot_id");
            entity.Property(e => e.Slot).HasColumnName("slot");
            entity.Property(e => e.State)
                .HasColumnType("character varying")
                .HasColumnName("state");
            entity.Property(e => e.Time).HasColumnName("time");
            entity.Property(e => e.WaferId)
                .HasColumnType("character varying")
                .HasColumnName("wafer_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
