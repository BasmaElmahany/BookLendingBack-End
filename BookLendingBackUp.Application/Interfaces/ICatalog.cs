using BookLendingBackUp.Application.DTOs;
using BookLendingBackUp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingBackUp.Application.Interfaces
{
    public interface ICatalog
    {
        Task<IEnumerable<Catalog>> GetAllAsync();

        Task<IEnumerable<Book>> GetAllBooksInCatalogAsync(int id);
        Task<Catalog?> GetByIdAsync(int id);
        Task<Catalog> CreateAsync(Catalog catalog);
        Task<Catalog> UpdateAsync(int id, CatalogDTO catalog);
        Task DeleteAsync(int id);
    }
}
