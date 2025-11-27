using Bookstore.Domain.Entities;
using Bookstore.Infrastructure.Data;
using Bookstore.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Bookstore.UnitTests.Repositories;

public class BookRepositoryTests
{
    [Fact]
    public async Task CreateAsync_PersistsBookWithRelations()
    {
        await using var context = CreateContext();
        var author = new Author("Author");
        var genre = new Genre("Genre");
        context.Authors.Add(author);
        context.Genres.Add(genre);
        await context.SaveChangesAsync();
        var repository = new BookRepository(context);
        var book = new Book("Book", author.Id, genre.Id, "desc");

        await repository.CreateAsync(book, CancellationToken.None);

        var stored = await context.Books.Include(b => b.Author).Include(b => b.Genre).SingleAsync();
        stored.AuthorId.Should().Be(author.Id);
        stored.GenreId.Should().Be(genre.Id);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsBooksWithNavigations()
    {
        await using var context = CreateContext();
        var author = new Author("Author");
        var genre = new Genre("Genre");
        context.Authors.Add(author);
        context.Genres.Add(genre);
        var book = new Book("Book", author.Id, genre.Id, "desc");
        context.Books.Add(book);
        await context.SaveChangesAsync();
        var repository = new BookRepository(context);

        var result = await repository.GetAllAsync(CancellationToken.None);

        result.Should().ContainSingle();
        var loaded = result.First();
        loaded.Author.Should().NotBeNull();
        loaded.Genre.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_ModifiesStoredEntity()
    {
        await using var context = CreateContext();
        var author = new Author("Author");
        var genre = new Genre("Genre");
        context.Authors.Add(author);
        context.Genres.Add(genre);
        var book = new Book("Original", author.Id, genre.Id, "desc");
        context.Books.Add(book);
        await context.SaveChangesAsync();
        var repository = new BookRepository(context);

        book.Update("Updated", author.Id, genre.Id, "new");
        await repository.UpdateAsync(book, CancellationToken.None);

        (await context.Books.SingleAsync()).Name.Should().Be("Updated");
    }

    private static BookstoreDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<BookstoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new BookstoreDbContext(options);
    }
}

