import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { User } from '../../shared/models/user.model';
import { LoginRequest, RegisterRequest, ForgotPasswordRequest, ResetPasswordRequest } from '../../shared/models/auth.model';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser: Observable<User | null>;

  constructor(private http: HttpClient) {
    const storedUser = localStorage.getItem('currentUser');
    this.currentUserSubject = new BehaviorSubject<User | null>(storedUser ? JSON.parse(storedUser) : null);
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }
  
  login(loginData: LoginRequest): Observable<User> {
    return this.http.post<User>('/api/auth/login', loginData)
      .pipe(
        tap(user => {
          // Store the user
          localStorage.setItem('currentUser', JSON.stringify(user));
          
          // If rememberMe is false, store the expiration time (e.g., browser session)
          if (!loginData.rememberMe) {
            // Session-only storage - will be cleared when browser closes
            sessionStorage.setItem('userExpiration', 'session');
          } else {
            // Store a longer expiration time
            const expirationDate = new Date();
            expirationDate.setDate(expirationDate.getDate() + 30); // 30 days
            localStorage.setItem('userExpiration', expirationDate.toString());
          }
          
          this.currentUserSubject.next(user);
        })
      );
  }

  register(userData: RegisterRequest): Observable<any> {
    return this.http.post<any>('/api/auth/register', userData);
  }

  logout(): Observable<any> {
    return this.http.post<any>('/api/auth/logout', {}).pipe(
      tap(() => {
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
      })
    );
  }

  forgotPassword(email: string): Observable<any> {
    return this.http.post<any>('/api/auth/forgot-password', { email });
  }

  validateResetToken(email: string, token: string): Observable<any> {
    return this.http.get<any>(`/api/auth/validate-reset-token?email=${encodeURIComponent(email)}&token=${encodeURIComponent(token)}`);
  }

  resetPassword(resetData: ResetPasswordRequest): Observable<any> {
    return this.http.post<any>('/api/auth/reset-password', resetData);
  }

  isLoggedIn(): boolean {
    return !!this.currentUserValue;
  }

  isTeacher(): boolean {
    return this.currentUserValue?.role === 'Teacher';
  }

  isStudent(): boolean {
    return this.currentUserValue?.role === 'Student';
  }

  getUserId(): string | null {
    return this.currentUserValue?.id || null;
  }
}
