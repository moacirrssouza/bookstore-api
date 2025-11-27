using Bookstore.Domain.Entities;

public class Book
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }

    public Guid AuthorId { get; private set; }
    public Author Author { get; private set; } = null!;

    public Guid GenreId { get; private set; }
    public Genre Genre { get; private set; } = null!;

    public string? Description { get; private set; }


    public Book(string name, Guid authorId, Guid genreId, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome is required", nameof(name));
        Name = name;
        AuthorId = authorId;
        GenreId = genreId;
        Description = description;
    }

    protected Book() { }

    public void Update(string name, Guid authorId, Guid genreId, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome is required", nameof(name));
        Name = name;
        AuthorId = authorId;
        GenreId = genreId;
        Description = description;
    }
}