import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ClassService } from '../../core/services/class.service';
import { Class } from '../../shared/models/class.model';
import { Student } from '../../shared/models/student.model';

/**
 * @brief Component for managing student enrollments in classes
 * 
 * This component allows teachers to view students in their classes,
 * add new students to classes, and remove students from classes.
 */
@Component({
  selector: 'app-student-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './student-management.component.html',
  styleUrl: './student-management.component.css'
})
export class StudentManagementComponent implements OnInit {
  /**
   * @brief List of classes taught by the teacher
   */
  classes: Class[] = [];
  
  /**
   * @brief List of students enrolled in the selected class
   */
  students: Student[] = [];
  
  /**
   * @brief List of students available to be added to the selected class
   */
  availableStudents: Student[] = [];
  
  /**
   * @brief ID of the currently selected class
   */
  selectedClassId: number | null = null;
  
  /**
   * @brief Flag indicating whether data is being loaded
   */
  isLoading = false;
  
  /**
   * @brief Error message to display when operations fail
   */
  error: string | null = null;

  /**
   * @brief Constructor for the student management component
   * 
   * @param classService Service for class and student operations
   */
  constructor(private classService: ClassService) {}

  /**
   * @brief Lifecycle hook that runs when the component initializes
   * 
   * Loads the list of classes taught by the teacher
   */
  ngOnInit(): void {
    this.loadClasses();
  }

  /**
   * @brief Loads classes taught by the current teacher
   */
  loadClasses(): void {
    this.isLoading = true;
    this.classService.getTeacherClasses().subscribe({
      next: (classes) => {
        this.classes = classes;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading classes', err);
        this.error = 'Failed to load classes. Please try again.';
        this.isLoading = false;
      }
    });
  }

  /**
   * @brief Handles selection of a class
   * 
   * Updates the selected class ID and loads students for the class
   * 
   * @param classId ID of the class that was selected
   */
  selectClass(classId: number): void {
    this.selectedClassId = classId;
    this.loadStudentsByClass(classId);
    this.loadAvailableStudents(classId);
  }

  /**
   * @brief Loads students enrolled in a specific class
   * 
   * @param classId ID of the class to load students for
   */
  loadStudentsByClass(classId: number): void {
    this.isLoading = true;
    this.classService.getStudentsByClass(classId).subscribe({
      next: (students) => {
        this.students = students;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading students', err);
        this.error = 'Failed to load students. Please try again.';
        this.isLoading = false;
      }
    });
  }

  /**
   * @brief Loads students available to be added to a class
   * 
   * @param classId ID of the class to load available students for
   */
  loadAvailableStudents(classId: number): void {
    this.classService.getAvailableStudents(classId).subscribe({
      next: (students) => {
        this.availableStudents = students;
      },
      error: (err) => {
        console.error('Error loading available students', err);
        this.error = 'Failed to load available students. Please try again.';
      }
    });
  }

  /**
   * @brief Enrolls a student in the selected class
   * 
   * @param studentId ID of the student to enroll
   */
  addStudentToClass(studentId: number): void {
    if (!this.selectedClassId) return;

    this.classService.addStudentToClass(this.selectedClassId, studentId).subscribe({
      next: () => {
        this.loadStudentsByClass(this.selectedClassId!);
        this.loadAvailableStudents(this.selectedClassId!);
      },
      error: (err) => {
        console.error('Error adding student to class', err);
        this.error = 'Failed to add student to class. Please try again.';
      }
    });
  }

  /**
   * @brief Removes a student from the selected class after confirmation
   * 
   * @param studentId ID of the student to remove
   */
  removeStudentFromClass(studentId: number): void {
    if (!this.selectedClassId) return;

    if (confirm('Are you sure you want to remove this student from the class?')) {
      this.classService.removeStudentFromClass(this.selectedClassId, studentId).subscribe({
        next: () => {
          this.loadStudentsByClass(this.selectedClassId!);
          this.loadAvailableStudents(this.selectedClassId!);
        },
        error: (err) => {
          console.error('Error removing student from class', err);
          this.error = 'Failed to remove student from class. Please try again.';
        }
      });
    }
  }
}