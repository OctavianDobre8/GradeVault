import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * @brief Role-based guard for route protection
 * 
 * This guard restricts access to routes based on the specific role of the authenticated user.
 * Unlike the AuthGuard that checks for authentication and a list of roles, this guard
 * checks for a single specific role requirement.
 */
@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  /**
   * @brief Constructor for the RoleGuard
   * 
   * @param authService Authentication service for checking user status and role
   * @param router Angular router service for navigation
   */
  constructor(private authService: AuthService, private router: Router) {}

  /**
   * @brief Determines if a route can be activated based on user role
   * 
   * Checks if the current user is authenticated and has exactly the required role.
   * Redirects to login page if not authenticated or to home page if wrong role.
   * 
   * @param route Information about the route being activated, containing role data
   * @returns boolean True if the user has the exact required role, false otherwise
   */
  canActivate(route: ActivatedRouteSnapshot): boolean {
    const expectedRole = route.data['role'];

    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/auth/login']);
      return false;
    }

    const currentUser = this.authService.currentUserValue;

    if (currentUser && currentUser.role === expectedRole) {
      return true;
    }

    this.router.navigate(['/']);
    return false;
  }
}