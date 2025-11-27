using Bookstore.Domain.Entities;

namespace Bookstore.Domain.Repositories;

public interface IAuthorRepository
{
    Task<IEnumerable<Author>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Author?> GetByIdAsync(Guid authorId, CancellationToken cancellationToken = default);

    Task CreateAsync(Author author, CancellationToken cancellationToken = default);

    Task UpdateAsync(Author author, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid authorId, CancellationToken cancellationToken = default);
}
