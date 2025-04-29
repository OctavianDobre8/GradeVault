import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { ResetPasswordComponent } from './features/auth/components/reset-password/reset-password.component';

/**
 * @brief Application routes configuration
 * 
 * Defines the main routing structure for the application, including:
 * - Authentication routes (login, register, password recovery)
 * - Teacher-specific routes protected by AuthGuard with Teacher role
 * - Student-specific routes protected by AuthGuard with Student role
 * - Public routes like password reset
 * - Default route redirecting to registration
 */
const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () =>
      import('./features/auth/auth.module').then((m) => m.AuthModule),
  },
  {
    path: 'teacher',
    loadChildren: () =>
      import('./features/teacher-dashboard/teacher-dashboard.module').then(
        (m) => m.TeacherDashboardModule
      ),
    canActivate: [AuthGuard],
    data: { roles: ['Teacher'] },
  },
  {
    path: 'student',
    loadChildren: () =>
      import('./features/student-dashboard/student-dashboard.module').then(
        (m) => m.StudentDashboardModule
      ),
    canActivate: [AuthGuard],
    data: { roles: ['Student'] },
  },
  {
    path: 'grades',
    loadChildren: () =>
      import('./features/grades/grades.module').then((m) => m.GradesModule),
    canActivate: [AuthGuard],
    data: { roles: ['Student'] },
  },
  {
    path: 'classes',
    loadChildren: () =>
      import('./features/classes/classes.module').then((m) => m.ClassesModule),
    canActivate: [AuthGuard],
    data: { roles: ['Student'] },
  },
  {
    path: 'reset-password',
    component: ResetPasswordComponent,
  },
  {
    path: '',
    redirectTo: 'auth/register',
    pathMatch: 'full',
  },
];

/**
 * @brief Root routing module for the application
 * 
 * Configures the router with the application's routes and makes
 * the router directives and services available throughout the app.
 */
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}