import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-teacher-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './teacher-dashboard.component.html',
  styleUrls: ['./teacher-dashboard.component.css']
})
export class TeacherDashboardComponent implements OnInit {
  teacherName: string = '';

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    const user = this.authService.currentUserValue;
    if (user) {
      this.teacherName = `${user.firstName} ${user.lastName}`;
    }
  }

  navigateToClassManagement(): void {
    this.router.navigate(['/teacher/classes']);
  }

  navigateToGradeManagement(): void {
    this.router.navigate(['/teacher/grades']);
  }

  navigateToStudentManagement(): void {
    this.router.navigate(['/teacher/students']);
  }

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