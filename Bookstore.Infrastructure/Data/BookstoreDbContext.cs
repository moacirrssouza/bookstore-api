using Bookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Infrastructure.Data;

public class BookstoreDbContext : DbContext
{
    public BookstoreDbContext(DbContextOptions<BookstoreDbContext> options) : base(options) { }

    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Genre> Genres { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Author>(b => {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<Genre>(b => {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Book>(b => {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(500);
            b.Property(x => x.Description).HasMaxLength(2000);

            b.HasOne(x => x.Author).WithMany(a => a.Books).HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Genre).WithMany(g => g.Books).HasForeignKey(x => x.GenreId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}