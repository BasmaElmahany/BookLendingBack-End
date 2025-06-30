import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Book } from '../models/book.model';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private apiUrl = 'http://localhost:12957/api/Book';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Book[]> {
    return this.http.get<Book[]>(`${this.apiUrl}/GetAllBooks`);
  }

  getById(id: number): Observable<Book> {
    return this.http.get<Book>(`${this.apiUrl}/${id}`);
  }

  create(book: Book): Observable<ApiResponse<Book>> {
    return this.http.post<ApiResponse<Book>>(this.apiUrl, book);
  }

  update(id: number, book: Book): Observable<ApiResponse<Book>> {
    return this.http.put<ApiResponse<Book>>(`${this.apiUrl}/${id}`, book);
  }

  delete(id: number): Observable<ApiResponse<string>> {
    return this.http.delete<ApiResponse<string>>(`${this.apiUrl}/${id}`);
  }

    getByCatalogId(catalogId: number): Observable<Book[]> {
    return this.http.get<Book[]>(`${this.apiUrl}/${catalogId}/GetAllcatalogBooks`);
  }
}
