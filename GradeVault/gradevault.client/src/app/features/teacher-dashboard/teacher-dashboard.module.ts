import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TeacherDashboardComponent } from './teacher-dashboard.component';
import { TeacherDashboardRoutingModule } from './teacher-dashboard-routing.module';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    TeacherDashboardRoutingModule,
    TeacherDashboardComponent // Import the standalone component here
  ]
})
export class TeacherDashboardModule { }