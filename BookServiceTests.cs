using AutoMapper;
using BookLendingBackUp.Application.DTOs;
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
    public class BookServiceTests
    {
        private readonly BookService _service;
        private readonly ApplicationDbContext _context;

        public BookServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "BookLendingTest")
                .Options;

            _context = new ApplicationDbContext(options);

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<BookDTO, Book>();
                cfg.CreateMap<Book, BookDTO>();
            });

            var mapper = config.CreateMapper();

            _service = new BookService(_context, mapper);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddBook()
        {
            var book = new Book { Title = "Test", Author = "Tester", Description = "Unit Test", PublishedAt = DateOnly.ParseExact("24-06-2025", "dd-MM-yyyy"), IsAvailable = true, CatalogId = 1 };

            var result = await _service.CreateAsync(book);

            Assert.NotNull(result);
            Assert.Equal("Test", result.Title);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnBooks()
        {
            _context.Books.Add(new Book { Title = "One", Author = "Author", Description = "Desc", IsAvailable = true });
            _context.SaveChanges();

            var result = await _service.GetAllAsync();

            Assert.NotEmpty(result);
        }
    }
}
