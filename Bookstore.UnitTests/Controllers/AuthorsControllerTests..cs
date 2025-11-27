using Bookstore.Api.Common;
using Bookstore.Api.Controllers.Authors.v1;
using Bookstore.Appication.DTOs;
using Bookstore.Domain.Entities;
using Bookstore.Domain.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bookstore.UnitTests.Controllers;

public class AuthorsControllerTests
{
    private readonly Mock<IAuthorRepository> _repoMock = new();
    private readonly AuthorsController _controller;

    public AuthorsControllerTests()
    {
        _controller = new AuthorsController(_repoMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsOk_WhenAuthorExists()
    {
        var author = new Author("Test");
        _repoMock
            .Setup(r => r.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _controller.GetByIdAsync(author.Id, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<ApiResponse<AuthorDto>>();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNotFound_WhenAuthorMissing()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        var result = await _controller.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFound.Value.Should().BeEquivalentTo(new ApiResponse("Author not found"));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOk_WithAuthorsList()
    {
        var authors = new List<Author> { new("Moacir"), new("João") };
        _repoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(authors);

        var result = await _controller.GetAllAsync(CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<ApiResponse<IEnumerable<AuthorDto>>>();
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreated_WhenValid()
    {
        var dto = new CreateAuthorDto("Novo Autor");
        _repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await _controller.CreateAsync(dto, CancellationToken.None);

        response.Should().BeOfType<CreatedAtActionResult>();
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsBadRequest_WhenModelInvalid()
    {
        var dto = new CreateAuthorDto("");

        _controller.ModelState.AddModelError("Name", "Required");

        var response = await _controller.CreateAsync(dto, CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNoContent_WhenAuthorExists()
    {
        var author = new Author("Deletable");
        _repoMock
            .Setup(r => r.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        _repoMock
            .Setup(r => r.DeleteAsync(author.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await _controller.DeleteAsync(author.Id, CancellationToken.None);

        response.Should().BeOfType<NoContentResult>();
        _repoMock.Verify(r => r.DeleteAsync(author.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenAuthorMissing()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        var response = await _controller.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        var notFound = response.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFound.Value.Should().BeEquivalentTo(new ApiResponse("Author not found"));
    }

    [Fact]
    public async Task UpdateAsync_ReturnsBadRequest_WhenModelInvalid()
    {
        var dto = new CreateAuthorDto("");
        _controller.ModelState.AddModelError("Name", "Required");

        var result = await _controller.UpdateAsync(Guid.NewGuid(), dto, CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsOk_WhenAuthorExists()
    {
        var author = new Author("Existing");
        _repoMock
            .Setup(r => r.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _repoMock
            .Setup(r => r.UpdateAsync(author, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await _controller.UpdateAsync(author.Id, new CreateAuthorDto("Updated"), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<ApiResponse<AuthorDto>>();
    }
}