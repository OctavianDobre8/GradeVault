import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * @brief Guard to prevent authenticated users from accessing auth pages
 * 
 * This guard prevents users who are already authenticated from accessing
 * pages like login and registration, redirecting them to their appropriate
 * role-based dashboard instead.
 */
@Injectable({
  providedIn: 'root'
})
export class NoAuthGuard implements CanActivate {
  /**
   * @brief Constructor for the NoAuthGuard
   * 
   * @param authService Authentication service for checking user status
   * @param router Angular router service for navigation
   */
  constructor(private authService: AuthService, private router: Router) {}

  /**
   * @brief Determines if a route can be activated based on authentication status
   * 
   * Checks if the current user is logged in. If they are, redirects to the
   * appropriate dashboard based on their role. If not, allows access to the route.
   * 
   * @returns boolean True if the user is not authenticated, false otherwise
   */
  canActivate(): boolean {
    // If user is already logged in, redirect to appropriate dashboard
    if (this.authService.isLoggedIn()) {
      if (this.authService.isTeacher()) {
        this.router.navigate(['/teacher']);
      } else {
        this.router.navigate(['/student']);
      }
      return false;
    }

    // Not logged in so allow access to auth pages
    return true;
  }
}