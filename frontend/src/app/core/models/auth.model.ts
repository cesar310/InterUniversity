export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponseData {
  token: string;
  tokenType: string;
  expiresIn: number;
  user: UserInfo;
}

export interface LoginResponse {
  success: boolean;
  data: LoginResponseData;
  message: string;
  errorCode: string | null;
  errors: any | null;
  timestamp: string;
}

export interface UserInfo {
  id: number;
  email: string;
  roles: string[];
  studentId?: number;
  mustChangePassword: boolean;
  emailVerified: boolean;
}

export interface ChangePasswordRequest {
  userId: number;
  currentPassword: string;
  newPassword: string;
}

export interface SelfRegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface SelfRegisterResponse {
  userId: number;
  studentId: number;
  email: string;
  name: string;
  studentCode: string;
  emailSent: boolean;
  message: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ForgotPasswordResponse {
  success: boolean;
  message: string;
  emailSent: boolean;
}

export interface ResendVerificationRequest {
  email: string;
}

export interface ResendVerificationResponse {
  success: boolean;
  message: string;
  details: {
    email: string;
    expiresIn: string;
    attemptsRemaining: number;
  };
}

export interface VerifyEmailResponse {
  success: boolean;
  message: string;
  redirectUrl: string;
}

// Modelos de error del backend
export interface ApiError {
  code?: string;
  message: string;
  details?: Record<string, any>;
}

export interface ApiErrorResponse {
  error?: ApiError;
  message?: string;
  errorCode?: string;
  errors?: string[] | Record<string, string[]>;
  timestamp?: string;
  status?: number;
}