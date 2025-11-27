namespace Bookstore.Domain.Entities;

public class Author
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public ICollection<Book> Books { get; private set; } = new List<Book>();

    public Author(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));
        Name = name;
    }

    protected Author() { } 
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));
        Name = name;
    }
}