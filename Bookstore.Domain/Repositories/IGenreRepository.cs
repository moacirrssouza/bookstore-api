using Bookstore.Domain.Entities;

namespace Bookstore.Domain.Repositories;

public interface IGenreRepository
{
    Task<IEnumerable<Genre>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Genre?> GetByIdAsync(Guid genceId, CancellationToken cancellationToken = default);

    Task CreateAsync(Genre genre, CancellationToken cancellationToken = default);

    Task UpdateAsync(Genre genre, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid genreId, CancellationToken cancellationToken = default);
}
