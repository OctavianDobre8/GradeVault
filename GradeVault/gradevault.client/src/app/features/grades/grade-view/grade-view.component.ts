import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-grade-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './grade-view.component.html',
  styleUrl: './grade-view.component.css',
})
export class GradeViewComponent implements OnInit {
  grades: any[] = [
    { className: 'Mathematics 101', gradeValue: 8, dateAssigned: '2025-04-15' },
    { className: 'History 202', gradeValue: 9, dateAssigned: '2025-04-18' },
    { className: 'Physics Lab', gradeValue: 7, dateAssigned: '2025-04-20' },
  ];
  isLoading: boolean = false; // Set to true when fetching data later

  constructor() {}

  ngOnInit(): void {
    // Logic to fetch real grade data will go here
  }
}
