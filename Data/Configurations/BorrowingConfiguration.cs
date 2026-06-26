using CourseLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseLibrary.Data.Configurations;

public class BorrowingConfiguration : IEntityTypeConfiguration<Borrowing>
{
    public void Configure(EntityTypeBuilder<Borrowing> builder)
    {
        builder.HasKey(borrowing => borrowing.Id);

        builder.Property(borrowing => borrowing.BorrowerName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(borrowing => borrowing.BorrowedAt)
            .IsRequired();

        builder.Property(borrowing => borrowing.ReturnedAt)
            .IsRequired(false);

        builder.HasOne(borrowing => borrowing.Book)
            .WithMany(book => book.Borrowings)
            .HasForeignKey(borrowing => borrowing.BookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}