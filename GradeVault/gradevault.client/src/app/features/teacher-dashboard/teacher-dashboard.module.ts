import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { TeacherDashboardComponent } from './teacher-dashboard.component';

const routes: Routes = [
  { path: '', component: TeacherDashboardComponent }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    TeacherDashboardComponent
  ]
})
export class TeacherDashboardModule { }
