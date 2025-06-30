import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, NgModel, FormsModule } from '@angular/forms';
import { Book } from '../../Core/models/book.model';
import { BookService } from '../../Core/services/book.service';
import { CatalogService } from '../../Core/services/catalog.service';
import { ToastrService, ToastrModule } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-book',
  templateUrl: './book.component.html',
  styleUrls: ['./book.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    ToastrModule , 
       FormsModule 
  ]
})
export class BookComponent implements OnInit {
  bookForm!: FormGroup;
  books: Book[] = [];
  catalogs: any[] = [];
  selectedId: number | null = null;
searchTerm: string = '';
currentPage: number = 1;
itemsPerPage: number = 5;
  constructor(
    private fb: FormBuilder,
    private bookService: BookService,
    private catalogService: CatalogService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.bookForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(2)]],
      description: ['', [Validators.required, Validators.minLength(5)]],
      author: ['', [Validators.required, Validators.minLength(3)]],
      publishedAt: ['', Validators.required],
      isAvailable: [true],
      catalogId: [null, Validators.required]
    });

    this.loadBooks();
    this.catalogService.getAll().subscribe(c => this.catalogs = c);
  }

get f() {
  return this.bookForm.controls;
}

  loadBooks(): void {
    this.bookService.getAll().subscribe(data => {
      this.books = data;
    });
  }

submit(): void {
  if (this.bookForm.invalid) {
    this.bookForm.markAllAsTouched();
    this.toastr.error('❗ Please fix the form errors.');
    return;
  }

  const book: Book = {
    id: this.selectedId ?? 0,
    ...this.bookForm.value
  };

  if (this.selectedId) {
    this.bookService.update(this.selectedId, book).subscribe(res => {
      const updatedBook = res.data;
      const index = this.books.findIndex(b => b.id === this.selectedId);
      if (index !== -1 && updatedBook) {
        this.books[index] = updatedBook;
        this.toastr.success(res.message || '✅ Book updated successfully!');
      }
      this.resetForm();
    });
  } else {
    this.bookService.create(book).subscribe(res => {
      if (res.data) {
        this.books.push(res.data);
        this.toastr.success(res.message || '📘 Book added successfully!');
      }
      this.resetForm();
    });
  }
}




  edit(book: Book): void {
    this.selectedId = book.id;
    this.bookForm.patchValue({
      title: book.title,
      author: book.author,
      description: book.description,
      publishedAt: book.publishedAt,
      isAvailable: book.isAvailable,
      catalogId: book.catalogId
    });
    document.getElementById('book-form')?.scrollIntoView({ behavior: 'smooth' });
  }

delete(id: number): void {
  this.bookService.delete(id).subscribe(res => {
    this.books = this.books.filter(b => b.id !== id);
    this.toastr.success(res.message || '❌ Book deleted successfully!');
  });
}
  resetForm(): void {
    this.selectedId = null;
    this.bookForm.reset({ isAvailable: true });
  }


  get filteredBooks(): Book[] {
  const filtered = this.books.filter(b =>
    b.title.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
    b.author.toLowerCase().includes(this.searchTerm.toLowerCase())
  );

  const start = (this.currentPage - 1) * this.itemsPerPage;
  return filtered.slice(start, start + this.itemsPerPage);
}

get totalPages(): number {
  const filteredCount = this.books.filter(b =>
    b.title.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
    b.author.toLowerCase().includes(this.searchTerm.toLowerCase())
  ).length;
  return Math.ceil(filteredCount / this.itemsPerPage);
}

changePage(delta: number): void {
  const newPage = this.currentPage + delta;
  if (newPage >= 1 && newPage <= this.totalPages) {
    this.currentPage = newPage;
  }
}
}
