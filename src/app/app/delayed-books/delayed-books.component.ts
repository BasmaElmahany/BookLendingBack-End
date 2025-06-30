import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DelayedBookService } from '../../Core/services/delayedbook.service';
import { DelayedBookDto } from '../../Core/models/DelayedBookDto';

@Component({
  selector: 'app-delayed-books',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './delayed-books.component.html',
})
export class DelayedBooksComponent implements OnInit {
  delayedBooks: DelayedBookDto[] = [];
  loading = true;

  constructor(private DelayedBookService: DelayedBookService) {}

  ngOnInit(): void {
    this.DelayedBookService.getDelayedBooks().subscribe({
      next: (data) => {
        this.delayedBooks = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('❌ Failed to load delayed books:', err);
        this.loading = false;
      }
    });
  }
}
