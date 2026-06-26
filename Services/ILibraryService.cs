using CourseLibrary.Models;

namespace CourseLibrary.Services;

public interface ILibraryService
{
    Task<List<Book>> GetBooksAsync();

    Task<Book?> GetBookDetailsAsync(int id);

    Task<List<Author>> GetAuthorsAsync();

    Task<List<Book>> GetBooksForSelectionAsync();

    Task AddBookAsync(Book book);

    Task<bool> AddBorrowingAsync(Borrowing borrowing);
}