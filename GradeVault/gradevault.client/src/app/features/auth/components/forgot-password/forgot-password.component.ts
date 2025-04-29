import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

/**
 * @brief Component for handling password recovery requests
 * 
 * This component provides a form for users to submit their email address
 * to begin the password recovery process.
 */
@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule]
})
export class ForgotPasswordComponent {
  /**
   * @brief Form group for the password recovery form
   */
  forgotPasswordForm: FormGroup;
  
  /**
   * @brief Flag indicating whether a request is in progress
   */
  loading = false;
  
  /**
   * @brief Flag indicating whether the form has been submitted
   */
  submitted = false;
  
  /**
   * @brief Error message to display when request fails
   */
  errorMessage = '';
  
  /**
   * @brief Success message to display when request succeeds
   */
  successMessage = '';

  /**
   * @brief Constructor for the forgot password component
   * 
   * Initializes the form with validation for the email field
   * 
   * @param formBuilder Angular form builder service
   * @param router Angular router for navigation
   * @param authService Authentication service for password recovery
   */
  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private authService: AuthService
  ) {
    this.forgotPasswordForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  /**
   * @brief Convenience getter for easy access to form fields
   * 
   * @returns The form controls
   */
  get f() { return this.forgotPasswordForm.controls; }

  /**
   * @brief Handles form submission for password recovery
   * 
   * Validates the form and calls the auth service to initiate
   * the password recovery process for the provided email.
   */
  onSubmit(): void {
    this.submitted = true;
    this.errorMessage = '';
    this.successMessage = '';

    // Stop if form is invalid
    if (this.forgotPasswordForm.invalid) {
      return;
    }

    this.loading = true;
    this.authService.forgotPassword(this.f['email'].value)
      .subscribe({
        next: (response) => {
          this.loading = false;
          this.successMessage = response.message || 'If your email exists in our system, you will receive a password reset link.';
          this.forgotPasswordForm.reset();
          this.submitted = false;
        },
        error: (error) => {
          this.loading = false;
          this.errorMessage = error?.error?.message || 'An error occurred while processing your request.';
        }
      });
  }
}