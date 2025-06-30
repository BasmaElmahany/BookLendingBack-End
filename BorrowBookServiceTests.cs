using BookLendingBackUp.Domain.Entities;
using BookLendingBackUp.Infrastructure.Persistence;
using BookLendingBackUp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingBackUp.Tests
{
    public class BorrowBookServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly BorrowBookService _service;

        public BorrowBookServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("BorrowDb")
                .Options;

            _context = new ApplicationDbContext(options);

            _context.Books.Add(new Book { Id = 1, Title = "Clean Code", IsAvailable = true });
            _context.SaveChanges();

            _service = new BorrowBookService(_context);
        }

        [Fact]
        public async Task BorrowBookAsync_ShouldMarkBookAsUnavailable()
        {
            var result = await _service.BorrowBookAsync("user1", 1);

            Assert.False(result.BookReturned);
            Assert.Equal(1, result.BookId);
            Assert.False(_context.Books.First().IsAvailable);
        }
    }
}
