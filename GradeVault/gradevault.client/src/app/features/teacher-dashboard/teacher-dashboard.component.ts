import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';

/**
 * @brief Main dashboard component for teacher users
 * 
 * This component serves as the landing page and navigation hub for teachers,
 * providing quick access to class management, grade management, and student management.
 */
@Component({
  selector: 'app-teacher-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './teacher-dashboard.component.html',
  styleUrls: ['./teacher-dashboard.component.css']
})
export class TeacherDashboardComponent implements OnInit {
  /**
   * @brief Full name of the teacher for display
   */
  teacherName: string = '';

  /**
   * @brief Constructor for the teacher dashboard component
   * 
   * @param authService Authentication service for user information
   * @param router Angular router for navigation
   */
  constructor(private authService: AuthService, private router: Router) { }

  /**
   * @brief Lifecycle hook that runs when the component initializes
   * 
   * Sets up the teacher's name from the authentication service
   */
  ngOnInit(): void {
    const user = this.authService.currentUserValue;
    if (user) {
      this.teacherName = `${user.firstName} ${user.lastName}`;
    }
  }

  /**
   * @brief Navigates to the class management page
   */
  navigateToClassManagement(): void {
    this.router.navigate(['/teacher/classes']);
  }

  /**
   * @brief Navigates to the grade management page
   */
  navigateToGradeManagement(): void {
    this.router.navigate(['/teacher/grades']);
  }

  /**
   * @brief Navigates to the student management page
   */
  navigateToStudentManagement(): void {
    this.router.navigate(['/teacher/students']);
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
        // Even if there's an error, try to redirect to login
        this.router.navigate(['/auth/login']);
      }
    });
  }
}