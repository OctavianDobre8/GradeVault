export interface LoginRequest {
  email:string;
  password:string;
  rememberMe:boolean;
}

export interface RegisterRequest{
  email:string;
  firstName:string;
  lastName:string;
  password:string;
  confirmPassword:string;
  role:string;
}

export interface ForgotPasswordRequest {
  email:string;
}

export interface ResetPasswordRequest {
  email:string;
  token:string;
  password:string;
  confirmPassword:string;
}
