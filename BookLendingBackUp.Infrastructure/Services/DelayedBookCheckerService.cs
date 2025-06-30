using BookLendingBackUp.Application.Interfaces;
using BookLendingBackUp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;



namespace BookLendingBackUp.Infrastructure.Services
{
    public class DelayedBookCheckerService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DelayedBookCheckerService> _logger;
        private readonly IEmailSender _emailSender;
        public DelayedBookCheckerService(ApplicationDbContext context, ILogger<DelayedBookCheckerService> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task<List<Entities.UsersBooksBorrow>> CheckOverdueBooksAsync()
        {
            var overdueBooks = await _context.UsersBooksBorrows
                .Include(b => b.Book)
                .Include(b => b.ApplicationUser)
                .Where(b => !b.BookReturned && b.DueDate < DateTime.UtcNow)
                .ToListAsync();
            foreach (var borrow in overdueBooks)
            {
                var email = borrow.ApplicationUser?.Email;
                var bookTitle = borrow.Book.Title;
                var userName = borrow.ApplicationUser?.FullName ?? "User";

                _logger.LogInformation($"Reminder: '{bookTitle}' is overdue for user {userName}.");

                if (!string.IsNullOrEmpty(email))
                {
                    string subject = "Book Overdue Reminder";
                    string body = $"Dear {userName},\n\nThe book '{bookTitle}' you borrowed is overdue. Please return it as soon as possible.\n\nThanks,\nLibrary System";

                    await _emailSender.SendEmailAsync(email, subject, body);
                }
            }


            return overdueBooks;
        }

    }
}
