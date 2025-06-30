using BookLendingBackUp.Application.DTOs;
using BookLendingBackUp.Domain.Entities;
using BookLendingBackUp.Infrastructure.Entities;
using BookLendingBackUp.Infrastructure.Interfaces;
using BookLendingBackUp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingBackUp.Infrastructure.Services
{
    public class BorrowBookService : IBorrowBook
    {
        private readonly ApplicationDbContext _context;

        public BorrowBookService(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<UsersBooksBorrow> BorrowBookAsync(string userId, int bookId)
        {

            var book = await _context.Books.FindAsync(bookId);
            if (book == null || !book.IsAvailable)
                throw new Exception("Book is not available");


            bool alreadyBorrowed = await _context.UsersBooksBorrows
                .AnyAsync(b => b.ApplicationUserID == userId && !b.BookReturned);

            if (alreadyBorrowed)
                throw new Exception("User already borrowed a book");


            var borrow = new UsersBooksBorrow
            {
                ApplicationUserID = userId,
                BookId = bookId,
                BorrowedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7),
                BookReturned = false
            };

            book.IsAvailable = false;

            _context.UsersBooksBorrows.Add(borrow);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save changes: " + ex.InnerException?.Message, ex);
            }

            return borrow;
        }

        public async Task<bool> ReturnBookAsync(int borrowId)
        {
            var borrow = await _context.UsersBooksBorrows
                  .Include(b => b.Book)
                  .FirstOrDefaultAsync(b => b.Id == borrowId);

            if (borrow == null || borrow.BookReturned)
                return false;

            borrow.BookReturned = true;
            borrow.Book.IsAvailable = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DelayedBookDto>> DelayedBooks()
        {
            var delayedBooks = await _context.UsersBooksBorrows
                .Where(u => u.DueDate < DateTime.Now)
                .Join(_context.Books,
                      u => u.BookId,
                      b => b.Id,
                      (u, b) => new DelayedBookDto
                      {
                          Id = b.Id,
                          Title = b.Title,
                          Author = b.Author,
                          Description = b.Description,
                          PublishedAt = b.PublishedAt,
                          CatalogName = b.catalog.Name,
                          FullName = u.ApplicationUser.FullName,
                          BorrowedAt = u.BorrowedAt,
                          DueDate = u.DueDate
                      })
                .ToListAsync();

            return delayedBooks;
        }


    }
}
