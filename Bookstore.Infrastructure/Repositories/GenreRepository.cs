using Bookstore.Domain.Entities;
using Bookstore.Domain.Repositories;
using Bookstore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Infrastructure.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly BookstoreDbContext _context;
    private readonly DbSet<Genre> _dbSet;

    public GenreRepository(BookstoreDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Genre>();
    }

    public async Task<IEnumerable<Genre>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<Genre?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task CreateAsync(Genre genre, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(genre, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Genre genre, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(genre);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid genreId, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(genreId, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}