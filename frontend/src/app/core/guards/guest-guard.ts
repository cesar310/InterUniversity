import { inject } from '@angular/core';
import { Router, UrlTree } from '@angular/router';
import { Auth } from '../services/auth';

/**
 * Guard para rutas de invitado (login, register, etc.)
 * Si el usuario ya está autenticado, redirige según su rol
 */
export const guestGuard = (): boolean | UrlTree => {
  const auth = inject(Auth);
  const router = inject(Router);

  // Si no está autenticado, permitir acceso
  if (!auth.isAuthenticated()) {
    return true;
  }

  // Si está autenticado, redirigir según rol
  const user = auth.currentUser();
  if (user?.roles?.includes('administrator')) {
    return router.parseUrl('/admin/dashboard');
  } else if (user?.roles?.includes('student')) {
    return router.parseUrl('/student/my-enrollments');
  }

  // Si no tiene roles reconocidos, permitir acceso (logout automático)
  auth.logout();
  return true;
};
