using AutoMapper;
using BookLendingBackUp.Application.DTOs;
using BookLendingBackUp.Application.Interfaces;
using BookLendingBackUp.Domain.Entities;
using BookLendingBackUp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingBackUp.Infrastructure.Services
{
    public class BookService : IBook
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public BookService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;

            _mapper = mapper;

        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            var Books = await _context.Books.AsNoTracking().ToListAsync();
            return Books;
        }


        public async Task<BookdetailsDTO?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new Exception("Invalid book ID");

            var book = await _context.Books
                .Where(b => b.Id == id)
                .Include(c => c.catalog)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (book == null)
                return null;

            var bookDetailsDto = new BookdetailsDTO
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Author = book.Author,
                PublishedAt = book.PublishedAt,
                IsAvailable = book.IsAvailable,
                CatalogName = book.catalog?.Name // add null-check if catalog is optional
            };

            return bookDetailsDto;
        }

        public async Task<IEnumerable<Book>> GetByCatalogIdAsync(int id)
        {
            if (id <= 0)
                throw new Exception("catalog not found");
            var books = await _context.Books.Where(b => b.CatalogId == id).ToListAsync();
            return books; 

        }
        public async Task<Book> CreateAsync(Book book)
        {

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book> UpdateAsync(int id, BookDTO bookdto)
        {
            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
                throw new Exception("Book not found");

            _mapper.Map(bookdto, existingBook);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception("Failed to save changes", ex);
            }
            return existingBook;

        }

        public async Task DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }

        }
    }
}
