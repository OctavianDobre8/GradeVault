import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';  // Add RouterModule import
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule]  // Add RouterModule here
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string = '/';
  error = '';
  registrationSuccess = false;  // Add this property

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      rememberMe: [false]
    });
  }

  ngOnInit(): void {
    // Get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    
    // Check if user was redirected from successful registration
    this.registrationSuccess = this.route.snapshot.queryParams['registered'] === 'true';
  }


  // Convenience getter for easy access to form fields
  get f() { return this.loginForm.controls; }

  onSubmit(): void {
    this.submitted = true;

    // Stop if form is invalid
    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;
    this.authService.login({
      email: this.f['email'].value,
      password: this.f['password'].value,
      rememberMe: this.f['rememberMe'].value
    }).subscribe({
      next: () => {
        const user = this.authService.currentUserValue;
        if (user) {
          if (user.role === 'Teacher') {
            this.router.navigate(['/teacher']);
          } else {
            this.router.navigate(['/student']);
          }
        }
      },
      error: error => {
        this.error = error?.error || 'Login failed';
        this.loading = false;
      }
    });
  }
}
