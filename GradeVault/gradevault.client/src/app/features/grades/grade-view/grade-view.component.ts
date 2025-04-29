import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Grade } from '../../../shared/models/grade.model';
import { GradesService } from '../../../core/services/grades.service';

/**
 * @brief Component for displaying a student's overall grades
 * 
 * This component shows all grades a student has received across all classes,
 * providing a comprehensive view of their academic performance.
 */
@Component({
  selector: 'app-grade-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './grade-view.component.html',
  styleUrl: './grade-view.component.css',
})
export class GradeViewComponent implements OnInit {
  /**
   * @brief Collection of all the student's grades
   */
  grades: Grade[] = [];
  
  /**
   * @brief Flag indicating whether data is being loaded
   */
  isLoading: boolean = false;
  
  /**
   * @brief Error message to display when operations fail
   */
  error: string | null = null;
  
  /**
   * @brief Constructor for the grade view component
   * 
   * @param gradesService Service for fetching grade data
   */
  constructor(private gradesService: GradesService) {}

  /**
   * @brief Lifecycle hook that runs when the component initializes
   * 
   * Fetches all grades for the current student
   */
  ngOnInit(): void {
    this.fetchGrades();
  }

  /**
   * @brief Fetches all grades for the current student
   * 
   * Retrieves all grades across all classes from the API
   */
  fetchGrades(): void {
    this.isLoading = true;
    this.error = null;

    this.gradesService.getMyGrades().subscribe({
      next: (data) => {
        this.grades = data;
        this.isLoading = false;
        //console.log('Grades fetched:', this.grades);
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