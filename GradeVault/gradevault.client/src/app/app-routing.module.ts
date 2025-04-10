import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';

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
    path: 'student',
    loadChildren: () => import('./features/student-dashboard/student-dashboard.module').then(m => m.StudentDashboardModule),
    canActivate: [AuthGuard],
    data: { roles: ['Student'] }
  },
  { path: '', redirectTo: 'auth/register', pathMatch: 'full' }  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }