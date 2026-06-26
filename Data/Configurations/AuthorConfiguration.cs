using CourseLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseLibrary.Data.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(author => author.Id);

        builder.Property(author => author.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(author => author.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(author => author.Books)
            .WithOne(book => book.Author)
            .HasForeignKey(book => book.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Author { Id = 1, FirstName = "George", LastName = "Orwell" },
            new Author { Id = 2, FirstName = "Robert", LastName = "Martin" },
            new Author { Id = 3, FirstName = "Martin", LastName = "Fowler" }
        );
    }
}