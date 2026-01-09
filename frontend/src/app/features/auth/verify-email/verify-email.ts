import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgOptimizedImage } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { Auth } from '../../../core/services/auth';
import { Notification } from '../../../core/services/notification';
import { extractErrorMessage } from '../../../core/interceptors/error.interceptor';

@Component({
  selector: 'app-verify-email',
  imports: [
    NgOptimizedImage,
    ButtonModule,
    ProgressSpinnerModule
  ],
  templateUrl: './verify-email.html',
  styleUrl: './verify-email.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class VerifyEmail implements OnInit {
  private readonly authService = inject(Auth);
  private readonly notificationService = inject(Notification);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  isVerifying = signal<boolean>(true);
  verificationSuccess = signal<boolean>(false);
  errorMessage = signal<string>('');

  ngOnInit() {
    // Obtener token de los query params
    this.route.queryParams.subscribe(params => {
      const token = params['token'];
      
      if (!token) {
        this.isVerifying.set(false);
        this.errorMessage.set('Token de verificación no proporcionado');
        this.notificationService.error('Token de verificación inválido');
        return;
      }

      // Llamar al servicio de verificación
      this.verifyEmail(token);
    });
  }

  verifyEmail(token: string) {
    this.authService.verifyEmail(token).subscribe({
      next: (response) => {
        this.isVerifying.set(false);
        this.verificationSuccess.set(true);
        this.notificationService.success('Email verificado exitosamente');
        
        // Redirigir al login después de 3 segundos
        setTimeout(() => {
          this.router.navigate(['/auth/login']);
        }, 3000);
      },
      error: (error) => {
        this.isVerifying.set(false);
        this.verificationSuccess.set(false);
        
        const message = extractErrorMessage(error, 'Error al verificar el email. El token puede haber expirado.');
        this.errorMessage.set(message);
        this.notificationService.error(message);
      }
    });
  }

  goToLogin() {
    this.router.navigate(['/auth/login']);
  }

  goToResendVerification() {
    this.router.navigate(['/auth/resend-verification']);
  }
}
