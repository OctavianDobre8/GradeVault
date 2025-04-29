import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

/**
 * @brief Component for user registration
 * 
 * This component provides a form for new users to create an account
 * with validation for all required fields and appropriate error handling.
 */
@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule]
})
export class RegisterComponent implements OnInit {
  /**
   * @brief Form group for the registration form
   */
  registerForm: FormGroup;
  
  /**
   * @brief Flag indicating whether a request is in progress
   */
  loading = false;
  
  /**
   * @brief Flag indicating whether the form has been submitted
   */
  submitted = false;
  
  /**
   * @brief Main error message to display when request fails
   */
  error = '';
  
  /**
   * @brief Array of validation error messages from the server
   */
  errors: string[] = [];  // Array to hold multiple error messages
  
  /**
   * @brief Success message to display when registration succeeds
   */
  success = '';

  /**
   * @brief Regular expression for password validation
   * 
   * Ensures password has minimum 8 characters, with at least one 
   * uppercase letter, one lowercase letter, one number, and one special character.
   */
  passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

  /**
   * @brief Constructor for the registration component
   * 
   * Initializes the form with validation for all required fields
   * 
   * @param formBuilder Angular form builder service
   * @param router Angular router for navigation
   * @param authService Authentication service for user registration
   */
  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private authService: AuthService
  ) {
    this.registerForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.minLength(4), Validators.pattern(/^[a-zA-Z]+$/)]],
      lastName: ['', [Validators.required, Validators.minLength(4), Validators.pattern(/^[a-zA-Z]+$/)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [
        Validators.required, 
        Validators.minLength(8), 
        Validators.pattern(this.passwordPattern)
      ]],
      confirmPassword: ['', Validators.required],
      role: ['Student', Validators.required]
    }, {
      validator: this.passwordMatchValidator
    });
  }

  /**
   * @brief Lifecycle hook that runs when the component initializes
   */
  ngOnInit(): void {
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
  get f() { return this.registerForm.controls; }

  /**
   * @brief Handles form submission for user registration
   * 
   * Validates the form and calls the auth service to register a new user
   * with the provided information.
   */
  onSubmit(): void {
    this.submitted = true;
    this.error = '';
    this.errors = [];
    this.success = '';

    // Stop if form is invalid
    if (this.registerForm.invalid) {
      return;
    }

    this.loading = true;
    this.authService.register({
      firstName: this.f['firstName'].value,
      lastName: this.f['lastName'].value,
      email: this.f['email'].value,
      password: this.f['password'].value,
      confirmPassword: this.f['confirmPassword'].value,
      role: this.f['role'].value,
    }).subscribe({
      next: () => {
        this.success = 'Registration successful! Redirecting to login...';
        setTimeout(() => {
          this.router.navigate(['/auth/login'], { queryParams: { registered: true }});
        }, 2000);
      },
      error: error => {
        this.loading = false;
        
        if (error.error && typeof error.error === 'object') {
          // Handle validation errors from the server
          if (error.error.errors) {
            // Model validation errors
            Object.keys(error.error.errors).forEach(key => {
              this.errors.push(...error.error.errors[key]);
            });
          } else if (error.error.length) {
            // Array of error messages
            this.errors = Array.isArray(error.error) ? error.error : [error.error];
          } else if (error.error.message) {
            // Single error with message property
            this.error = error.error.message;
          } else {
            // Other error format with direct properties
            for (const key in error.error) {
              if (typeof error.error[key] === 'string') {
                this.errors.push(error.error[key]);
              } else if (Array.isArray(error.error[key])) {
                this.errors.push(...error.error[key]);
              }
            }
          }
        } else if (typeof error.error === 'string') {
          // Direct error message as string
          this.error = error.error;
        } else {
          // Fallback error message
          this.error = 'Registration failed. Please check your details and try again.';
        }
        
        // Log the error for debugging
        console.error('Registration error:', error);
      }
    });
  }
}