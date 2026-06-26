using CourseLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.Controllers;

public class AuthorsController : Controller
{
    private readonly ILibraryService _libraryService;

    public AuthorsController(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    public async Task<IActionResult> Index()
    {
        var authors = await _libraryService.GetAuthorsAsync();

        return View(authors);
    }
}