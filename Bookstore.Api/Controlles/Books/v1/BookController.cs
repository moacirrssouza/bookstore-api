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
    public async Task<IActionResult> GetAllAsync(CancellationToken ct)
    {
        var books = await _bookRepository.GetAllAsync(ct);

        var dtos = books.Select(b => new BookDto(
            b.Id,
            b.Name,
            b.Author?.Name ?? string.Empty,
            b.Genre?.Name ?? string.Empty, 
            b.Description
        ));

        return Ok(new ApiResponse<IEnumerable<BookDto>>(dtos));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var book = await _bookRepository.GetByIdAsync(id, ct);

        if (book == null)
            return NotFound(new ApiResponse("Book not found"));

        var dto = new BookDto(
            book.Id,
            book.Name,
            book.Author?.Name ?? string.Empty, 
            book.Genre?.Name ?? string.Empty, 
            book.Description
        );

        return Ok(new ApiResponse<BookDto>(dto));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateBookDto dto, CancellationToken ct)
    {
        var book = new Book(dto.Name, dto.AuthorId, dto.GenreId, dto.Description);
        await _bookRepository.CreateAsync(book, ct);

        var response = new BookDto(
            book.Id,
            book.Name,
            book.Author.Name,
            book.Genre.Name,
            book.Description
        );

        return CreatedAtAction(
            nameof(GetAllAsync),
            new { id = book.Id, version = "1.0" },
            new ApiResponse<BookDto>(response)
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, CreateBookDto dto, CancellationToken ct)
    {
        var book = await _bookRepository.GetByIdAsync(id, ct);
        if (book == null)
            return NotFound(new ApiResponse("Book not found"));

        book.Update(dto.Name, dto.AuthorId, dto.GenreId, dto.Description);
        await _bookRepository.UpdateAsync(book, ct);
        var response = new BookDto(
            book.Id,
            book.Name,
            book.Author.Name,
            book.Genre.Name,
            book.Description
        );

        return Ok(new ApiResponse<BookDto>(response));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var book = await _bookRepository.GetByIdAsync(id, ct);
        if (book == null)
            return NotFound(new ApiResponse("Book not found"));

        await _bookRepository.DeleteAsync(id, ct);
        return NoContent();
    }
}