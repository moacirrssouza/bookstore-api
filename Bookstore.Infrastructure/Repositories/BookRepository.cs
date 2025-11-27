using Bookstore.Domain.Repositories;
using Bookstore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Infrastructure.Repositories;
public class BookRepository : IBookRepository
{
    private readonly BookstoreDbContext _context;
    private readonly DbSet<Book> _dbSet;

    public BookRepository(BookstoreDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Book>();
    }

    public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .AsNoTracking();
        return await query.ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task CreateAsync(Book Book, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(Book, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Book Book, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(Book);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid BookId, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(BookId, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}