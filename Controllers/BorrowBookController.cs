using BookLendingBackUp.Domain.Entities;
using BookLendingBackUp.Infrastructure.Interfaces;
using BookLendingBackUp.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookLendingBackUp.Controllers
{
    /// <summary>
    /// Controller responsible for borrowing and returning books.
    /// Includes functionality for tracking delayed books and overdue check operations.
    /// Requires authorization via policies.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowBookController : ControllerBase
    {
        private readonly IBorrowBook _borrowService;

        private readonly DelayedBookCheckerService _DelayedBookCheckerService;
        public BorrowBookController(IBorrowBook borrowBookService , DelayedBookCheckerService DelayedBookCheckerService)
        {
            _borrowService = borrowBookService;

            _DelayedBookCheckerService = DelayedBookCheckerService;
        }


        /// <summary>
        /// Borrows a book for a specific user.
        /// Only accessible by users with Admin or Member roles.
        /// </summary>
        /// <param name="userId">ID of the user borrowing the book</param>
        /// <param name="bookId">ID of the book to borrow</param>
        /// <returns>Details of the borrowed book and due date</returns>
        [Authorize(Policy = "AdminOrMember")]
        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBook(string userId, int bookId)
        {
            try
            {
                var borrow = await _borrowService.BorrowBookAsync(userId, bookId);
                return Ok(new
                {
                    message = "Book borrowed successfully",
                    borrow.Id,
                    borrow.BookId,
                    borrow.BorrowedAt,
                    borrow.DueDate
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Returns a borrowed book based on the borrow record ID.
        /// Only accessible by users with Admin or Member roles.
        /// </summary>
        /// <param name="borrowId">ID of the borrow record</param>
        /// <returns>Success message if the book is returned; NotFound if invalid</returns>
        [Authorize(Policy = "AdminOrMember")]
        [HttpPost("return")]
        public async Task<IActionResult> ReturnBook([FromQuery] int borrowId)
        {
            var success = await _borrowService.ReturnBookAsync(borrowId);
            if (!success)
                return NotFound("Borrow record not found or already returned");

            return Ok("Book returned successfully");
        }

        /// <summary>
        /// Manually triggers the background service that checks for overdue books.
        /// Only accessible by Admins.
        /// </summary>
        [Authorize(Policy = "Admin")]
        [HttpGet("DelayedBookCheckerService")]
        public async Task<IActionResult> DelayedBookCheckerService() => Ok(await _DelayedBookCheckerService.CheckOverdueBooksAsync());


        /// <summary>
        /// Retrieves a list of books that are currently overdue.
        /// Only accessible by Admins.
        /// </summary>
        /// <returns>List of delayed borrow records</returns>
        [Authorize(Policy = "Admin")]
        [HttpGet("DelayedBooks")]
        public async Task<IActionResult> DelayedBooks() => Ok(await _borrowService.DelayedBooks());


    }
}
