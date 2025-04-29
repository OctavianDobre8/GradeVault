import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ClassService } from '../../core/services/class.service';
import { Class } from '../../shared/models/class.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

/**
 * @brief Component for managing classes by teachers
 * 
 * This component provides functionality for teachers to create, view,
 * edit, and delete their classes, with a form-based interface.
 */
@Component({
  selector: 'app-class-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './class-management.component.html',
  styleUrl: './class-management.component.css'
})
export class ClassManagementComponent implements OnInit {
  /**
   * @brief List of classes taught by the teacher
   */
  classes: Class[] = [];
  
  /**
   * @brief Flag indicating whether data is being loaded
   */
  isLoading = false;
  
  /**
   * @brief Error message to display when operations fail
   */
  error: string | null = null;
  
  /**
   * @brief Form group for creating or updating classes
   */
  classForm: FormGroup;
  
  /**
   * @brief Flag indicating whether we're editing an existing class
   */
  isEditing = false;
  
  /**
   * @brief ID of the class being edited
   */
  selectedClassId: number | null = null;

  /**
   * @brief Constructor for the class management component
   * 
   * Initializes the form with validation for class fields
   * 
   * @param classService Service for class CRUD operations
   * @param fb Form builder for creating reactive forms
   */
  constructor(
    private classService: ClassService,
    private fb: FormBuilder
  ) {
    this.classForm = this.fb.group({
      name: ['', [Validators.required]],
      description: ['', [Validators.required]],
      roomNumber: ['', [Validators.required]]
    });
  }

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
   * @brief Handles form submission for creating or updating classes
   * 
   * Validates the form and calls the appropriate service method based
   * on whether we're creating a new class or updating an existing one.
   */
  onSubmit(): void {
    if (this.classForm.invalid) {
      return;
    }

    const classData = this.classForm.value;
    
    if (this.isEditing && this.selectedClassId) {
      this.classService.updateClass(this.selectedClassId, classData).subscribe({
        next: () => {
          this.loadClasses();
          this.resetForm();
        },
        error: (err) => {
          console.error('Error updating class', err);
          this.error = 'Failed to update class. Please try again.';
        }
      });
    } else {
      this.classService.createClass(classData).subscribe({
        next: () => {
          this.loadClasses();
          this.resetForm();
        },
        error: (err) => {
          console.error('Error creating class', err);
          this.error = 'Failed to create class. Please try again.';
        }
      });
    }
  }

  /**
   * @brief Sets up the form for editing an existing class
   * 
   * @param classItem The class object to edit
   */
  editClass(classItem: Class): void {
    this.isEditing = true;
    this.selectedClassId = classItem.id;
    this.classForm.patchValue({
      name: classItem.name,
      description: classItem.description,
      roomNumber: classItem.roomNumber
    });
  }

  /**
   * @brief Deletes a class after confirmation
   * 
   * @param id ID of the class to delete
   */
  deleteClass(id: number): void {
    if (confirm('Are you sure you want to delete this class?')) {
      this.classService.deleteClass(id).subscribe({
        next: () => {
          this.loadClasses();
        },
        error: (err) => {
          console.error('Error deleting class', err);
          this.error = 'Failed to delete class. Please try again.';
        }
      });
    }
  }

  /**
   * @brief Resets the class form and editing state
   */
  resetForm(): void {
    this.classForm.reset();
    this.isEditing = false;
    this.selectedClassId = null;
  }
}