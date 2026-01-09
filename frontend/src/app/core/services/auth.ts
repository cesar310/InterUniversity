import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { 
  LoginRequest, 
  LoginResponse, 
  ChangePasswordRequest,
  SelfRegisterRequest,
  SelfRegisterResponse,
  ForgotPasswordRequest,
  ForgotPasswordResponse,
  ResendVerificationRequest,
  ResendVerificationResponse,
  VerifyEmailResponse
} from '../models/auth.model';
import { User } from '../models/user.model';
import { Token } from './token';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private readonly http = inject(HttpClient);
  private readonly tokenService = inject(Token);

  private readonly currentUserSignal = signal<User | null>(null);
  readonly currentUser = this.currentUserSignal.asReadonly();

  readonly isAuthenticated = computed(() => !!this.currentUser());
  readonly isAdmin = computed(() => this.currentUser()?.roles?.includes('administrator') ?? false);
  readonly isStudent = computed(() => this.currentUser()?.roles?.includes('student') ?? false);

  constructor() {
    this.loadUserFromToken();
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, request).pipe(
      tap(response => {
        this.tokenService.setToken(response.data.token);
        this.currentUserSignal.set({
          id: response.data.user.id,
          email: response.data.user.email,
          roles: response.data.user.roles,
          studentId: response.data.user.studentId,
          mustChangePassword: response.data.user.mustChangePassword
        });
      })
    );
  }

  logout(): void {
    this.tokenService.removeToken();
    this.currentUserSignal.set(null);
  }

  changePassword(request: ChangePasswordRequest): Observable<{ success: boolean; message: string }> {
    return this.http.post<{ success: boolean; message: string }>(
      `${environment.apiUrl}/auth/change-password`, 
      request
    );
  }

  selfRegister(request: SelfRegisterRequest): Observable<SelfRegisterResponse> {
    return this.http.post<SelfRegisterResponse>(
      `${environment.apiUrl}/auth/self-register`,
      request
    );
  }

  forgotPassword(request: ForgotPasswordRequest): Observable<ForgotPasswordResponse> {
    return this.http.post<ForgotPasswordResponse>(
      `${environment.apiUrl}/auth/forgot-password`,
      request
    );
  }

  resendVerification(request: ResendVerificationRequest): Observable<ResendVerificationResponse> {
    return this.http.post<ResendVerificationResponse>(
      `${environment.apiUrl}/auth/email-verifications`,
      request
    );
  }

  verifyEmail(token: string): Observable<VerifyEmailResponse> {
    return this.http.get<VerifyEmailResponse>(
      `${environment.apiUrl}/auth/verify-email?token=${encodeURIComponent(token)}`
    );
  }

  private loadUserFromToken(): void {
    if (this.tokenService.isTokenExpired()) {
      this.logout();
      return;
    }

    const payload = this.tokenService.decodeToken();
    if (payload) {
      this.currentUserSignal.set({
        id: parseInt(payload.sub),
        email: payload.email,
        roles: payload.roles,
        studentId: payload.studentId,
        mustChangePassword: false // Asumir false, o manejar de otra forma
      });
    }
  }
}
