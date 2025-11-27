using Bookstore.Domain.Entities;
using Bookstore.Domain.Repositories;
using Bookstore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly BookstoreDbContext _context;
    private readonly DbSet<Author> _dbSet;

    public AuthorRepository(BookstoreDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Author>();
    }

    public async Task<IEnumerable<Author>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<Author?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task CreateAsync(Author author, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(author, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Author author, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(author);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(authorId, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}