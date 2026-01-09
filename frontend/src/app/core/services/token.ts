import { Injectable } from '@angular/core';
import { JwtPayload } from '../models/user.model';

@Injectable({
  providedIn: 'root',
})
export class Token {
  private readonly TOKEN_KEY = 'auth_token';

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  decodeToken(): JwtPayload | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const payloadPart = token.split('.')[1];
      const decoded = JSON.parse(atob(payloadPart));
      
      // Mapeo robusto de claims
      const roleClaim = decoded['roles'] || 
                        decoded['role'] || 
                        decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      
      const roles = Array.isArray(roleClaim) ? roleClaim : (roleClaim ? [roleClaim] : []);

      return {
        sub: decoded.sub,
        email: decoded.email || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
        roles: roles,
        studentId: decoded.studentId || decoded.StudentId ? parseInt(decoded.studentId || decoded.StudentId) : undefined,
        exp: decoded.exp,
        iat: decoded.iat
      } as JwtPayload;
    } catch (e) {
      console.error('Error decoding token', e);
      return null;
    }
  }

  isTokenExpired(): boolean {
    const payload = this.decodeToken();
    if (!payload) return true;

    const now = Math.floor(Date.now() / 1000);
    return payload.exp < now;
  }
}
