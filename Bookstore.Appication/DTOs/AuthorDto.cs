namespace Bookstore.Appication.DTOs;

/// <summary>
/// Represents a data transfer object containing information about an author.
/// </summary>
/// <param name="Id">The unique identifier of the author.</param>
/// <param name="Name">The full name of the author.</param>
public record AuthorDto(Guid Id, string Name);