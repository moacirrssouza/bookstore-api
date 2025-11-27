using Bookstore.Api.Common;
using Bookstore.Appication.DTOs;
using Bookstore.Domain.Entities;
using Bookstore.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Api.Features.Genres.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class GenresController : ControllerBase
{
    private readonly IGenreRepository _repo;

    public GenresController(IGenreRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GenreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var genre = await _repo.GetByIdAsync(id, ct);
        if (genre == null)
            return NotFound(new ApiResponse("Genre not found"));

        return Ok(new ApiResponse<GenreDto>(new GenreDto(genre.Id, genre.Name)));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<GenreDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct)
    {
        var genres = await _repo.GetAllAsync(ct);
        var dtos = genres.Select(g => new GenreDto(g.Id, g.Name));
        return Ok(new ApiResponse<IEnumerable<GenreDto>>(dtos));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<GenreDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(CreateGenreDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse("Invalid request payload."));

        var genre = new Genre(dto.Name);
        await _repo.CreateAsync(genre, ct);

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = genre.Id, version = "1.0" },
            new ApiResponse<GenreDto>(new GenreDto(genre.Id, genre.Name))
        );
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GenreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAsync(Guid id, CreateGenreDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse("Invalid request payload."));

        var genre = await _repo.GetByIdAsync(id, ct);
        if (genre == null)
            return NotFound(new ApiResponse("Genre not found"));

        genre.UpdateName(dto.Name);
        await _repo.UpdateAsync(genre, ct);
        return Ok(new ApiResponse<GenreDto>(new GenreDto(genre.Id, genre.Name)));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var genre = await _repo.GetByIdAsync(id, ct);
        if (genre == null)
            return NotFound(new ApiResponse("Genre not found"));

        if (genre.Books != null && genre.Books.Any())
            return BadRequest(new ApiResponse("Cannot delete genre with books."));

        await _repo.DeleteAsync(id, ct);
        return NoContent();
    }
}