using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Bookstore.Infrastructure.Data;

public class BookstoreDbContextFactory : IDesignTimeDbContextFactory<BookstoreDbContext>
{
    public BookstoreDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BookstoreDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost, 1401;Database=BookstoreDB;User Id=sa;Password=Dev@123456;TrustServerCertificate=True;");

        return new BookstoreDbContext(optionsBuilder.Options);
    }
}