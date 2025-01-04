using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TaskTip.Models.Entities;

public partial class TaskTipDbContext : DbContext
{
    private string _connectionString;
    public TaskTipDbContext()
    {
        var dir = AppDomain.CurrentDomain.BaseDirectory;
        string? sqliteDb = $"Data Source={dir}\\Resources\\TaskTipDB.db";
        _connectionString = sqliteDb;
    }

    public TaskTipDbContext(DbContextOptions<TaskTipDbContext> options,string connectionString)
        : base(options)
    {
        _connectionString = connectionString;
    }

    public virtual DbSet<BizRecord> BizRecords { get; set; }

    public virtual DbSet<BizRecordMenu> BizRecordMenus { get; set; }

    public virtual DbSet<BizRecordWork> BizRecordWorks { get; set; }

    public virtual DbSet<BizTask> BizTasks { get; set; }

    public virtual DbSet<SysParam> SysParams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BizRecord>(entity =>
        {
            entity.HasKey(e => e.Guid);

            entity.ToTable("biz_record");
        });

        modelBuilder.Entity<BizRecordMenu>(entity =>
        {
            entity.HasKey(e => e.Guid);

            entity.ToTable("biz_record_menu");

            entity.Property(e => e.Guid).HasColumnName("GUID");
            entity.Property(e => e.IsDirectory).HasColumnType("int(1)");
        });

        modelBuilder.Entity<BizRecordWork>(entity =>
        {
            entity.HasKey(e => e.Guid);

            entity.ToTable("biz_record_work");

            entity.Property(e => e.Guid).HasColumnName("GUID");
        });

        modelBuilder.Entity<BizTask>(entity =>
        {
            entity.HasKey(e => e.Guid);

            entity.ToTable("biz_task");

            entity.Property(e => e.Guid).HasColumnName("GUID");
            entity.Property(e => e.CompletedDateTime).HasColumnType("DATE");
            entity.Property(e => e.IsCompleted).HasColumnType("integer(1)");
            entity.Property(e => e.TaskTimePlan).HasColumnType("DATE");
        });

        modelBuilder.Entity<SysParam>(entity =>
        {
            entity.HasKey(e => e.Key);

            entity.ToTable("sys_param");

            entity.Property(e => e.LastUpdate).HasColumnType("DATE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
