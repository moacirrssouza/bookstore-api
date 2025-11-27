namespace Bookstore.Domain.Entities;

public class Genre
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public ICollection<Book> Books { get; private set; } = new List<Book>();

    public Genre(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));
        Name = name;
    }

    protected Genre() { }
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));
        Name = name;
    }
}