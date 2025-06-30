using AutoMapper;
using BookLendingBackUp.Application.DTOs;
using BookLendingBackUp.Application.Interfaces;
using BookLendingBackUp.Domain.Entities;
using BookLendingBackUp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingBackUp.Infrastructure.Services
{

    public class CatalogService : ICatalog
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public CatalogService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;

            _mapper = mapper;

        }





        public async Task<IEnumerable<Catalog>> GetAllAsync()
        {
            var Catalogs = await _context.Catalogs.AsNoTracking().ToListAsync();
            return Catalogs;
        }

        public async Task<IEnumerable<Book>> GetAllBooksInCatalogAsync(int id)
        {
            if (id <= 0)
                return Enumerable.Empty<Book>();

            var catalog = await _context.Catalogs
                .Include(c => c.Books).AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return catalog?.Books ?? Enumerable.Empty<Book>();

        }

        public async Task<Catalog?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new Exception("Catalog not found"); ;

            var catalog = await _context.Catalogs
            .Include(c => c.Books).AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
            return catalog;

        }


        public async Task<Catalog> CreateAsync(Catalog catalog)
        {
            _context.Catalogs.Add(catalog);
            await _context.SaveChangesAsync();
            return catalog;
        }

        public async Task<Catalog> UpdateAsync(int id, CatalogDTO catalog)
        {
            var existingcatalog = await _context.Catalogs.FindAsync(id);

            if (existingcatalog == null) throw new Exception("Catalog not found");

       

            _mapper.Map(catalog, existingcatalog);

            _context.Entry(existingcatalog).Property(c => c.Name).IsModified = true;
            _context.Entry(existingcatalog).Property(c => c.Description).IsModified = true;

            await _context.SaveChangesAsync();

            return existingcatalog;
          

        }

        public async Task DeleteAsync(int id)
        {
            var catalog = await _context.Catalogs.FindAsync(id);
            if (catalog != null)
            {
                _context.Catalogs.Remove(catalog);
                await _context.SaveChangesAsync();
            }

        }
    }
}
