import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ClassService } from '../../core/services/class.service';
import { Class } from '../../shared/models/class.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-class-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './class-management.component.html',
  styleUrl: './class-management.component.css'
})
export class ClassManagementComponent implements OnInit {
  classes: Class[] = [];
  isLoading = false;
  error: string | null = null;
  classForm: FormGroup;
  isEditing = false;
  selectedClassId: number | null = null;

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

  editClass(classItem: Class): void {
    this.isEditing = true;
    this.selectedClassId = classItem.id;
    this.classForm.patchValue({
      name: classItem.name,
      description: classItem.description,
      roomNumber: classItem.roomNumber
    });
  }

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

  resetForm(): void {
    this.classForm.reset();
    this.isEditing = false;
    this.selectedClassId = null;
  }
}