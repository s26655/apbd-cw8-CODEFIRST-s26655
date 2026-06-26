using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseLibrary.ViewModels;

public class BorrowingCreateViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Please choose a book.")]
    public int BookId { get; set; }

    [Required]
    [MaxLength(150)]
    public string BorrowerName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime BorrowedAt { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    public DateTime? ReturnedAt { get; set; }

    public List<SelectListItem> Books { get; set; } = new();
}