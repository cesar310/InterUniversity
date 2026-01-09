import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { Token } from '../services/token';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(Token);
  const tokenValue = token.getToken();

  if (tokenValue) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${tokenValue}`
      }
    });
  }

  return next(req);
};