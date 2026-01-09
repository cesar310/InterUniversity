import { Component, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgOptimizedImage } from '@angular/common';
import { form, Field, required, email, minLength } from '@angular/forms/signals';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { Auth } from '../../../core/services/auth';
import { Notification } from '../../../core/services/notification';
import { extractErrorMessage } from '../../../core/interceptors/error.interceptor';

@Component({
  selector: 'app-login',
  imports: [
    Field,
    FormsModule,
    NgOptimizedImage,
    ButtonModule,
    CheckboxModule,
    InputTextModule,
    IconFieldModule,
    InputIconModule,
    RouterLink
  ],
  templateUrl: './login.html',
  styleUrl: './login.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Login {
  private readonly authService = inject(Auth);
  private readonly notificationService = inject(Notification);
  private readonly router = inject(Router);

  loginModel = signal({
    email: '',
    password: '',
  });

  rememberMe = signal<boolean>(false);
  isLoading = signal<boolean>(false);
  showPassword = signal<boolean>(false);

  loginForm = form(this.loginModel, (fieldPath) => {
    required(fieldPath.email, { message: 'El correo electrónico es requerido' });
    email(fieldPath.email, { message: 'Ingresa un correo electrónico válido' });
    required(fieldPath.password, { message: 'La contraseña es requerida' });
    minLength(fieldPath.password, 6, { message: 'La contraseña debe tener al menos 6 caracteres' });
  });

  togglePasswordVisibility(): void {
    this.showPassword.update(v => !v);
  }

  onSubmit() {
    if (this.loginForm().invalid()) {
      this.notificationService.error('Por favor, corrige los errores en el formulario');
      return;
    }

    this.isLoading.set(true);
    const credentials = this.loginModel();

    this.authService.login(credentials).subscribe({
      next: (response) => {
        this.isLoading.set(false);

        // Verificar si el usuario no ha verificado su email
        if (!response.data.user.emailVerified) {
          this.notificationService.warn(
            'Debes verificar tu correo electrónico antes de iniciar sesión. Revisa tu bandeja de entrada.'
          );
          this.router.navigate(['/auth/resend-verification']);
          return;
        }

        // Verificar si el usuario debe cambiar su contraseña
        if (response.data.user.mustChangePassword) {
          this.notificationService.info('Debes cambiar tu contraseña antes de continuar');
          this.router.navigate(['/auth/change-password']);
          return;
        }

        // Redirección según el rol (usar directamente del response)
        const userRoles = response.data.user.roles;
        if (userRoles.includes('administrator')) {
          this.notificationService.success('Bienvenido, Administrador');
          this.router.navigate(['/admin/dashboard']);
        } else if (userRoles.includes('student')) {
          this.notificationService.success('Bienvenido, Estudiante');
          this.router.navigate(['/student/enrollments']);
        } else {
          this.notificationService.error('Rol de usuario no reconocido');
          this.authService.logout();
        }
      },
      error: (error) => {
        this.isLoading.set(false);
        const message = extractErrorMessage(error, 'Error al iniciar sesión');
        this.notificationService.error(message);
      }
    });
  }
}

