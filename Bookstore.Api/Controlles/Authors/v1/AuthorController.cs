using Bookstore.Api.Common;
using Bookstore.Appication.DTOs;
using Bookstore.Domain.Repositories;
using Bookstore.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Api.Controllers.Authors.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorRepository _repo;

    public AuthorsController(IAuthorRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAllAsync(Guid id, CancellationToken ct)
    {
        var author = await _repo.GetByIdAsync(id, ct);
        if (author == null)
            return NotFound(new ApiResponse("Author not found"));

        return Ok(new ApiResponse<AuthorDto>(new AuthorDto(author.Id, author.Name)));
    }

    [HttpGet]
    public async Task<IActionResult> GetByIdAsync(CancellationToken ct)
    {
        var authors = await _repo.GetAllAsync(ct);
        var dtos = authors.Select(a => new AuthorDto(a.Id, a.Name));
        return Ok(new ApiResponse<IEnumerable<AuthorDto>>(dtos));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateAuthorDto dto, CancellationToken ct)
    {
        var author = new Author(dto.Name);
        await _repo.CreateAsync(author, ct);

        return CreatedAtAction(
            nameof(GetAllAsync),
            new { id = author.Id, version = "1.0" },
            new ApiResponse<AuthorDto>(new AuthorDto(author.Id, author.Name))
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, CreateAuthorDto dto, CancellationToken ct)
    {
        var author = await _repo.GetByIdAsync(id, ct);
        if (author == null)
            return NotFound(new ApiResponse("Author not found"));

        author.UpdateName(dto.Name);
        await _repo.UpdateAsync(author, ct);
        return Ok(new ApiResponse<AuthorDto>(new AuthorDto(author.Id, author.Name)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var author = await _repo.GetByIdAsync(id, ct);
        if (author == null)
            return NotFound(new ApiResponse("Author not found"));

        await _repo.DeleteAsync(id, ct);
        return NoContent();
    }
}