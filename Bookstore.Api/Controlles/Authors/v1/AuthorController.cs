using Bookstore.Api.Common;
using Bookstore.Appication.DTOs;
using Bookstore.Domain.Entities;
using Bookstore.Domain.Repositories;
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
    [ProducesResponseType(typeof(ApiResponse<AuthorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var author = await _repo.GetByIdAsync(id, ct);
        if (author == null)
            return NotFound(new ApiResponse("Author not found"));

        return Ok(new ApiResponse<AuthorDto>(new AuthorDto(author.Id, author.Name)));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AuthorDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct)
    {
        var authors = await _repo.GetAllAsync(ct);
        var dtos = authors.Select(a => new AuthorDto(a.Id, a.Name));
        return Ok(new ApiResponse<IEnumerable<AuthorDto>>(dtos));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AuthorDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(CreateAuthorDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse("Invalid request payload."));

        var author = new Author(dto.Name);
        await _repo.CreateAsync(author, ct);

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = author.Id, version = "1.0" },
            new ApiResponse<AuthorDto>(new AuthorDto(author.Id, author.Name))
        );
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AuthorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAsync(Guid id, CreateAuthorDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse("Invalid request payload."));

        var author = await _repo.GetByIdAsync(id, ct);
        if (author == null)
            return NotFound(new ApiResponse("Author not found"));

        author.UpdateName(dto.Name);
        await _repo.UpdateAsync(author, ct);
        return Ok(new ApiResponse<AuthorDto>(new AuthorDto(author.Id, author.Name)));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var author = await _repo.GetByIdAsync(id, ct);
        if (author == null)
            return NotFound(new ApiResponse("Author not found"));

        await _repo.DeleteAsync(id, ct);
        return NoContent();
    }
}