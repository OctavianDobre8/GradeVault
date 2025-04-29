import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { Router, RouterModule } from '@angular/router';

/**
 * @brief Main dashboard component for student users
 * 
 * This component serves as the landing page and navigation hub for students,
 * providing quick access to view classes and grades.
 */
@Component({
  selector: 'app-student-dashboard',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './student-dashboard.component.html',
  styleUrls: ['./student-dashboard.component.css'],
})
export class StudentDashboardComponent implements OnInit {
  /**
   * @brief Full name of the student for display
   */
  studentName: string = '';

  /**
   * @brief Constructor for the student dashboard component
   * 
   * @param authService Authentication service for user information
   * @param router Angular router for navigation
   */
  constructor(private authService: AuthService, private router: Router) {}

  /**
   * @brief Lifecycle hook that runs when the component initializes
   * 
   * Sets up the student's name from the authentication service
   */
  ngOnInit(): void {
    const currentUser = this.authService.currentUserValue;
    if (currentUser) {
      this.studentName = `${currentUser.firstName} ${currentUser.lastName}`;
    }
  }

  /**
   * @brief Logs out the current user
   * 
   * Calls the authentication service to log out and redirects to the login page
   */
  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/auth/login']);
      },
      error: (error) => {
        console.error('Logout error', error);
      },
    });
  }
}