using Bookstore.Api.Common;
using Bookstore.Api.Features.Books.v1;
using Bookstore.Appication.DTOs;
using Bookstore.Domain.Entities;
using Bookstore.Domain.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bookstore.UnitTests.Controllers;

public class BooksControllerTests
{
    private readonly Mock<IBookRepository> _repoMock = new();
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _controller = new BooksController(_repoMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOk_WithBooks()
    {
        var books = new List<Book>
        {
            CreateBook("Clean Code", "Uncle Bob", "Software"),
            CreateBook("Refactoring", "Martin Fowler", "Software")
        };
        _repoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(books);

        var response = await _controller.GetAllAsync(CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<ApiResponse<IEnumerable<BookDto>>>();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsOk_WhenBookExists()
    {
        var book = CreateBook("DDD", "Eric Evans", "Architecture");
        _repoMock
            .Setup(r => r.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var response = await _controller.GetByIdAsync(book.Id, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<ApiResponse<BookDto>>();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNotFound_WhenMissing()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var response = await _controller.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new ApiResponse("Book not found"));
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenBookMissing()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var result = await _controller.UpdateAsync(Guid.NewGuid(), new CreateBookDto("Any", Guid.NewGuid(), Guid.NewGuid(), "desc"), CancellationToken.None);

        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new ApiResponse("Book not found"));
    }

    [Fact]
    public async Task UpdateAsync_ReturnsOk_WhenBookExists()
    {
        var book = CreateBook("Original", "Author", "Genre");
        _repoMock
            .Setup(r => r.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);
        _repoMock
            .Setup(r => r.UpdateAsync(book, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await _controller.UpdateAsync(book.Id, new CreateBookDto("New", Guid.NewGuid(), Guid.NewGuid(), "desc"), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<ApiResponse<BookDto>>();
    }

    [Fact]
    public async Task CreateAsync_ReturnsBadRequest_WhenModelInvalid()
    {
        var dto = new CreateBookDto(string.Empty, Guid.NewGuid(), Guid.NewGuid(), "desc");
        _controller.ModelState.AddModelError("Name", "Required");

        var response = await _controller.CreateAsync(dto, CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsBadRequest_WhenModelInvalid()
    {
        var dto = new CreateBookDto(string.Empty, Guid.NewGuid(), Guid.NewGuid(), "desc");
        _controller.ModelState.AddModelError("Name", "Required");

        var response = await _controller.UpdateAsync(Guid.NewGuid(), dto, CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNoContent_WhenBookExists()
    {
        var book = CreateBook("Clean Architecture", "Robert Martin", "Architecture");
        _repoMock
            .Setup(r => r.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _repoMock
            .Setup(r => r.DeleteAsync(book.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await _controller.DeleteAsync(book.Id, CancellationToken.None);

        response.Should().BeOfType<NoContentResult>();
        _repoMock.Verify(r => r.DeleteAsync(book.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenMissing()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var response = await _controller.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new ApiResponse("Book not found"));
    }

    private static Book CreateBook(string name, string authorName, string genreName)
    {
        var book = new Book(name, Guid.NewGuid(), Guid.NewGuid(), "Desc");

        var authorProp = typeof(Book).GetProperty(nameof(Book.Author));
        authorProp!.SetValue(book, new Author(authorName));

        var genreProp = typeof(Book).GetProperty(nameof(Book.Genre));
        genreProp!.SetValue(book, new Genre(genreName));

        return book;
    }
}

