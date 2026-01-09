import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

// Validador de complejidad de contraseña
export const passwordComplexityValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const value = control.value;
  if (!value) return null;

  const hasMinLength = value.length >= 8;
  const hasUpperCase = /[A-Z]/.test(value);
  const hasLowerCase = /[a-z]/.test(value);
  const hasNumbers = /\d/.test(value);
  const hasSpecialChars = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(value);

  const isValid = hasMinLength && hasUpperCase && hasLowerCase && hasNumbers && hasSpecialChars;

  return isValid ? null : {
    passwordComplexity: {
      hasMinLength,
      hasUpperCase,
      hasLowerCase,
      hasNumbers,
      hasSpecialChars
    }
  };
};

// NOTA: El validador async de email único fue removido porque el endpoint
// /auth/check-email NO existe en la API. La validación de email duplicado
// se maneja mediante el error 409 (Conflict) retornado por el backend
// al intentar registrar un email que ya existe.