import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { Notification } from '../services/notification';
import { Auth } from '../services/auth';
import { ApiErrorResponse } from '../models/auth.model';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notification = inject(Notification);
  const auth = inject(Auth);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const errorBody = error.error as ApiErrorResponse;
      
      // No mostrar notificación si ya se maneja en el componente
      // Solo manejar casos críticos aquí
      
      if (error.status === 401 && !req.url.includes('/login')) {
        // Solo redirigir si no es la página de login
        auth.logout();
        router.navigate(['/auth/login']);
        notification.error('Sesión expirada. Por favor, inicia sesión nuevamente.');
      } else if (error.status === 500) {
        // Errores de servidor siempre se muestran
        const message = errorBody?.message || 'Error del servidor. Inténtalo de nuevo más tarde.';
        notification.error(message);
      }
      // Para otros errores (400, 403, 404, 409, 422, 429), 
      // los componentes manejarán el error específicamente

      return throwError(() => error);
    })
  );
};

/**
 * Extrae el mensaje de error más relevante de una respuesta de error del backend
 */
export function extractErrorMessage(error: any, defaultMessage: string = 'Ocurrió un error inesperado'): string {
  if (!error) return defaultMessage;
  
  const errorBody = error.error as ApiErrorResponse;
  
  // Prioridad 1: error.code con details
  if (errorBody?.error?.code) {
    const code = errorBody.error.code;
    const message = errorBody.error.message;
    const details = errorBody.error.details;
    
    // Construir mensaje con detalles
    if (details) {
      const detailsStr = Object.entries(details)
        .map(([key, value]) => `${key}: ${value}`)
        .join(', ');
      return `${message} (${detailsStr})`;
    }
    return message;
  }
  
  // Prioridad 2: message directo
  if (errorBody?.message) {
    return errorBody.message;
  }
  
  // Prioridad 3: errors array
  if (errorBody?.errors) {
    if (Array.isArray(errorBody.errors)) {
      return errorBody.errors.join(', ');
    } else if (typeof errorBody.errors === 'object') {
      // Errores de validación por campo
      return Object.entries(errorBody.errors)
        .map(([field, messages]) => {
          if (Array.isArray(messages)) {
            return `${field}: ${messages.join(', ')}`;
          }
          return `${field}: ${messages}`;
        })
        .join('; ');
    }
  }
  
  // Prioridad 4: errorCode
  if (errorBody?.errorCode) {
    return `Error: ${errorBody.errorCode}`;
  }
  
  // Fallback a mensajes por status code
  if (error.status) {
    switch (error.status) {
      case 400: return 'Solicitud inválida. Verifica los datos ingresados.';
      case 401: return 'No autorizado. Verifica tus credenciales.';
      case 403: return 'Acceso prohibido. No tienes permisos para esta acción.';
      case 404: return 'Recurso no encontrado.';
      case 409: return 'Conflicto. El recurso ya existe.';
      case 422: return 'Error de validación. Verifica los datos ingresados.';
      case 429: return 'Demasiadas solicitudes. Intenta de nuevo más tarde.';
      default: return defaultMessage;
    }
  }
  
  return defaultMessage;
}

/**
 * Extrae detalles adicionales del error para logging o debugging
 */
export function extractErrorDetails(error: any): Record<string, any> | null {
  if (!error?.error) return null;
  
  const errorBody = error.error as ApiErrorResponse;
  return errorBody?.error?.details || null;
}