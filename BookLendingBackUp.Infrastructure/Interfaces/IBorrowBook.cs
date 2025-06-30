using BookLendingBackUp.Application.DTOs;
using BookLendingBackUp.Domain.Entities;
using BookLendingBackUp.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingBackUp.Infrastructure.Interfaces
{
    public interface IBorrowBook
    {
        Task<UsersBooksBorrow> BorrowBookAsync(string userId, int bookId);
        Task<bool> ReturnBookAsync(int borrowId);
        Task<IEnumerable<DelayedBookDto>> DelayedBooks ();

    }
}
