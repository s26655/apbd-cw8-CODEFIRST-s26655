using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseLibrary.ViewModels;

public class BookCreateViewModel
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Isbn { get; set; } = string.Empty;

    [Range(1901, int.MaxValue, ErrorMessage = "Published year must be greater than 1900.")]
    public int PublishedYear { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please choose an author.")]
    public int AuthorId { get; set; }

    public List<SelectListItem> Authors { get; set; } = new();
}