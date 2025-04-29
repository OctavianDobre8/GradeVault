import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

import { AuthRoutingModule } from './auth-routing.module';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';

/**
 * @brief Feature module for authentication-related functionality
 * 
 * This module encapsulates all components and functionality related to user
 * authentication, including login, registration, and password recovery.
 * It uses standalone components imported through the imports array.
 */
@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AuthRoutingModule,
    LoginComponent,
    RegisterComponent
  ],
  declarations: [
  ]
})
export class AuthModule { }