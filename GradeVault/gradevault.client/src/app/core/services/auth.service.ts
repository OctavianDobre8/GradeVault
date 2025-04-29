import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { User } from '../../shared/models/user.model';
import { LoginRequest, RegisterRequest, ForgotPasswordRequest, ResetPasswordRequest } from '../../shared/models/auth.model';

/**
 * @brief Service handling authentication operations
 * 
 * This service manages user authentication state and provides methods for
 * login, registration, password recovery, and checking user permissions.
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  /**
   * @brief Subject containing the current user state
   */
  private currentUserSubject: BehaviorSubject<User | null>;
  
  /**
   * @brief Observable of the current user that components can subscribe to
   */
  public currentUser: Observable<User | null>;

  /**
   * @brief Constructor for the authentication service
   * 
   * Initializes the current user from local storage if available
   * 
   * @param http HttpClient for making API requests
   */
  constructor(private http: HttpClient) {
    const storedUser = localStorage.getItem('currentUser');
    this.currentUserSubject = new BehaviorSubject<User | null>(storedUser ? JSON.parse(storedUser) : null);
    this.currentUser = this.currentUserSubject.asObservable();
  }

  /**
   * @brief Gets the current user value without subscribing
   * 
   * @returns User|null The currently authenticated user or null if not logged in
   */
  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }
  
  /**
   * @brief Authenticates a user with credentials
   * 
   * Sends login request to the API and stores the returned user data
   * locally with appropriate expiration based on remember me option.
   * 
   * @param loginData Login credentials including email, password, and remember me flag
   * @returns Observable<User> Observable that emits the authenticated user
   */
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

  /**
   * @brief Registers a new user account
   * 
   * Sends registration data to the API to create a new user account.
   * 
   * @param userData Registration data including email, password, and personal details
   * @returns Observable<any> Observable that completes when registration succeeds
   */
  register(userData: RegisterRequest): Observable<any> {
    return this.http.post<any>('/api/auth/register', userData);
  }

  /**
   * @brief Logs out the current user
   * 
   * Sends logout request to the API and clears local user data.
   * 
   * @returns Observable<any> Observable that completes when logout succeeds
   */
  logout(): Observable<any> {
    return this.http.post<any>('/api/auth/logout', {}).pipe(
      tap(() => {
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
      })
    );
  }

  /**
   * @brief Initiates the password recovery process
   * 
   * Sends password reset request to the API for the specified email.
   * 
   * @param email Email address of the account to recover
   * @returns Observable<any> Observable that completes when request succeeds
   */
  forgotPassword(email: string): Observable<any> {
    return this.http.post<any>('/api/auth/forgot-password', { email });
  }

  /**
   * @brief Validates a password reset token
   * 
   * Checks if the provided token is valid for resetting the password.
   * 
   * @param email Email associated with the reset token
   * @param token The reset token to validate
   * @returns Observable<any> Observable that completes when validation succeeds
   */
  validateResetToken(email: string, token: string): Observable<any> {
    return this.http.get<any>(`/api/auth/validate-reset-token?email=${encodeURIComponent(email)}&token=${encodeURIComponent(token)}`);
  }

  /**
   * @brief Completes the password reset process
   * 
   * Sends new password along with the reset token to finalize password reset.
   * 
   * @param resetData Data containing email, token, and new password
   * @returns Observable<any> Observable that completes when reset succeeds
   */
  resetPassword(resetData: ResetPasswordRequest): Observable<any> {
    return this.http.post<any>('/api/auth/reset-password', resetData);
  }

  /**
   * @brief Checks if a user is currently logged in
   * 
   * @returns boolean True if user is authenticated, false otherwise
   */
  isLoggedIn(): boolean {
    return !!this.currentUserValue;
  }

  /**
   * @brief Checks if current user has Teacher role
   * 
   * @returns boolean True if user is a teacher, false otherwise
   */
  isTeacher(): boolean {
    return this.currentUserValue?.role === 'Teacher';
  }

  /**
   * @brief Checks if current user has Student role
   * 
   * @returns boolean True if user is a student, false otherwise
   */
  isStudent(): boolean {
    return this.currentUserValue?.role === 'Student';
  }

  /**
   * @brief Gets the ID of the current user
   * 
   * @returns string|null User ID if logged in, null otherwise
   */
  getUserId(): string | null {
    return this.currentUserValue?.id || null;
  }
}