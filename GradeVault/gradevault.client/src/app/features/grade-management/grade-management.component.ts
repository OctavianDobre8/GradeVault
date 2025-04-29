import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GradesService } from '../../core/services/grades.service';
import { ClassService } from '../../core/services/class.service';
import { Grade } from '../../shared/models/grade.model';
import { Class } from '../../shared/models/class.model';
import { Student } from '../../shared/models/student.model';

/**
 * @brief Component for managing student grades
 * 
 * This component allows teachers to view, create, update, and delete grades
 * for students in their classes, as well as upload grades in bulk via CSV.
 */
@Component({
  selector: 'app-grade-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './grade-management.component.html',
  styleUrl: './grade-management.component.css'
})
export class GradeManagementComponent implements OnInit {
  /**
   * @brief List of classes taught by the teacher
   */
  classes: Class[] = [];
  
  /**
   * @brief List of students in the selected class
   */
  students: Student[] = [];
  
  /**
   * @brief List of grades in the selected class
   */
  grades: Grade[] = [];
  
  /**
   * @brief Flag indicating whether data is being loaded
   */
  isLoading = false;
  
  /**
   * @brief Error message to display when operations fail
   */
  error: string | null = null;
  
  /**
   * @brief Form group for creating or updating grades
   */
  gradeForm: FormGroup;
  
  /**
   * @brief Currently selected class ID
   */
  selectedClassId: number | null = null;
  
  /**
   * @brief Flag indicating whether we're editing an existing grade
   */
  isEditing = false;
  
  /**
   * @brief ID of the grade being edited
   */
  selectedGradeId: number | null = null;

  /**
   * @brief Constructor for the grade management component
   * 
   * Initializes the form with validation for student and grade value
   * 
   * @param gradesService Service for grade CRUD operations
   * @param classService Service for class and student data
   * @param fb Form builder for creating reactive forms
   */
  constructor(
    private gradesService: GradesService,
    private classService: ClassService,
    private fb: FormBuilder
  ) {
    this.gradeForm = this.fb.group({
      studentId: ['', [Validators.required]],
      value: ['', [Validators.required, Validators.min(1), Validators.max(10)]],
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
   * @brief Loads students enrolled in a specific class
   * 
   * @param classId ID of the class to load students for
   */
  loadStudentsByClass(classId: number): void {
    this.selectedClassId = classId;
    this.isLoading = true;
    this.classService.getStudentsByClass(classId).subscribe({
      next: (students) => {
        this.students = students;
        this.loadGradesByClass(classId);
      },
      error: (err) => {
        console.error('Error loading students', err);
        this.error = 'Failed to load students. Please try again.';
        this.isLoading = false;
      }
    });
  }

  /**
   * @brief Loads grades for a specific class
   * 
   * @param classId ID of the class to load grades for
   */
  loadGradesByClass(classId: number): void {
    this.gradesService.getGradesByClass(classId).subscribe({
      next: (grades) => {
        this.grades = grades;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading grades', err);
        this.error = 'Failed to load grades. Please try again.';
        this.isLoading = false;
      }
    });
  }

  /**
   * @brief Handles form submission for creating or updating grades
   * 
   * Validates the form and calls the appropriate service method based
   * on whether we're creating a new grade or updating an existing one.
   */
  onSubmit(): void {
    console.log("Form submitted", {
      valid: this.gradeForm.valid,
      values: this.gradeForm.value,
      selectedClassId: this.selectedClassId,
      errors: this.gradeForm.errors
    });
  
    if (this.gradeForm.invalid || !this.selectedClassId) {
      console.log("Form is invalid or no class selected");
      return;
    }
  
    const gradeData = {
      ...this.gradeForm.value,
      classId: this.selectedClassId
    };
  
    if (this.isEditing && this.selectedGradeId) {
      // Only send the value when updating
      this.gradesService.updateGrade(this.selectedGradeId, { value: Number(gradeData.value) }).subscribe({
        next: () => {
          this.loadGradesByClass(this.selectedClassId!);
          this.resetForm();
        },
        error: (err) => {
          console.error('Error updating grade', err);
          this.error = 'Failed to update grade. Please try again.';
        }
      });
    } else {
      console.log("Creating grade with:", {
        studentId: Number(gradeData.studentId),
        classId: Number(this.selectedClassId),
        value: Number(gradeData.value)
      });
      // Make sure we're sending studentId, classId, and value as numbers
      this.gradesService.createGrade({
        studentId: Number(gradeData.studentId),
        classId: Number(this.selectedClassId),
        value: Number(gradeData.value)
      }).subscribe({
        next: () => {
          this.loadGradesByClass(this.selectedClassId!);
          this.resetForm();
        },
        error: (err) => {
          console.error('Error creating grade', err);
          this.error = 'Failed to create grade. Please try again.';
        }
      });
    }
  }
  
  /**
   * @brief Sets up the form for editing an existing grade
   * 
   * @param grade The grade to edit
   */
  editGrade(grade: Grade): void {
    this.isEditing = true;
    this.selectedGradeId = grade.id;
    this.gradeForm.patchValue({
      studentId: grade.studentId,
      value: grade.value
    });
  }

  /**
   * @brief Deletes a grade after confirmation
   * 
   * @param id ID of the grade to delete
   */
  deleteGrade(id: number): void {
    if (confirm('Are you sure you want to delete this grade?')) {
      this.gradesService.deleteGrade(id).subscribe({
        next: () => {
          this.loadGradesByClass(this.selectedClassId!);
        },
        error: (err) => {
          console.error('Error deleting grade', err);
          this.error = 'Failed to delete grade. Please try again.';
        }
      });
    }
  }

  /**
   * @brief Resets the grade form and editing state
   */
  resetForm(): void {
    this.gradeForm.reset();
    this.isEditing = false;
    this.selectedGradeId = null;
  }

  /**
   * @brief Handles CSV file upload for bulk grade creation
   * 
   * @param event The file input change event
   */
  uploadBulkGrades(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0 || !this.selectedClassId) {
      return;
    }

    const file = input.files[0];
    const formData = new FormData();
    formData.append('file', file);
    formData.append('classId', this.selectedClassId.toString());

    this.gradesService.bulkUploadGrades(formData).subscribe({
      next: () => {
        this.loadGradesByClass(this.selectedClassId!);
      },
      error: (err) => {
        console.error('Error uploading grades', err);
        this.error = 'Failed to upload grades. Please try again.';
      }
    });
  }
}