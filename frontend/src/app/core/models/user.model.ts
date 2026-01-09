export interface User {
  id: number;
  email: string;
  roles: string[];
  studentId?: number;
  mustChangePassword: boolean;
}

export interface JwtPayload {
  sub: string; // user id
  email: string;
  roles: string[];
  studentId?: number;
  exp: number;
  iat: number;
}