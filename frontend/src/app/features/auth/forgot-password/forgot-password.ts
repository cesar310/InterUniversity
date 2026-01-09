import { Component, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgOptimizedImage } from '@angular/common';
import { form, Field, required, email } from '@angular/forms/signals';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { Auth } from '../../../core/services/auth';
import { Notification } from '../../../core/services/notification';
import { extractErrorMessage } from '../../../core/interceptors/error.interceptor';

@Component({
  selector: 'app-forgot-password',
  imports: [
    Field,
    NgOptimizedImage,
    ButtonModule,
    InputTextModule,
    RouterLink
  ],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ForgotPassword {
  private readonly authService = inject(Auth);
  private readonly notificationService = inject(Notification);
  private readonly router = inject(Router);

  emailModel = signal({
    email: ''
  });

  isLoading = signal<boolean>(false);
  requestSuccess = signal<boolean>(false);

  emailForm = form(this.emailModel, (fieldPath) => {
    required(fieldPath.email, { message: 'El correo electrónico es requerido' });
    email(fieldPath.email, { message: 'Ingresa un correo electrónico válido' });
  });

  onSubmit() {
    if (this.emailForm().invalid()) {
      this.notificationService.error('Por favor, ingresa un correo electrónico válido');
      return;
    }

    this.isLoading.set(true);
    const model = this.emailModel();

    this.authService.forgotPassword({ email: model.email }).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        this.requestSuccess.set(true);
        this.notificationService.success(
          'Se ha enviado una contraseña temporal a tu correo electrónico'
        );
      },
      error: (error) => {
        this.isLoading.set(false);
        
        // Manejar error 404 (email no encontrado)
        if (error.status === 404) {
          this.notificationService.error(
            'No existe una cuenta registrada con este correo electrónico.'
          );
        } else if (error.status === 429) {
          this.notificationService.error('Demasiados intentos. Intenta de nuevo en 1 hora.');
        } else {
          const message = extractErrorMessage(error, 'Error al procesar la solicitud');
          this.notificationService.error(message);
        }
      }
    });
  }

  goToLogin() {
    this.router.navigate(['/auth/login']);
  }
}
