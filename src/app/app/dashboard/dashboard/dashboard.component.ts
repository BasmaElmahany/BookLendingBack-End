import { Component } from '@angular/core';
import { CatalogComponent } from "../../catalog/catalog.component";
import { BookComponent } from "../../book/book.component";
import { CommonModule } from '@angular/common';
import { DelayedBooksComponent } from '../../delayed-books/delayed-books.component';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  imports: [CatalogComponent, BookComponent , CommonModule , DelayedBooksComponent  ]
})
export class DashboardComponent {
  showCategory = false;
  showBook = false;
  showDelayedBooks = false;

  toggleCategory() {
    this.showCategory = !this.showCategory;
    if (this.showCategory) {
      this.showBook = false;
      this.showDelayedBooks = false;
    }
  }

  toggleBook() {
    this.showBook = !this.showBook;
    if (this.showBook) {
      this.showCategory = false;
      this.showDelayedBooks = false;
    }
  }

  toggleDelayedBooks() {
    this.showDelayedBooks = !this.showDelayedBooks;
    if (this.showDelayedBooks) {
      this.showCategory = false;
      this.showBook = false;
    }
  }
}
