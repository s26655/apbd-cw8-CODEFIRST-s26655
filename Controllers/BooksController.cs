using CourseLibrary.Models;
using CourseLibrary.Services;
using CourseLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseLibrary.Controllers;

public class BooksController : Controller
{
    private readonly ILibraryService _libraryService;

    public BooksController(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _libraryService.GetBooksAsync();

        return View(books);
    }

    public async Task<IActionResult> Details(int id)
    {
        var book = await _libraryService.GetBookDetailsAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        return View(book);
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new BookCreateViewModel();

        await PopulateAuthorsAsync(viewModel);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            await PopulateAuthorsAsync(viewModel);

            return View(viewModel);
        }

        var book = new Book
        {
            Title = viewModel.Title,
            Isbn = viewModel.Isbn,
            PublishedYear = viewModel.PublishedYear,
            AuthorId = viewModel.AuthorId
        };

        try
        {
            await _libraryService.AddBookAsync(book);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(nameof(viewModel.AuthorId), exception.Message);

            await PopulateAuthorsAsync(viewModel);

            return View(viewModel);
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateAuthorsAsync(BookCreateViewModel viewModel)
    {
        var authors = await _libraryService.GetAuthorsAsync();

        viewModel.Authors = authors
            .Select(author => new SelectListItem
            {
                Value = author.Id.ToString(),
                Text = $"{author.FirstName} {author.LastName}"
            })
            .ToList();
    }
}