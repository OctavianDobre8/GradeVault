import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ClassService } from '../../core/services/class.service';
import { Class } from '../../shared/models/class.model';
import { Student } from '../../shared/models/student.model';

@Component({
  selector: 'app-student-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './student-management.component.html',
  styleUrl: './student-management.component.css'
})
export class StudentManagementComponent implements OnInit {
  classes: Class[] = [];
  students: Student[] = [];
  availableStudents: Student[] = [];
  selectedClassId: number | null = null;
  isLoading = false;
  error: string | null = null;

  constructor(private classService: ClassService) {}

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

  selectClass(classId: number): void {
    this.selectedClassId = classId;
    this.loadStudentsByClass(classId);
    this.loadAvailableStudents(classId);
  }

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