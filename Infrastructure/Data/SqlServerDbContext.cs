using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public partial class SqlServerDbContext : DbContext
{
    public SqlServerDbContext()
    {
    }

    public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TodoEntity> TodoEntities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("data source=John;initial catalog=moolahTodo;trusted_connection=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Items__3213E83F5FB52011");

            entity.ToTable("Items", "Todo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.Guid)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("guid");
            entity.Property(e => e.IsComplete).HasColumnName("is_complete");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
