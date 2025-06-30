using AutoMapper;
using BookLendingBackUp.Application.DTOs;
using BookLendingBackUp.Application.Interfaces;
using BookLendingBackUp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace BookLendingBackUp.Controllers
{
    
    /// <summary>
    /// Controller for managing books in the library system.
    /// Supports operations for retrieving, creating, updating, and deleting books.
    /// Uses AutoMapper to map between Book entities and BookDTOs.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBook _Ibook;
        private readonly IMapper _imapper;
        
        public BookController(IBook book, IMapper mapper)
        {
            _Ibook = book;
            _imapper = mapper;
           
        }


        /// <summary>
        /// Retrieves a list of all books.
        /// </summary>
        /// <returns>List of books.</returns>
        [HttpGet("GetAllBooks")]
        public async Task<IActionResult> GetAll() =>
        Ok(await _Ibook.GetAllAsync());


        /// <summary>
        /// Retrieves details of a specific book by its ID.
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns>Book details if found; otherwise NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _Ibook.GetByIdAsync(id);

            if (result == null)
                return NotFound("Book not found");

            return Ok(result);
        }


        /// <summary>
        /// Retrieves all books associated with a specific catalog ID.
        /// </summary>
        /// <param name="id">Catalog ID</param>
        /// <returns>List of books under the given catalog.</returns>
        [HttpGet("{id}/GetAllcatalogBooks")]
        public async Task<IActionResult> GetByCatalogIdAsync(int id) =>
               Ok(await _Ibook.GetByCatalogIdAsync(id));


        /// <summary>
        /// Creates a new book.
        /// Requires Admin role.
        /// Uses AutoMapper to map BookDTO to Book entity.
        /// </summary>
        /// <param name="bookdto">Book data transfer object</param>
        /// <returns>The created book and a success message.</returns>
        [Authorize(Policy = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(BookDTO bookdto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("User ID not found in token.");

            var book = _imapper.Map<Book>(bookdto);

            var created = await _Ibook.CreateAsync(book);
            var result = _imapper.Map<BookDTO>(created);

            return Ok(new
            {
                message = "Book Created Successfully",
                data = result
            });
        }

        /// <summary>
        /// Updates an existing book.
        /// Requires Admin role.
        /// Uses AutoMapper to map BookDTO to Book entity.
        /// </summary>
        /// <param name="id">ID of the book to update</param>
        /// <param name="bookdto">Updated book details</param>
        /// <returns>Success message after update.</returns>
        [Authorize(Policy = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BookDTO bookdto)
        {
            var book = _imapper.Map<Book>(bookdto);
            var updated = await _Ibook.UpdateAsync(id, bookdto);

            return Ok(new { message = "Updated Successfully" });
        }



        /// <summary>
        /// Deletes a book by ID.
        /// Requires Admin role.
        /// </summary>
        /// <param name="id">ID of the book to delete</param>
        /// <returns>Success message after deletion.</returns>
        [Authorize(Policy = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _Ibook.DeleteAsync(id);
            return Ok(new { message = "Book Deleted Sucessfully" });
        }


    }
}