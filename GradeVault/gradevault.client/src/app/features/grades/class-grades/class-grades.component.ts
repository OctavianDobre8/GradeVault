import { CommonModule, DecimalPipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Grade } from '../../../shared/models/grade.model';
import { GradesService } from '../../../core/services/grades.service';

/**
 * @brief Component for displaying a student's grades for a specific class
 * 
 * This component shows all grades a student has received in a particular class,
 * along with an average grade calculation for the class.
 */
@Component({
  selector: 'app-class-grades',
  standalone: true,
  imports: [CommonModule, DecimalPipe],
  templateUrl: './class-grades.component.html',
  styleUrl: './class-grades.component.css',
})
export class ClassGradesComponent implements OnInit {
  /**
   * @brief ID of the class being viewed
   */
  classId: number | null = null;
  
  /**
   * @brief Name of the class being viewed
   */
  className: string | null = null;
  
  /**
   * @brief Collection of grades for the student in this class
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
   * @brief Average grade value for the student in this class
   */
  averageGrade: number | null = null;

  /**
   * @brief Constructor for the class grades component
   * 
   * @param route Activated route for accessing route parameters
   * @param router Angular router for navigation
   * @param gradesService Service for fetching grade data
   */
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private gradesService: GradesService
  ) {}

  /**
   * @brief Lifecycle hook that runs when the component initializes
   * 
   * Extracts the class ID from the route parameters and fetches grades
   */
  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      const id = +params['id'];
      if (!isNaN(id)) {
        this.classId = id;
        this.fetchClassGrades(id);
      } else {
        this.router.navigate(['/classes']);
      }
    });
  }

  /**
   * @brief Fetches grades for the current student in the specified class
   * 
   * @param classId ID of the class to fetch grades for
   */
  fetchClassGrades(classId: number): void {
    this.isLoading = true;
    this.error = null;
    this.averageGrade = null;

    this.gradesService.getGradesByClass(classId).subscribe({
      next: (data) => {
        this.grades = data;
        this.isLoading = false;

        if (data.length > 0 && data[0].className) {
          this.className = data[0].className;
        }
        this.calculateAverage();
      },
      error: (err) => {
        console.error('Error fetching class grades:', err);
        this.error = 'Failed to load grades for this class.';
        if (err.status === 401 || err.status === 403) {
          this.error = 'You are not authorized to view these grades.';
        } else if (err.status === 404) {
          this.error = 'No student profile found for your account.';
        }
        this.isLoading = false;
      },
    });
  }

  /**
   * @brief Navigates back to the classes list view
   */
  goBackToClasses(): void {
    this.router.navigate(['/classes']);
  }

  /**
   * @brief Calculates the average grade for the class
   * 
   * Sets the averageGrade property if there are at least two grades,
   * otherwise sets it to null.
   */
  calculateAverage(): void {
    if (this.grades && this.grades.length >= 2) {
      const sum = this.grades.reduce((acc, grade) => acc + grade.value, 0);
      this.averageGrade = sum / this.grades.length;
    } else {
      this.averageGrade = null;
    }
  }
}