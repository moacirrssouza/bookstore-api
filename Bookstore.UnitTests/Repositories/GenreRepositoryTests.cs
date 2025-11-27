using Bookstore.Domain.Entities;
using Bookstore.Infrastructure.Data;
using Bookstore.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Bookstore.UnitTests.Repositories;

public class GenreRepositoryTests
{
    [Fact]
    public async Task CreateAsync_PersistsGenre()
    {
        await using var context = CreateContext();
        var repository = new GenreRepository(context);
        var genre = new Genre("Terror");

        await repository.CreateAsync(genre, CancellationToken.None);

        var stored = await context.Genres.SingleAsync();
        stored.Name.Should().Be("Terror");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsItems()
    {
        await using var context = CreateContext();
        context.Genres.AddRange(new Genre("Drama"), new Genre("Action"));
        await context.SaveChangesAsync();
        var repository = new GenreRepository(context);

        var result = await repository.GetAllAsync(CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ChangesName()
    {
        await using var context = CreateContext();
        var genre = new Genre("Old");
        context.Genres.Add(genre);
        await context.SaveChangesAsync();
        var repository = new GenreRepository(context);

        genre.UpdateName("New");
        await repository.UpdateAsync(genre, CancellationToken.None);

        (await context.Genres.SingleAsync()).Name.Should().Be("New");
    }

    private static BookstoreDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<BookstoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new BookstoreDbContext(options);
    }
}

