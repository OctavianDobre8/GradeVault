import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

/**
 * @brief Component for password reset functionality
 * 
 * This component allows users to set a new password after clicking 
 * a reset link from their email. It validates the token from the URL
 * and then provides a form to enter a new password.
 */
@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule]
})
export class ResetPasswordComponent implements OnInit {
  /**
   * @brief Form group for the password reset form
   */
  resetPasswordForm: FormGroup;
  
  /**
   * @brief Flag indicating whether a request is in progress
   */
  loading = false;
  
  /**
   * @brief Flag indicating whether the form has been submitted
   */
  submitted = false;
  
  /**
   * @brief Token from the reset password link
   */
  token: string = '';
  
  /**
   * @brief Email address from the reset password link
   */
  email: string = '';
  
  /**
   * @brief Error message to display when request fails
   */
  errorMessage = '';
  
  /**
   * @brief Success message to display when request succeeds
   */
  successMessage = '';
  
  /**
   * @brief Flag indicating whether the token is valid
   */
  tokenValidated = false;
  
  /**
   * @brief Flag indicating token validation is in progress
   */
  validatingToken = true;
  
  /**
   * @brief Regular expression for password validation
   * 
   * Ensures password has minimum 8 characters, with at least one 
   * uppercase letter, one lowercase letter, one number, and one special character.
   */
  passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

  /**
   * @brief Constructor for the reset password component
   * 
   * Initializes the form with validation for password fields
   * 
   * @param formBuilder Angular form builder service
   * @param route Activated route for accessing URL parameters
   * @param router Angular router for navigation
   * @param authService Authentication service for password reset
   */
  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {
    this.resetPasswordForm = this.formBuilder.group({
      password: ['', [
        Validators.required, 
        Validators.minLength(8),
        Validators.pattern(this.passwordPattern)
      ]],
      confirmPassword: ['', Validators.required]
    }, {
      validator: this.passwordMatchValidator
    });
  }

  /**
   * @brief Lifecycle hook that runs when the component initializes
   * 
   * Extracts the token and email from URL parameters and validates the token
   */
  ngOnInit(): void {
    // Get token and email from query parameters
    this.email = this.route.snapshot.queryParams['email'] || '';
    this.token = this.route.snapshot.queryParams['token'] || '';
    
    if (!this.email || !this.token) {
      this.errorMessage = 'Invalid password reset link. Please request a new one.';
      this.validatingToken = false;
      return;
    }
    
    // Validate token
    this.authService.validateResetToken(this.email, this.token)
      .subscribe({
        next: () => {
          this.tokenValidated = true;
          this.validatingToken = false;
        },
        error: () => {
          this.errorMessage = 'Your password reset link is invalid or has expired. Please request a new one.';
          this.validatingToken = false;
        }
      });
  }

  /**
   * @brief Custom validator to check that password and confirm password match
   * 
   * @param formGroup The form group containing password fields
   * @returns null if valid, error object if passwords don't match
   */
  passwordMatchValidator(formGroup: FormGroup) {
    const password = formGroup.get('password')?.value;
    const confirmPassword = formGroup.get('confirmPassword')?.value;
    
    if (password && confirmPassword && password !== confirmPassword) {
      formGroup.get('confirmPassword')?.setErrors({ mismatch: true });
      return { mismatch: true };
    } else {
      return null;
    }
  }

  /**
   * @brief Convenience getter for easy access to form fields
   * 
   * @returns The form controls
   */
  get f() { return this.resetPasswordForm.controls; }

  /**
   * @brief Handles form submission for password reset
   * 
   * Validates the form and calls the auth service to reset the password
   * with the provided email, token, and new password.
   */
  onSubmit(): void {
    this.submitted = true;
    this.errorMessage = '';
    this.successMessage = '';

    // Stop if form is invalid
    if (this.resetPasswordForm.invalid) {
      return;
    }

    this.loading = true;
    this.authService.resetPassword({
      email: this.email,
      token: this.token,
      password: this.f['password'].value,
      confirmPassword: this.f['confirmPassword'].value
    }).subscribe({
      next: (response) => {
        this.loading = false;
        this.successMessage = response.message || 'Your password has been reset successfully.';
        setTimeout(() => {
          this.router.navigate(['/auth/login']);
        }, 3000);
      },
      error: (error) => {
        this.loading = false;
        if (error.error && error.error.errors) {
          this.errorMessage = Array.isArray(error.error.errors) 
            ? error.error.errors.join(', ') 
            : 'Failed to reset password. Please try again.';
        } else {
          this.errorMessage = error.error?.message || 'Failed to reset password. Please try again.';
        }
      }
    });
  }
}