using Bookstore.Domain.Entities;
using Bookstore.Infrastructure.Data;
using Bookstore.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.UnitTests.Repositories;

public class AuthorRepositoryTests
{
    [Fact]
    public async Task CreateAsync_PersistsAuthor()
    {
        await using var context = CreateContext();
        var repository = new AuthorRepository(context);
        var author = new Author("Test Author");

        await repository.CreateAsync(author, CancellationToken.None);

        var stored = await context.Authors.SingleAsync();
        stored.Name.Should().Be("Test Author");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsAuthor_WhenExists()
    {
        await using var context = CreateContext();
        var author = new Author("Existing");
        context.Authors.Add(author);
        await context.SaveChangesAsync();
        var repository = new AuthorRepository(context);

        var found = await repository.GetByIdAsync(author.Id, CancellationToken.None);

        found.Should().NotBeNull();
        found!.Name.Should().Be("Existing");
    }

    [Fact]
    public async Task DeleteAsync_RemovesEntity_WhenExists()
    {
        await using var context = CreateContext();
        var author = new Author("Removable");
        context.Authors.Add(author);
        await context.SaveChangesAsync();
        var repository = new AuthorRepository(context);

        await repository.DeleteAsync(author.Id, CancellationToken.None);

        (await context.Authors.CountAsync()).Should().Be(0);
    }

    private static BookstoreDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<BookstoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new BookstoreDbContext(options);
    }
}

