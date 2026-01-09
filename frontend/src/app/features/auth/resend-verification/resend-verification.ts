import { Component, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { form, Field, required, email } from '@angular/forms/signals';
import { NgOptimizedImage } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { Auth } from '../../../core/services/auth';
import { Notification } from '../../../core/services/notification';
import { extractErrorMessage } from '../../../core/interceptors/error.interceptor';

@Component({
  selector: 'app-resend-verification',
  imports: [
    Field,
    FormsModule,
    NgOptimizedImage,
    ButtonModule,
    InputTextModule
  ],
  templateUrl: './resend-verification.html',
  styleUrl: './resend-verification.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ResendVerification {
  private readonly authService = inject(Auth);
  private readonly notificationService = inject(Notification);
  private readonly router = inject(Router);

  isLoading = signal<boolean>(false);
  emailSent = signal<boolean>(false);

  resendModel = signal({
    email: ''
  });

  resendForm = form(this.resendModel, (fieldPath) => {
    required(fieldPath.email, { message: 'El correo electrónico es requerido' });
    email(fieldPath.email, { message: 'Ingresa un correo electrónico válido' });
  });

  onSubmit() {
    if (this.resendForm().invalid()) {
      this.notificationService.error('Por favor, ingresa un correo electrónico válido');
      return;
    }

    const emailValue = this.resendModel().email;
    this.isLoading.set(true);

    this.authService.resendVerification({ email: emailValue }).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.emailSent.set(true);
        this.notificationService.success('Correo de verificación enviado exitosamente');
      },
      error: (error: any) => {
        this.isLoading.set(false);
        
        // Mensajes específicos según el caso
        if (error.status === 400) {
          const errorBody = error.error;
          if (errorBody?.message?.includes('already verified')) {
            this.notificationService.error('Este correo ya está verificado. Puedes iniciar sesión.');
          } else if (errorBody?.message?.includes('not found')) {
            this.notificationService.error('No se encontró una cuenta con este correo.');
          } else {
            const message = extractErrorMessage(error, 'Error al enviar el correo de verificación');
            this.notificationService.error(message);
          }
        } else if (error.status === 429) {
          this.notificationService.error('Demasiados intentos. Intenta de nuevo en 1 hora.');
        } else {
          const message = extractErrorMessage(error, 'Error al enviar el correo de verificación');
          this.notificationService.error(message);
        }
      }
    });
  }

  goToLogin() {
    this.router.navigate(['/auth/login']);
  }
}
