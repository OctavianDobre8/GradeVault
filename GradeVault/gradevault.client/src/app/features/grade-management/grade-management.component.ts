
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GradesService } from '../../core/services/grades.service';
import { ClassService } from '../../core/services/class.service';
import { Grade } from '../../shared/models/grade.model';
import { Class } from '../../shared/models/class.model';
import { Student } from '../../shared/models/student.model';

@Component({
  selector: 'app-grade-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './grade-management.component.html',
  styleUrl: './grade-management.component.css'
})
export class GradeManagementComponent implements OnInit {
  classes: Class[] = [];
  students: Student[] = [];
  grades: Grade[] = [];
  isLoading = false;
  error: string | null = null;
  gradeForm: FormGroup;
  selectedClassId: number | null = null;
  isEditing = false;
  selectedGradeId: number | null = null;

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

  ngOnInit(): void {
    this.loadClasses();
  }

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
  
  editGrade(grade: Grade): void {
    this.isEditing = true;
    this.selectedGradeId = grade.id;
    this.gradeForm.patchValue({
      studentId: grade.studentId,
      value: grade.value
    });
  }

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

  resetForm(): void {
    this.gradeForm.reset();
    this.isEditing = false;
    this.selectedGradeId = null;
  }

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