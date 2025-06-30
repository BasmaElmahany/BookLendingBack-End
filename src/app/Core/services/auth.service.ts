import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { LoginDto, RegisterUserDto, TokenPayload } from "../models/auth.model";
import { jwtDecode } from "jwt-decode";

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = 'http://localhost:12957/api/Auth';
  private readonly tokenKey = 'jwt';
  private readonly userInfoKey = 'user-info';

  constructor(private http: HttpClient) {}

  // ✅ Send login request
  login(data: LoginDto): Observable<{ token: string }> {
    return this.http.post<{ token: string }>(`${this.apiUrl}/login`, data);
  }

  // ✅ Register a new user
  register(data: RegisterUserDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, data, {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  // ✅ Save token and decoded user info
  storeToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
    const decoded = this.decodeToken(token);
    if (decoded) {
      localStorage.setItem(this.userInfoKey, JSON.stringify(decoded));
    }
  }

  // ✅ Get token from localStorage
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  // ✅ Decode JWT to get user info
  decodeToken(token?: string): TokenPayload | null {
    const jwt = token || this.getToken();
    if (!jwt) return null;

    try {
      return jwtDecode<TokenPayload>(jwt);
    } catch (error) {
      console.error('Failed to decode token:', error);
      return null;
    }
  }

  // ✅ Get user info from localStorage (decoded)
  getUserInfo(): TokenPayload | null {
    const data = localStorage.getItem(this.userInfoKey);
    return data ? JSON.parse(data) : null;
  }

  // ✅ Clear stored token and user info
  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userInfoKey);
  }
}
