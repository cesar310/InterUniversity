import { inject } from '@angular/core';
import { Router, UrlTree } from '@angular/router';
import { Auth } from '../services/auth';

export const roleRedirectGuard = (): boolean | UrlTree => {
  const auth = inject(Auth);
  const router = inject(Router);

  // Si no está autenticado, redirigir al login
  if (!auth.isAuthenticated()) {
    return router.parseUrl('/auth/login');
  }

  // Redirigir según el rol del usuario
  const user = auth.currentUser();
  if (user?.roles?.includes('administrator')) {
    return router.parseUrl('/admin/dashboard');
  } else if (user?.roles?.includes('student')) {
    return router.parseUrl('/student/my-enrollments');
  }

  // Si no tiene roles reconocidos, redirigir al login
  return router.parseUrl('/auth/login');
};
