using AutoMapper;
using BookLendingBackUp.Application.DTOs;
using BookLendingBackUp.Application.Interfaces;
using BookLendingBackUp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace BookLendingBackUp.Controllers
{/// <summary>
/// Controller responsible for managing book catalogs.
/// Supports operations for listing, retrieving, creating, updating, and deleting catalogs.
/// Uses AutoMapper for DTO and entity conversion, and role-based authorization for access control.
/// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalog _catalog;

        private readonly IMapper _mapper;
        public CatalogController(ICatalog catalog, IMapper mapper)
        {
            _catalog = catalog;
            _mapper = mapper;
        }
        /// <summary>
        /// Retrieves a list of all catalogs.
        /// </summary>
        /// <returns>List of catalog records.</returns>
        [Authorize(Policy = "AdminOrMember")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll() =>
        Ok(await _catalog.GetAllAsync());



        /// <summary>
        /// Retrieves a specific catalog by its ID.
        /// </summary>
        /// <param name="id">The catalog ID.</param>
        /// <returns>The catalog details.</returns>
        [Authorize(Policy = "AdminOrMember")]
        [HttpGet("GetbyId/{id}")]
        public async Task<IActionResult> GetById(int id) =>
               Ok(await _catalog.GetByIdAsync(id));



        /// <summary>
        /// Retrieves all books under a specific catalog.
        /// </summary>
        /// <param name="id">The catalog ID.</param>
        /// <returns>List of books within the catalog.</returns>
        [HttpGet("{id}/books")]
        public async Task<IActionResult> GetBooksInCatalog(int id) =>
            Ok(await _catalog.GetAllBooksInCatalogAsync(id));



        /// <summary>
        /// Creates a new catalog entry.
        /// </summary>
        /// <param name="dto">The catalog data transfer object.</param>
        /// <returns>The created catalog and success message.</returns>
        [Authorize(Policy = "Admin")]
        [HttpPost("PostCatalog")]
        public async Task<IActionResult> Create(CatalogDTO dto)
        {
            var catalog = _mapper.Map<Catalog>(dto);
            var created = await _catalog.CreateAsync(catalog);
            var result = _mapper.Map<CatalogDTO>(created);
            return Ok(new { data = result, message = "Catalog Added Successfully" });
        }



        /// <summary>
        /// Updates an existing catalog by ID.
        /// </summary>
        /// <param name="id">The ID of the catalog to update.</param>
        /// <param name="catalog">The updated catalog DTO.</param>
        /// <returns>The updated catalog and success message.</returns>
        [Authorize(Policy = "Admin")]
        [HttpPut("Putcatalog/{id}")]
        public async Task<IActionResult> Update(int id, CatalogDTO catalog)
        {
            var updated = await _catalog.UpdateAsync(id, catalog);
            return Ok(new { data = updated, message = "Catalog Updated Successfully" });
        }



        /// <summary>
        /// Deletes a catalog by its ID.
        /// </summary>
        /// <param name="id">The catalog ID.</param>
        /// <returns>A success message after deletion.</returns>
        [Authorize(Policy = "Admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _catalog.DeleteAsync(id);
            return Ok(new { message = "Catalog Deleted Successfully" });
        }
    }
}
