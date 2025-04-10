import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-student-dashboard',
  templateUrl: './student-dashboard.component.html',
  styleUrls: ['./student-dashboard.component.css']
})
export class StudentDashboardComponent implements OnInit {
  studentName: string = '';

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    const currentUser = this.authService.currentUserValue;
    if (currentUser) {
      this.studentName = `${currentUser.firstName} ${currentUser.lastName}`;
    }
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/auth/login']);
      },
      error: error => {
        console.error('Logout error', error);
      }
    });
  }
}
