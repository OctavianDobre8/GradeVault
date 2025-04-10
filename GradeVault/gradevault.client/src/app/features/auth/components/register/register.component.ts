import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule]
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  loading = false;
  submitted = false;
  error = '';
  errors: string[] = [];  // Array to hold multiple error messages
  success = '';

  // Password validation pattern
  passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

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

  ngOnInit(): void {
  }

  // Custom validator to check that password and confirm password match
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

  // Convenience getter for easy access to form fields
  get f() { return this.registerForm.controls; }

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

