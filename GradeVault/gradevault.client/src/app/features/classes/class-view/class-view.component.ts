import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ClassesService } from '../../../core/services/classes.service';
import { Class } from '../../../shared/models/class.model';

@Component({
  selector: 'app-class-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './class-view.component.html',
  styleUrl: './class-view.component.css',
})
export class ClassViewComponent implements OnInit {
  classes: Class[] = [];
  isLoading: boolean = false;
  error: string | null = null;

  constructor(private classesService: ClassesService, private router: Router) {}

  ngOnInit(): void {
    this.fetchClasses();
  }

  fetchClasses(): void {
    this.isLoading = true;
    this.error = null;

    this.classesService.getMyClasses().subscribe({
      next: (data) => {
        this.classes = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching classes:', err);
        this.error = 'Failed to load classes. Please try again later.';
        if (err.status === 401 || err.status === 403) {
          this.error = 'You are not authorized to view these classes.';
        } else if (err.status === 404) {
          this.error = 'No student profile found for your account.';
        }
        this.isLoading = false;
      },
    });
  }

  viewClassGrades(classId: number): void {
    this.router.navigate(['/grades', classId]);
  }
}
