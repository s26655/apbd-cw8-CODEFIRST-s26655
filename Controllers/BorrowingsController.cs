using CourseLibrary.Models;
using CourseLibrary.Services;
using CourseLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseLibrary.Controllers;

public class BorrowingsController : Controller
{
    private readonly ILibraryService _libraryService;

    public BorrowingsController(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    public async Task<IActionResult> Create(int? bookId)
    {
        var viewModel = new BorrowingCreateViewModel
        {
            BookId = bookId ?? 0
        };

        await PopulateBooksAsync(viewModel);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BorrowingCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            await PopulateBooksAsync(viewModel);

            return View(viewModel);
        }

        var borrowing = new Borrowing
        {
            BookId = viewModel.BookId,
            BorrowerName = viewModel.BorrowerName,
            BorrowedAt = viewModel.BorrowedAt,
            ReturnedAt = viewModel.ReturnedAt
        };

        var borrowingAdded = await _libraryService.AddBorrowingAsync(borrowing);

        if (!borrowingAdded)
        {
            ModelState.AddModelError(nameof(viewModel.BookId), "Selected book does not exist.");

            await PopulateBooksAsync(viewModel);

            return View(viewModel);
        }

        return RedirectToAction("Details", "Books", new { id = viewModel.BookId });
    }

    private async Task PopulateBooksAsync(BorrowingCreateViewModel viewModel)
    {
        var books = await _libraryService.GetBooksForSelectionAsync();

        viewModel.Books = books
            .Select(book => new SelectListItem
            {
                Value = book.Id.ToString(),
                Text = $"{book.Title} - {book.Author.FirstName} {book.Author.LastName}"
            })
            .ToList();
    }
}