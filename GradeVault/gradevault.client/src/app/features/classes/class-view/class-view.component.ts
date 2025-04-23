import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-class-view',
  imports: [CommonModule],
  templateUrl: './class-view.component.html',
  styleUrl: './class-view.component.css',
})
export class ClassViewComponent implements OnInit {
  classes: any[] = [
    {
      id: 1,
      name: 'Mathematics 101',
      teacher: 'Dr. Smith',
      room: 'A-101',
      schedule: 'Mon/Wed/Fri 9:00 AM',
    },
    {
      id: 2,
      name: 'History 202',
      teacher: 'Prof. Jones',
      room: 'B-205',
      schedule: 'Tue/Thu 11:00 AM',
    },
    {
      id: 3,
      name: 'Physics Lab',
      teacher: 'Dr. Green',
      room: 'C-Lab',
      schedule: 'Wed 1:00 PM - 3:00 PM',
    },
    {
      id: 4,
      name: 'English Literature',
      teacher: 'Ms. Davis',
      room: 'A-110',
      schedule: 'Mon/Fri 2:00 PM',
    },
  ];
  isLoading: boolean = false; // Set to true when fetching data later

  constructor() {}

  ngOnInit(): void {
    // Logic to fetch real class data will go here
  }
}
