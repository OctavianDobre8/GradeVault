import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TeacherDashboardComponent } from './teacher-dashboard.component';
import { ClassManagementComponent } from '../class-management/class-management.component';
import { GradeManagementComponent } from '../grade-management/grade-management.component';
import { StudentManagementComponent } from '../student-management/student-management.component';

const routes: Routes = [
  {
    path: '',
    component: TeacherDashboardComponent
  },
  {
    path: 'classes',
    component: ClassManagementComponent
  },
  {
    path: 'grades',
    component: GradeManagementComponent
  },
  {
    path: 'students',
    component: StudentManagementComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TeacherDashboardRoutingModule { }