import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Catalog } from '../models/catalog.model';

interface ApiResponse<T> {
  data?: T;
  message: string;
}

@Injectable({
  providedIn: 'root',
})
export class CatalogService {
  private apiUrl = 'http://localhost:12957/api/Catalog';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Catalog[]> {
    return this.http.get<Catalog[]>(`${this.apiUrl}/GetAll`);
  }

  getById(id: number): Observable<Catalog> {
    return this.http.get<Catalog>(`${this.apiUrl}/GetbyId/${id}`);
  }

  create(catalog: Catalog): Observable<ApiResponse<Catalog>> {
    return this.http.post<ApiResponse<Catalog>>(`${this.apiUrl}/PostCatalog`, catalog);
  }

  update(id: number, catalog: Catalog): Observable<ApiResponse<Catalog>> {
    return this.http.put<ApiResponse<Catalog>>(`${this.apiUrl}/Putcatalog/${id}`, catalog);
  }

  delete(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.apiUrl}/Delete/${id}`);
  }
}
