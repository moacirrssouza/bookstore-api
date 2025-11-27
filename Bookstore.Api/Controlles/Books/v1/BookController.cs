using Bookstore.Api.Common;
using Bookstore.Appication.DTOs;
using Bookstore.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Api.Features.Books.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _bookRepository;

    public BooksController(IBookRepository repo)
    {
        _bookRepository = repo;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BookDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct)
    {
        var books = await _bookRepository.GetAllAsync(ct);

        var dtos = books.Select(MapToDto);

        return Ok(new ApiResponse<IEnumerable<BookDto>>(dtos));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BookDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var book = await _bookRepository.GetByIdAsync(id, ct);

        if (book == null)
            return NotFound(new ApiResponse("Book not found"));

        return Ok(new ApiResponse<BookDto>(MapToDto(book)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BookDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(CreateBookDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse("Invalid request payload."));

        var book = new Book(dto.Name, dto.AuthorId, dto.GenreId, dto.Description);
        await _bookRepository.CreateAsync(book, ct);

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = book.Id, version = "1.0" },
            new ApiResponse<BookDto>(MapToDto(book))
        );
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BookDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAsync(Guid id, CreateBookDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse("Invalid request payload."));

        var book = await _bookRepository.GetByIdAsync(id, ct);
        if (book == null)
            return NotFound(new ApiResponse("Book not found"));

        book.Update(dto.Name, dto.AuthorId, dto.GenreId, dto.Description);
        await _bookRepository.UpdateAsync(book, ct);

        return Ok(new ApiResponse<BookDto>(MapToDto(book)));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var book = await _bookRepository.GetByIdAsync(id, ct);
        if (book == null)
            return NotFound(new ApiResponse("Book not found"));

        await _bookRepository.DeleteAsync(id, ct);
        return NoContent();
    }

    private static BookDto MapToDto(Book book) =>
        new(
            book.Id,
            book.Name,
            book.Author?.Name ?? string.Empty,
            book.Genre?.Name ?? string.Empty,
            book.Description ?? string.Empty
        );
}