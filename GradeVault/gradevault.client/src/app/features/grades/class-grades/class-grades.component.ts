import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Grade } from '../../../shared/models/grade.model';
import { GradesService } from '../../../core/services/grades.service';

@Component({
  selector: 'app-class-grades',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './class-grades.component.html',
  styleUrl: './class-grades.component.css',
})
export class ClassGradesComponent implements OnInit {
  classId: number | null = null;
  className: string | null = null;
  grades: Grade[] = [];
  isLoading: boolean = false;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private gradesService: GradesService
  ) {}

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

  fetchClassGrades(classId: number): void {
    this.isLoading = true;
    this.error = null;

    console.log(`Fetching grades for class ID: ${classId}`);

    this.gradesService.getGradesByClass(classId).subscribe({
      next: (data) => {
        this.grades = data;
        this.isLoading = false;

        if (data.length > 0 && data[0].className) {
          this.className = data[0].className;
        }

        console.log(`Received ${data.length} grades for class ID: ${classId}`);
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

  goBackToClasses(): void {
    this.router.navigate(['/classes']);
  }
}