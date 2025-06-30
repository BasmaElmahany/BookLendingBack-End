import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { BookService } from '../../Core/services/book.service';
import { BorrowBookService } from '../../Core/services/borrowbook.Service';
import { Book } from '../../Core/models/book.model';
import { AuthService } from '../../Core/services/auth.service';
import { ReturnBookService } from '../../Core/services/returnBook.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-book-details',
  templateUrl: './book-details.component.html',
  standalone: true,
  imports: [CommonModule, RouterModule],
})
export class BookDetailsComponent implements OnInit {
  book!: Book;
  bookId!: number;
  loading = true;
successMessage: string = '';
returnDate: Date | null = null;
returnMessage: string = '';
returnError: string = '';
  constructor(
     private router: Router,
    private route: ActivatedRoute,
    private bookService: BookService,
    private BorrowBookService :BorrowBookService ,
    private authService :AuthService,
   private ReturnBookService :ReturnBookService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const idParam = params.get('id');
      if (idParam) {
        this.bookId = +idParam;
        this.loadBookById(this.bookId);
      }
    });
  }

  loadBookById(id: number): void {
    this.bookService.getById(id).subscribe({
      next: (data) => {
        this.book = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to fetch book details', err);
        this.loading = false;
      }
    });
  }



borrowBook(bookId: number): void {
  const user = this.authService.getUserInfo();
  const userId = user?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];

  if (!userId) {
    alert('You are not logged in.');
    return;
  }

  this.BorrowBookService.borrowBook(userId, bookId).subscribe({
    next: () => {
      this.successMessage = '✅ Book borrowed successfully!';
      this.returnDate = new Date();
      this.returnDate.setDate(this.returnDate.getDate() + 7);
      this.book.isAvailable = false;
    },
    error: (err) => {
      console.error('Error borrowing book:', err);

      // Check backend error (example: 400 Bad Request or specific message)
      if (
        err?.error?.message?.includes('already borrowed') || 
        err?.status === 400
      ) {
        this.successMessage = '⚠️ You can borrow only one book at a time.';
      } else {
        this.successMessage = '❌ Failed to borrow the book. Please try again later.';
      }
    }
  });
}
returnBook(borrowId: number): void {
  this.returnMessage = '';
  this.returnError = '';

  this.ReturnBookService.returnBook(borrowId).subscribe({
    next: () => {
  this.successMessage = '✅ Book returned successfully!';
  this.returnDate = new Date();
  this.returnMessage = ''; 
},
  error: (err) => {
  const apiMsg = err.error?.message || '❌ Failed to return book.';
  this.returnMessage = apiMsg;
  this.returnDate = null; 
  this.successMessage = ''
  }
  });
}


logout(): void {
    this.authService.logout();
  }


  goHome(): void {
  this.router.navigate(['/home']);
}

}
