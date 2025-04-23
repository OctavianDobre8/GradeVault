import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Grade } from '../../../shared/models/grade.model';
import { GradesService } from '../../../core/services/grades.service';

@Component({
  selector: 'app-grade-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './grade-view.component.html',
  styleUrl: './grade-view.component.css',
})
export class GradeViewComponent implements OnInit {
  grades: Grade[] = []; 
  isLoading: boolean = false;
  error: string | null = null; 
  constructor(private gradesService: GradesService) {}

  ngOnInit(): void {
    this.fetchGrades();
  }

  fetchGrades(): void {
    this.isLoading = true; 
    this.error = null; 

    this.gradesService.getMyGrades().subscribe({
      next: (data) => {
        this.grades = data; 
        this.isLoading = false; 
        console.log('Grades fetched:', this.grades); 
      },
      error: (err) => {
        console.error('Error fetching grades:', err);
        this.error = 'Failed to load grades. Please try again later.';
        if (err.status === 401 || err.status === 403) {
          this.error = 'You are not authorized to view these grades.';
        } else if (err.status === 404) {
          this.error = 'No student profile found for your account.';
        }
        this.isLoading = false; 
        this.grades = [];
      },
    });
  }
}
