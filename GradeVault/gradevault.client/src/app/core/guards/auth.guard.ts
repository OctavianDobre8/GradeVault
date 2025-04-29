import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * @brief Authentication guard for route protection
 * 
 * This guard prevents unauthorized access to protected routes in the application.
 * It checks if the user is logged in and has the required role for accessing a route.
 */
@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  /**
   * @brief Constructor for the AuthGuard
   * 
   * @param router Angular router service for navigation
   * @param authService Authentication service for checking user status
   */
  constructor(private router: Router, private authService: AuthService) {}

  /**
   * @brief Determines if a route can be activated
   * 
   * Checks if the current user is authenticated and has the required role.
   * Redirects to login page if not authenticated or to home page if not authorized.
   * 
   * @param route Information about the route being activated
   * @param state Current router state information
   * @returns boolean True if the user can access the route, false otherwise
   */
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const currentUser = this.authService.currentUserValue;

    if (currentUser) {
      // Check if route is restricted by role
      if (route.data['roles'] && !route.data['roles'].includes(currentUser.role)) {
        // Role not authorized so redirect to home page
        this.router.navigate(['/']);
        return false;
      }

      // Authorized so return true
      return true;
    }

    // Not logged in so redirect to login page with the return url
    this.router.navigate(['/auth/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }
}