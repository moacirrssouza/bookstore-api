namespace Bookstore.Domain.Repositories;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Book?> GetByIdAsync(Guid BookId, CancellationToken cancellationToken = default);

    Task CreateAsync(Book Book, CancellationToken cancellationToken = default);

    Task UpdateAsync(Book Book, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid BookId, CancellationToken cancellationToken = default);
}