using Bookstore.Api.Common;
using Bookstore.Api.Features.Genres.v1;
using Bookstore.Appication.DTOs;
using Bookstore.Domain.Entities;
using Bookstore.Domain.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bookstore.UnitTests.Controllers;

public class GenresControllerTests
{
    private readonly Mock<IGenreRepository> _repoMock = new();
    private readonly GenresController _controller;

    public GenresControllerTests()
    {
        _controller = new GenresController(_repoMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsOk_WhenGenreExists()
    {
        var genre = new Genre("Fantasy");
        _repoMock
            .Setup(r => r.GetByIdAsync(genre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(genre);

        var result = await _controller.GetByIdAsync(genre.Id, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<ApiResponse<GenreDto>>();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNotFound_WhenMissing()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Genre?)null);

        var result = await _controller.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new ApiResponse("Genre not found"));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOk_WithGenres()
    {
        var genres = new List<Genre> { new("Romance"), new("Horror") };
        _repoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(genres);

        var result = await _controller.GetAllAsync(CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<ApiResponse<IEnumerable<GenreDto>>>();
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreated_WhenValidRequest()
    {
        var dto = new CreateGenreDto("New Genre");
        _repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Genre>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await _controller.CreateAsync(dto, CancellationToken.None);

        response.Should().BeOfType<CreatedAtActionResult>();
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Genre>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsBadRequest_WhenModelInvalid()
    {
        var dto = new CreateGenreDto(string.Empty);
        _controller.ModelState.AddModelError("Name", "Required");

        var response = await _controller.CreateAsync(dto, CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Genre>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenGenreMissing()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Genre?)null);

        var response = await _controller.UpdateAsync(Guid.NewGuid(), new CreateGenreDto("Name"), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new ApiResponse("Genre not found"));
    }

    [Fact]
    public async Task UpdateAsync_ReturnsOk_WhenGenreExists()
    {
        var genre = new Genre("Old Name");
        _repoMock
            .Setup(r => r.GetByIdAsync(genre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(genre);

        _repoMock
            .Setup(r => r.UpdateAsync(genre, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await _controller.UpdateAsync(genre.Id, new CreateGenreDto("Updated"), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<ApiResponse<GenreDto>>();
    }

    [Fact]
    public async Task UpdateAsync_ReturnsBadRequest_WhenModelInvalid()
    {
        var dto = new CreateGenreDto(string.Empty);
        _controller.ModelState.AddModelError("Name", "Required");

        var response = await _controller.UpdateAsync(Guid.NewGuid(), dto, CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Genre>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsBadRequest_WhenGenreHasBooks()
    {
        var genre = new Genre("Used");
        genre.Books.Add(new Book("Linked", Guid.NewGuid(), Guid.NewGuid()));
        _repoMock
            .Setup(r => r.GetByIdAsync(genre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(genre);

        var response = await _controller.DeleteAsync(genre.Id, CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new ApiResponse("Cannot delete genre with books."));
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenMissing()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Genre?)null);

        var response = await _controller.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        response.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new ApiResponse("Genre not found"));
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNoContent_WhenGenreExists()
    {
        var genre = new Genre("Disposable");
        _repoMock
            .Setup(r => r.GetByIdAsync(genre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(genre);

        _repoMock
            .Setup(r => r.DeleteAsync(genre.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await _controller.DeleteAsync(genre.Id, CancellationToken.None);

        response.Should().BeOfType<NoContentResult>();
        _repoMock.Verify(r => r.DeleteAsync(genre.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}

