import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { BookService } from '../../Core/services/book.service';
import { CatalogService } from '../../Core/services/catalog.service';
import { AuthService } from '../../Core/services/auth.service';
import { Book } from '../../Core/models/book.model';
import { Catalog } from '../../Core/models/catalog.model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,FormsModule 
  ],
  templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
  catalogs: Catalog[] = [];
  catalogBooksMap: { [catalogId: number]: Book[] } = {};
 filteredBooksMap: { [catalogId: number]: Book[] } = {};
  searchTerms: { [catalogId: number]: string } = {};
  currentPageMap: { [catalogId: number]: number } = {};
  pageSize = 4;

  constructor(
    private bookService: BookService,
    private catalogService: CatalogService,
    private authService: AuthService
  ) {}

 ngOnInit(): void {
    this.loadCatalogs();
  }

  loadCatalogs() {
    this.catalogService.getAll().subscribe({
      next: (catalogs) => {
        this.catalogs = catalogs;
        this.catalogs.forEach((catalog) => {
          this.currentPageMap[catalog.id] = 1;
          this.loadCatalogBooks(catalog.id);
        });
      },
      error: (err) => console.error('Failed to load catalogs', err)
    });
  }

  loadCatalogBooks(catalogId: number) {
    this.bookService.getByCatalogId(catalogId).subscribe({
      next: (books) => {
        this.catalogBooksMap[catalogId] = books;
        this.filterBooks(catalogId); // initialize filtered books
      },
      error: (err) => console.error('Failed to load books for catalog', err)
    });
  }

  filterBooks(catalogId: number) {
    const searchTerm = this.searchTerms[catalogId]?.toLowerCase() || '';
    const allBooks = this.catalogBooksMap[catalogId] || [];
    this.filteredBooksMap[catalogId] = allBooks.filter(book =>
      book.title.toLowerCase().includes(searchTerm) ||
      (book.description?.toLowerCase().includes(searchTerm))
    );
    this.currentPageMap[catalogId] = 1; // Reset to first page on new search
  }

  getPaginatedBooks(catalogId: number): Book[] {
    const page = this.currentPageMap[catalogId] || 1;
    const books = this.filteredBooksMap[catalogId] || [];
    const start = (page - 1) * this.pageSize;
    return books.slice(start, start + this.pageSize);
  }

  totalPages(catalogId: number): number[] {
    const total = this.filteredBooksMap[catalogId]?.length || 0;
    return Array(Math.ceil(total / this.pageSize))
      .fill(0)
      .map((_, i) => i + 1);
  }

  setPage(catalogId: number, page: number) {
    this.currentPageMap[catalogId] = page;
  }
  scrollToTop(): void {
  window.scrollTo({ top: 0, behavior: 'smooth' });
}
logout(): void {
    this.authService.logout();
  }
}
