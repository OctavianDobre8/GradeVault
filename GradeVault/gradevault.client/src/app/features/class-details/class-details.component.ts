import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ClassService } from '../../core/services/class.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-class-details',
  templateUrl: './class-details.component.html',
  styleUrls: ['./class-details.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class ClassDetailsComponent implements OnInit {
  classId!: string;
  classDetails: any = {};
  loading = true;
  error = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private classService: ClassService
  ) {}

  ngOnInit(): void {
    this.classId = this.route.snapshot.paramMap.get('id') || '';
    if (this.classId) {
      this.loadClassDetails();
    } else {
      this.error = true;
      this.loading = false;
    }
  }

  loadClassDetails(): void {
    this.classService.getClass(this.classId).subscribe({
      next: (data) => {
        this.classDetails = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading class details:', error);
        this.error = true;
        this.loading = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/teacher']);
  }
}