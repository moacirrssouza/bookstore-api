namespace Bookstore.Appication.DTOs;

public record BookDto(Guid Id, string Name, string Author, string Genre, string Description);