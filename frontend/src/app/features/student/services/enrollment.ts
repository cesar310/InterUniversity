import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { MyEnrollment, EnrollRequest } from '../../../core/models/enrollment.model';
import { AcademicOffer } from '../../../core/models/subject.model';
import { Auth } from '../../../core/services/auth';

export interface MyEnrollmentsResponse {
  studentId: number;
  studentName: string;
  enrollments: MyEnrollment[];
  totalCredits: number;
  activeEnrollments: number;
  maxAllowed: number;
}

export interface ClassmateResponse {
  subjectName: string;
  studentName: string;
}

export interface EnrollmentValidation {
  canEnroll: boolean;
  reason?: string;
}

@Injectable({
  providedIn: 'root',
})
export class Enrollment {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(Auth);

  readonly myEnrollmentsData = signal<MyEnrollmentsResponse | null>(null);
  readonly loading = signal<boolean>(false);

  // Signal computado para total de inscripciones activas
  readonly activeEnrollmentsCount = computed(() => {
    return this.myEnrollmentsData()?.activeEnrollments ?? 0;
  });

  // Signal computado para total de créditos
  readonly totalCredits = computed(() => {
    return this.myEnrollmentsData()?.totalCredits ?? 0;
  });

  // Signal computado para máximo permitido
  readonly maxAllowed = computed(() => {
    return this.myEnrollmentsData()?.maxAllowed ?? 3;
  });

  // Signal computado para lista de inscripciones
  readonly myEnrollments = computed(() => {
    return this.myEnrollmentsData()?.enrollments ?? [];
  });

  /**
   * Obtiene las inscripciones del estudiante autenticado
   */
  getMyEnrollments(): Observable<MyEnrollmentsResponse> {
    this.loading.set(true);
    return this.http.get<{ success: boolean; data: MyEnrollmentsResponse; message: string | null }>(
      `${environment.apiUrl}/enrollments/me`
    ).pipe(
      map(response => response.data),
      tap({
        next: (data) => {
          this.myEnrollmentsData.set(data);
          this.loading.set(false);
        },
        error: () => {
          this.loading.set(false);
        }
      })
    );
  }

  /**
   * Inscribe al estudiante en una materia
   */
  enroll(subjectId: number): Observable<MyEnrollment> {
    this.loading.set(true);
    const studentId = this.auth.currentUser()?.studentId;
    const request: EnrollRequest = {
      subjectId,
      ...(studentId && { studentId })
    };
    return this.http.post<{ success: boolean; data: MyEnrollment; message: string | null }>(
      `${environment.apiUrl}/enrollments`,
      request
    ).pipe(
      map(response => response.data),
      tap({
        next: () => {
          this.loading.set(false);
          // Recargar inscripciones después de inscribirse
          this.getMyEnrollments().subscribe();
        },
        error: () => {
          this.loading.set(false);
        }
      })
    );
  }

  /**
   * Cancela una inscripción
   */
  cancel(studentId: number, subjectId: number): Observable<void> {
    this.loading.set(true);
    return this.http.delete<void>(
      `${environment.apiUrl}/enrollments/${studentId}/${subjectId}`
    ).pipe(
      tap({
        next: () => {
          this.loading.set(false);
          // Recargar inscripciones después de cancelar
          this.getMyEnrollments().subscribe();
        },
        error: () => {
          this.loading.set(false);
        }
      })
    );
  }

  /**
   * Obtiene los compañeros de clase en una materia
   */
  getClassmates(subjectId: number): Observable<ClassmateResponse[]> {
    this.loading.set(true);
    return this.http.get<{ success: boolean; data: ClassmateResponse[]; message: string | null }>(
      `${environment.apiUrl}/enrollments/classmates/${subjectId}`
    ).pipe(
      map(response => response.data),
      tap({
        next: () => this.loading.set(false),
        error: () => this.loading.set(false)
      })
    );
  }

  /**
   * Valida si un estudiante puede inscribirse en una materia
   * Implementa todas las reglas de negocio del sistema
   */
  canEnrollInSubject(subject: AcademicOffer): EnrollmentValidation {
    const enrollmentsData = this.myEnrollmentsData();

    if (!enrollmentsData) {
      return {
        canEnroll: false,
        reason: 'No se han cargado las inscripciones del estudiante'
      };
    }

    // 1. Verificar que la materia esté activa
    if (!subject.available) {
      return {
        canEnroll: false,
        reason: 'Esta materia no está disponible para inscripción'
      };
    }

    // 2. Verificar límite de materias por estudiante
    if (enrollmentsData.activeEnrollments >= enrollmentsData.maxAllowed) {
      return {
        canEnroll: false,
        reason: `Has alcanzado el límite de ${enrollmentsData.maxAllowed} materias`
      };
    }

    // 3. Verificar que no esté ya inscrito en la materia
    const alreadyEnrolled = enrollmentsData.enrollments.some(
      e => e.subjectId === subject.subjectId
    );
    if (alreadyEnrolled) {
      return {
        canEnroll: false,
        reason: 'Ya estás inscrito en esta materia'
      };
    }

    // 4. Verificar que no tenga 2 materias del mismo profesor
    const professorEnrollmentsCount = enrollmentsData.enrollments.filter(
      e => e.professorName === subject.professor
    ).length;

    if (professorEnrollmentsCount >= 2) {
      return {
        canEnroll: false,
        reason: `No puedes inscribir más de 2 materias con el profesor ${subject.professor}`
      };
    }

    // Todas las validaciones pasaron
    return {
      canEnroll: true
    };
  }

  /**
   * Método helper para verificar si el estudiante está inscrito en una materia
   */
  isEnrolledInSubject(subjectId: number): boolean {
    const enrollmentsData = this.myEnrollmentsData();
    if (!enrollmentsData) return false;

    return enrollmentsData.enrollments.some(e => e.subjectId === subjectId);
  }

  /**
   * Método helper para obtener el conteo de materias con un profesor específico
   */
  getEnrollmentCountByProfessor(professorName: string): number {
    const enrollmentsData = this.myEnrollmentsData();
    if (!enrollmentsData) return 0;

    return enrollmentsData.enrollments.filter(
      e => e.professorName === professorName
    ).length;
  }
}
