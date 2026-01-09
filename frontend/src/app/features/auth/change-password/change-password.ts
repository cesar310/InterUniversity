import { Component, signal, computed, inject, ChangeDetectionStrategy } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgOptimizedImage } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { form, Field, required, minLength, pattern } from '@angular/forms/signals';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import { ProgressBarModule } from 'primeng/progressbar';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { Auth } from '../../../core/services/auth';
import { Notification } from '../../../core/services/notification';
import { extractErrorMessage } from '../../../core/interceptors/error.interceptor';

@Component({
  selector: 'app-change-password',
  imports: [
    Field,
    FormsModule,
    NgOptimizedImage,
    ButtonModule,
    InputTextModule,
    CardModule,
    ProgressBarModule,
    IconFieldModule,
    InputIconModule,
    RouterLink
  ],
  templateUrl: './change-password.html',
  styleUrl: './change-password.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChangePassword {
  showCurrentPassword = signal<boolean>(false);
  showNewPassword = signal<boolean>(false);
  showConfirmPassword = signal<boolean>(false);

  private readonly authService = inject(Auth);
  private readonly notificationService = inject(Notification);
  private readonly router = inject(Router);

  passwordModel = signal({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });

  isLoading = signal<boolean>(false);

  passwordForm = form(this.passwordModel, (fieldPath) => {
    // Validación de contraseña actual
    required(fieldPath.currentPassword, { message: 'La contraseña actual es requerida' });
    minLength(fieldPath.currentPassword, 6, { message: 'Mínimo 6 caracteres' });

    // Validación de nueva contraseña
    required(fieldPath.newPassword, { message: 'La nueva contraseña es requerida' });
    minLength(fieldPath.newPassword, 8, { message: 'La contraseña debe tener al menos 8 caracteres' });
    pattern(fieldPath.newPassword, /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/, 
      { message: 'Debe contener mayúsculas, minúsculas, números y símbolos' });

    // Validación de confirmación
    required(fieldPath.confirmPassword, { message: 'Confirma tu nueva contraseña' });
  });

  // Signal computado para verificar que las contraseñas coincidan
  passwordsMatch = computed(() => {
    const model = this.passwordModel();
    return model.newPassword === model.confirmPassword;
  });

  // Signal computado para verificar que la nueva contraseña sea diferente de la actual
  passwordsDifferent = computed(() => {
    const model = this.passwordModel();
    if (!model.currentPassword || !model.newPassword) return true;
    return model.newPassword !== model.currentPassword;
  });

  // Signal computado para verificar si el formulario tiene errores de complejidad
  hasComplexityError = computed(() => {
    const password = this.passwordModel().newPassword;
    if (!password || password.length < 8) return false;
    
    const hasUpperCase = /[A-Z]/.test(password);
    const hasLowerCase = /[a-z]/.test(password);
    const hasNumbers = /\d/.test(password);
    const hasSpecialChars = /[@$!%*?&]/.test(password);
    
    return !(hasUpperCase && hasLowerCase && hasNumbers && hasSpecialChars);
  });

  // Signal computado para calcular la fortaleza de la contraseña
  passwordStrength = computed(() => {
    const password = this.passwordModel().newPassword;
    if (!password) return { value: 0, label: '', color: '' };

    let strength = 0;
    const checks = {
      length: password.length >= 8,
      uppercase: /[A-Z]/.test(password),
      lowercase: /[a-z]/.test(password),
      numbers: /\d/.test(password),
      special: /[@$!%*?&]/.test(password),
    };

    if (checks.length) strength += 20;
    if (checks.uppercase) strength += 20;
    if (checks.lowercase) strength += 20;
    if (checks.numbers) strength += 20;
    if (checks.special) strength += 20;

    if (strength < 60) {
      return { value: strength, label: 'Débil', color: 'danger' };
    } else if (strength < 100) {
      return { value: strength, label: 'Media', color: 'warn' };
    } else {
      return { value: strength, label: 'Fuerte', color: 'success' };
    }
  });

  onSubmit() {
    // Validar formulario
    if (this.passwordForm().invalid()) {
      this.notificationService.error('Por favor, corrige los errores en el formulario');
      return;
    }

    // Validar complejidad
    if (this.hasComplexityError()) {
      this.notificationService.error('La contraseña no cumple con los requisitos de complejidad');
      return;
    }

    // Validar que las contraseñas coincidan
    if (!this.passwordsMatch()) {
      this.notificationService.error('Las contraseñas no coinciden');
      return;
    }

    // Validar que la nueva contraseña sea diferente
    if (!this.passwordsDifferent()) {
      this.notificationService.error('La nueva contraseña debe ser diferente a la actual');
      return;
    }

    const currentUser = this.authService.currentUser();
    if (!currentUser) {
      this.notificationService.error('No hay un usuario autenticado');
      this.router.navigate(['/auth/login']);
      return;
    }

    this.isLoading.set(true);
    const model = this.passwordModel();

    this.authService.changePassword({
      userId: currentUser.id,
      currentPassword: model.currentPassword,
      newPassword: model.newPassword
    }).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        this.notificationService.success('Contraseña actualizada exitosamente');
        
        // Redireccionar según el rol
        if (this.authService.isAdmin()) {
          this.router.navigate(['/admin/dashboard']);
        } else if (this.authService.isStudent()) {
          this.router.navigate(['/student/my-enrollments']);
        } else {
          this.router.navigate(['/auth/login']);
        }
      },
      error: (error) => {
        this.isLoading.set(false);
        
        // Mensaje específico para contraseña actual incorrecta
        if (error.status === 401) {
          this.notificationService.error('La contraseña actual es incorrecta.');
        } else {
          const message = extractErrorMessage(error, 'Error al cambiar la contraseña');
          this.notificationService.error(message);
        }
      }
    });
  }

  toggleCurrentPasswordVisibility(): void {
    this.showCurrentPassword.update(v => !v);
  }

  toggleNewPasswordVisibility(): void {
    this.showNewPassword.update(v => !v);
  }

  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword.update(v => !v);
  }
}
