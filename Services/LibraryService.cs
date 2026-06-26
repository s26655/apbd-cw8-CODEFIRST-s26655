using CourseLibrary.Data;
using CourseLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.Services;

public class LibraryService : ILibraryService
{
    private readonly LibraryDbContext _context;

    public LibraryService(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<List<Book>> GetBooksAsync()
    {
        return await _context.Books
            .Include(book => book.Author)
            .OrderBy(book => book.Title)
            .ToListAsync();
    }

    public async Task<Book?> GetBookDetailsAsync(int id)
    {
        return await _context.Books
            .Include(book => book.Author)
            .Include(book => book.Borrowings.OrderByDescending(borrowing => borrowing.BorrowedAt))
            .FirstOrDefaultAsync(book => book.Id == id);
    }

    public async Task<List<Author>> GetAuthorsAsync()
    {
        return await _context.Authors
            .OrderBy(author => author.LastName)
            .ThenBy(author => author.FirstName)
            .ToListAsync();
    }

    public async Task<List<Book>> GetBooksForSelectionAsync()
    {
        return await _context.Books
            .Include(book => book.Author)
            .OrderBy(book => book.Title)
            .ToListAsync();
    }

    public async Task AddBookAsync(Book book)
    {
        var authorExists = await _context.Authors
            .AnyAsync(author => author.Id == book.AuthorId);

        if (!authorExists)
        {
            throw new InvalidOperationException("Selected author does not exist.");
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AddBorrowingAsync(Borrowing borrowing)
    {
        var bookExists = await _context.Books
            .AnyAsync(book => book.Id == borrowing.BookId);

        if (!bookExists)
        {
            return false;
        }

        _context.Borrowings.Add(borrowing);
        await _context.SaveChangesAsync();

        return true;
    }
}