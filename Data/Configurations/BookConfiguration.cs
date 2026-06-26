using CourseLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseLibrary.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(book => book.Id);

        builder.Property(book => book.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(book => book.Isbn)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(book => book.PublishedYear)
            .IsRequired();

        builder.HasIndex(book => book.Isbn)
            .IsUnique();

        builder.HasOne(book => book.Author)
            .WithMany(author => author.Books)
            .HasForeignKey(book => book.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(book => book.Borrowings)
            .WithOne(borrowing => borrowing.Book)
            .HasForeignKey(borrowing => borrowing.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
           new Book { Id = 1, Title = "1984", Isbn = "9780451524935", PublishedYear = 1949, AuthorId = 1 },
           new Book { Id = 2, Title = "Animal Farm", Isbn = "9780451526342", PublishedYear = 1945, AuthorId = 1 },
           new Book { Id = 3, Title = "Clean Code", Isbn = "9780132350884", PublishedYear = 2008, AuthorId = 2 },
           new Book { Id = 4, Title = "Agile Software Development", Isbn = "9780135974445", PublishedYear = 2002, AuthorId = 2 },
           new Book { Id = 5, Title = "Refactoring", Isbn = "9780134757599", PublishedYear = 2018, AuthorId = 3 }
       );
    }
}