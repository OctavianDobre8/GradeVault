import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

/**
 * @brief HTTP interceptor for adding JWT authentication tokens to API requests
 * 
 * This interceptor automatically adds the JWT authentication token to outgoing
 * HTTP requests to the API. This ensures authenticated access without manually
 * adding the token to each request.
 */
@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  /**
   * @brief Constructor for the JWT interceptor
   * 
   * @param authService Authentication service for accessing the current user's token
   */
  constructor(private authService: AuthService) {}

  /**
   * @brief Intercepts HTTP requests to add authentication header when appropriate
   * 
   * Examines each outgoing request and adds the JWT token to the Authorization header
   * if the user is logged in and the request is going to the API.
   * 
   * @param request The outgoing HTTP request
   * @param next The next handler in the HTTP interceptor chain
   * @returns Observable<HttpEvent<any>> An observable of the HTTP response
   */
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Add auth header with jwt if user is logged in and request is to the api url
    const currentUser = this.authService.currentUserValue;
    const isLoggedIn = currentUser !== null;
    const isApiUrl = request.url.startsWith('/api');

    if (isLoggedIn && isApiUrl) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentUser.token}`
        }
      });
    }

    return next.handle(request);
  }
}