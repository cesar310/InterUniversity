import { Component, signal, computed, inject, ChangeDetectionStrategy } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgOptimizedImage } from '@angular/common';
import { form, Field, required, minLength, email, pattern } from '@angular/forms/signals';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ProgressBarModule } from 'primeng/progressbar';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { Auth } from '../../../core/services/auth';
import { Notification } from '../../../core/services/notification';
import { extractErrorMessage } from '../../../core/interceptors/error.interceptor';

@Component({
  selector: 'app-self-register',
  imports: [
    Field,
    NgOptimizedImage,
    ButtonModule,
    InputTextModule,
    ProgressBarModule,
    IconFieldModule,
    InputIconModule,
    RouterLink
  ],
  templateUrl: './self-register.html',
  styleUrl: './self-register.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SelfRegister {
  showPassword = signal<boolean>(false);
  showConfirmPassword = signal<boolean>(false);

  private readonly authService = inject(Auth);
  private readonly notificationService = inject(Notification);
  private readonly router = inject(Router);

  registerModel = signal({
    name: '',
    email: '',
    password: '',
    confirmPassword: '',
  });

  isLoading = signal<boolean>(false);
  registrationSuccess = signal<boolean>(false);

  registerForm = form(this.registerModel, (fieldPath) => {
    // Validación de nombre
    required(fieldPath.name, { message: 'El nombre es requerido' });
    minLength(fieldPath.name, 3, { message: 'El nombre debe tener al menos 3 caracteres' });

    // Validación de email
    required(fieldPath.email, { message: 'El correo electrónico es requerido' });
    email(fieldPath.email, { message: 'Ingresa un correo electrónico válido' });

    // Validación de contraseña con complejidad
    required(fieldPath.password, { message: 'La contraseña es requerida' });
    minLength(fieldPath.password, 8, { message: 'La contraseña debe tener al menos 8 caracteres' });
    pattern(fieldPath.password, /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/, 
      { message: 'Debe contener mayúsculas, minúsculas, números y símbolos' });

    // Validación de confirmación
    required(fieldPath.confirmPassword, { message: 'Confirma tu contraseña' });
  });

  // Signal computado para verificar que las contraseñas coincidan
  passwordsMatch = computed(() => {
    const model = this.registerModel();
    return model.password === model.confirmPassword;
  });

  // Signal computado para verificar si tiene errores de complejidad
  hasComplexityError = computed(() => {
    const password = this.registerModel().password;
    if (!password || password.length < 8) return false;
    
    const hasUpperCase = /[A-Z]/.test(password);
    const hasLowerCase = /[a-z]/.test(password);
    const hasNumbers = /\d/.test(password);
    const hasSpecialChars = /[@$!%*?&]/.test(password);
    
    return !(hasUpperCase && hasLowerCase && hasNumbers && hasSpecialChars);
  });

  // Signal computado para calcular la fortaleza de la contraseña
  passwordStrength = computed(() => {
    const password = this.registerModel().password;
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
    if (this.registerForm().invalid()) {
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

    this.isLoading.set(true);
    const model = this.registerModel();

    this.authService.selfRegister({
      name: model.name,
      email: model.email,
      password: model.password
    }).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        this.registrationSuccess.set(true);
        this.notificationService.success(
          `Registro exitoso. Se ha enviado un correo de verificación a ${response.email}`
        );
      },
      error: (error) => {
        this.isLoading.set(false);
        
        // Manejar error 409 (email duplicado)
        if (error.status === 409) {
          this.notificationService.error(
            'Este correo electrónico ya está registrado. Intenta iniciar sesión.'
          );
        } else {
          const message = extractErrorMessage(error, 'Error al registrar la cuenta');
          this.notificationService.error(message);
        }
      }
    });
  }

  goToLogin() {
    this.router.navigate(['/auth/login']);
  }

  togglePasswordVisibility(): void {
    this.showPassword.update(v => !v);
  }

  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword.update(v => !v);
  }
}
