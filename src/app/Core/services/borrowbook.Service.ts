import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs'; @Injectable({
  providedIn: 'root'
})
export class BorrowBookService {
  private apiUrl = 'http://localhost:12957/api/BorrowBook';

  constructor(private http: HttpClient) {}

  borrowBook(userId: string, bookId: number): Observable<any> {
    const params = new HttpParams()
      .set('userId', userId)
      .set('bookId', bookId.toString());

    return this.http.post(`${this.apiUrl}/borrow`, null, { params });
  }
}