using BookLendingBackUp.Application.DTOs;
using BookLendingBackUp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingBackUp.Application.Interfaces
{
    public interface IBook
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<BookdetailsDTO?> GetByIdAsync(int id);
        Task<IEnumerable<Book>>? GetByCatalogIdAsync(int id);
        Task<Book> CreateAsync(Book book);
        Task<Book> UpdateAsync(int id, BookDTO bookdto);
        Task DeleteAsync(int id);
    }
}
