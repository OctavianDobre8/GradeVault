import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class NoAuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

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
