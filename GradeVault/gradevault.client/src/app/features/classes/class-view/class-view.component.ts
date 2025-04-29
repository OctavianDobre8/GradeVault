import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ClassesService } from '../../../core/services/classes.service';
import { Class } from '../../../shared/models/class.model';

/**
 * @brief Component for displaying a student's enrolled classes
 * 
 * This component shows all classes a student is enrolled in and
 * allows navigation to view grades for each class.
 */
@Component({
  selector: 'app-class-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './class-view.component.html',
  styleUrl: './class-view.component.css',
})
export class ClassViewComponent implements OnInit {
  /**
   * @brief Collection of classes the student is enrolled in
   */
  classes: Class[] = [];
  
  /**
   * @brief Flag indicating whether data is being loaded
   */
  isLoading: boolean = false;
  
  /**
   * @brief Error message to display when operations fail
   */
  error: string | null = null;

  /**
   * @brief Constructor for the class view component
   * 
   * @param classesService Service for fetching class data
   * @param router Angular router for navigation
   */
  constructor(private classesService: ClassesService, private router: Router) {}

  /**
   * @brief Lifecycle hook that runs when the component initializes
   * 
   * Fetches the student's enrolled classes
   */
  ngOnInit(): void {
    this.fetchClasses();
  }

  /**
   * @brief Fetches classes for the current student
   * 
   * Retrieves all classes the student is enrolled in from the API
   */
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

  /**
   * @brief Navigates to the grades view for a specific class
   * 
   * @param classId ID of the class to view grades for
   */
  viewClassGrades(classId: number): void {
    this.router.navigate(['/grades', classId]);
  }
}