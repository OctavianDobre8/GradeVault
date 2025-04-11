import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule]
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordForm: FormGroup;
  loading = false;
  submitted = false;
  token: string = '';
  email: string = '';
  errorMessage = '';
  successMessage = '';
  tokenValidated = false;
  validatingToken = true;
  
  // Password validation pattern - same as registration
  passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

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
  get f() { return this.resetPasswordForm.controls; }

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