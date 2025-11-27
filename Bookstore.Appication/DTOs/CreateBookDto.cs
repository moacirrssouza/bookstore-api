namespace Bookstore.Appication.DTOs;

public record CreateBookDto(string Name, Guid AuthorId, Guid GenreId, string Description);