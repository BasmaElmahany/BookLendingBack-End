import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs'; 
import { DelayedBookDto } from '../models/DelayedBookDto';


@Injectable({
  providedIn: 'root'
})
export class DelayedBookService {
  private readonly apiUrl = 'http://localhost:12957/api/BorrowBook';

  constructor(private http: HttpClient) {}

  getDelayedBooks(): Observable<DelayedBookDto[]> {
    return this.http.get<DelayedBookDto[]>(`${this.apiUrl}/DelayedBooks`);
  }
}