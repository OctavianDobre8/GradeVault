import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';  // Add RouterModule import
import { AuthService } from '../../../../core/services/auth.service';

/**
 * @brief Component for user authentication
 * 
 * This component provides a form for existing users to log into the system
 * with appropriate validation and error handling.
 */
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule]  // Add RouterModule here
})
export class LoginComponent implements OnInit {
  /**
   * @brief Form group for the login form
   */
  loginForm: FormGroup;
  
  /**
   * @brief Flag indicating whether a request is in progress
   */
  loading = false;
  
  /**
   * @brief Flag indicating whether the form has been submitted
   */
  submitted = false;
  
  /**
   * @brief URL to redirect to after successful login
   */
  returnUrl: string = '/';
  
  /**
   * @brief Error message to display when login fails
   */
  error = '';
  
  /**
   * @brief Flag indicating whether user was redirected from registration
   */
  registrationSuccess = false;

  /**
   * @brief Constructor for the login component
   * 
   * Initializes the form with validation for email and password fields
   * 
   * @param formBuilder Angular form builder service
   * @param route Activated route for accessing URL parameters
   * @param router Angular router for navigation
   * @param authService Authentication service for user login
   */
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

  /**
   * @brief Lifecycle hook that runs when the component initializes
   * 
   * Gets the return URL from query parameters and checks if user
   * was redirected from successful registration.
   */
  ngOnInit(): void {
    // Get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    
    // Check if user was redirected from successful registration
    this.registrationSuccess = this.route.snapshot.queryParams['registered'] === 'true';
  }

  /**
   * @brief Convenience getter for easy access to form fields
   * 
   * @returns The form controls
   */
  get f() { return this.loginForm.controls; }

  /**
   * @brief Handles form submission for user login
   * 
   * Validates the form and calls the auth service to authenticate the user
   * with the provided credentials. Redirects to the appropriate dashboard based
   * on user role after successful login.
   */
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