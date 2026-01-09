import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { ChangePassword } from './features/auth/change-password/change-password';
import { SelfRegister } from './features/auth/self-register/self-register';
import { ForgotPassword } from './features/auth/forgot-password/forgot-password';
import { VerifyEmail } from './features/auth/verify-email/verify-email';
import { ResendVerification } from './features/auth/resend-verification/resend-verification';
import { AdminLayout } from './features/admin/layout/admin-layout/admin-layout';
import { Dashboard } from './features/admin/dashboard/dashboard';
import { StudentList } from './features/admin/students/student-list/student-list';
import { StudentDetail } from './features/admin/students/student-detail/student-detail';
import { ProfessorList } from './features/admin/professors/professor-list/professor-list';
import { SubjectList } from './features/admin/subjects/subject-list/subject-list';
import { ConfigManager } from './features/admin/system-config/config-manager/config-manager';
import { ConfigAudit } from './features/admin/system-config/config-audit/config-audit';
import { StudentLayout } from './features/student/layout/student-layout/student-layout';
import { MyEnrollments } from './features/student/my-enrollments/my-enrollments';
import { AvailableSubjects } from './features/student/available-subjects/available-subjects';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';
import { studentGuard } from './core/guards/student.guard';
import { roleRedirectGuard } from './core/guards/role-redirect-guard';
import { guestGuard } from './core/guards/guest-guard';
import { configLoaderGuard } from './core/guards/config-loader-guard';

export const routes: Routes = [
  // Rutas públicas de autenticación
  {
    path: 'auth',
    children: [
      { path: 'login', component: Login, canActivate: [guestGuard] },
      { path: 'register', component: SelfRegister, canActivate: [guestGuard] },
      { path: 'forgot-password', component: ForgotPassword, canActivate: [guestGuard] },
      { path: 'verify-email', component: VerifyEmail },
      { path: 'resend-verification', component: ResendVerification },
      { path: 'change-password', component: ChangePassword, canActivate: [authGuard] },
      { path: '', redirectTo: 'login', pathMatch: 'full' }
    ]
  },
  // Rutas de administración
  {
    path: 'admin',
    component: AdminLayout,
    canActivate: [authGuard, adminGuard, configLoaderGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: Dashboard },
      { path: 'students', component: StudentList },
      { path: 'students/:id', component: StudentDetail },
      { path: 'professors', component: ProfessorList },
      { path: 'subjects', component: SubjectList },
      { path: 'system-config', component: ConfigManager },
      { path: 'config-audit', component: ConfigAudit },
      // Las rutas hijas se agregarán en las siguientes tareas
    ]
  },
  // Rutas de estudiantes
  {
    path: 'student',
    component: StudentLayout,
    canActivate: [authGuard, studentGuard, configLoaderGuard],
    children: [
      { path: '', redirectTo: 'enrollments', pathMatch: 'full' },
      { path: 'enrollments', component: MyEnrollments },
      { path: 'subjects', component: AvailableSubjects },
    ]
  },
  // Ruta por defecto - redirige según el rol del usuario
  { 
    path: '', 
    canActivate: [roleRedirectGuard],
    children: []
  },
  // Ruta 404
  { path: '**', redirectTo: '/auth/login' }
];
