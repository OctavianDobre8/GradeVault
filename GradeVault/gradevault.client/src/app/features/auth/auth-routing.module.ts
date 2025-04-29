import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component'; 
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { NoAuthGuard } from '../../core/guards/no-auth.guard';

/**
 * @brief Routes configuration for authentication feature
 * 
 * Defines all routes related to authentication functionality including
 * login, registration, password recovery, and password reset pages.
 * All routes are protected by the NoAuthGuard to prevent authenticated
 * users from accessing these pages.
 */
const routes: Routes = [
  {
    path: '',
    canActivate: [NoAuthGuard],
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'forgot-password', component: ForgotPasswordComponent },
      { path: 'reset-password', component: ResetPasswordComponent },
      { path: '', redirectTo: 'login', pathMatch: 'full' }
    ]
  }
];

/**
 * @brief Routing module for authentication features
 * 
 * This module configures and provides routes for auth-related pages,
 * using the NoAuthGuard to prevent authenticated users from accessing
 * pages they don't need when already logged in.
 */
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule { }