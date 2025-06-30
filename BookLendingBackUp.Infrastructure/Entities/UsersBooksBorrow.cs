using BookLendingBackUp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingBackUp.Infrastructure.Entities
{
    public class UsersBooksBorrow
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserID { get; set; }

        [ForeignKey(nameof(ApplicationUserID))]
        public ApplicationUser? ApplicationUser { get; set; }

        [Required]
        public int BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book? Book { get; set; }
        public DateTime BorrowedAt { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }  //BorrowedAt + 7 days

        public bool BookReturned { get; set; } = false;

        // make returned = true after 7 days then make bookIsAvailable = true 


    }
}
