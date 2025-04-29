/**
 * @brief Interface for user login credentials
 * 
 * Represents the data required to authenticate a user, including
 * the remember me option for persistent login.
 */
export interface LoginRequest {
  email:string;
  password:string;
  rememberMe:boolean;
}

/**
 * @brief Interface for new user registration data
 * 
 * Contains all required fields for creating a new user account,
 * including personal information and role selection.
 */
export interface RegisterRequest{
  email:string;
  firstName:string;
  lastName:string;
  password:string;
  confirmPassword:string;
  role:string;
}

/**
 * @brief Interface for password recovery request
 * 
 * Contains the email address for initiating password recovery.
 */
export interface ForgotPasswordRequest {
  email:string;
}

/**
 * @brief Interface for password reset request
 * 
 * Contains all required fields to reset a user's password,
 * including the reset token, email, and new password.
 */
export interface ResetPasswordRequest {
  email:string;
  token:string;
  password:string;
  confirmPassword:string;
}