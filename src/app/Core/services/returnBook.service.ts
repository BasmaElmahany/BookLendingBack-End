import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReturnBookService {
  private apiUrl = 'http://localhost:12957/api/BorrowBook';

  constructor(private http: HttpClient) {}

  returnBook(borrowId: number): Observable<any> {
    const params = new HttpParams().set('borrowId', borrowId.toString());
    return this.http.post(`${this.apiUrl}/return`, null, { params });
  }
}
