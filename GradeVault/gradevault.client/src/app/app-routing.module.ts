import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { ResetPasswordComponent } from './features/auth/components/reset-password/reset-password.component';
import { ClassDetailsModule } from './features/class-details/class-details.module';

const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: 'teacher',
    loadChildren: () => import('./features/teacher-dashboard/teacher-dashboard.module').then(m => m.TeacherDashboardModule),
    canActivate: [AuthGuard],
    data: { roles: ['Teacher'] }
  },
  {
    path: 'teacher/classes/:id',
    loadChildren: () => import('./features/class-details/class-details.module').then(m => m.ClassDetailsModule),
    canActivate: [AuthGuard],
    data: { roles: ['Teacher'] }
  },
  {
    path: 'student',
    loadChildren: () => import('./features/student-dashboard/student-dashboard.module').then(m => m.StudentDashboardModule),
    canActivate: [AuthGuard],
    data: { roles: ['Student'] }
  },
  {
    path: 'reset-password',
    component: ResetPasswordComponent
  },
  { path: '', redirectTo: 'auth/register', pathMatch: 'full' }  
];

@NgModule({
  imports: [RouterModule.forRoot(routes), ClassDetailsModule],
  exports: [RouterModule]
})
export class AppRoutingModule { }