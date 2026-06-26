namespace CourseLibrary.Models;

public class Borrowing
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public Book Book { get; set; } = null!;

    public string BorrowerName { get; set; } = string.Empty;

    public DateTime BorrowedAt { get; set; }

    public DateTime? ReturnedAt { get; set; }
}