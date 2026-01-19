import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, map } from 'rxjs';
import { environment } from '../../../../environments/environment';

export interface AdminEnrollment {
  studentId: number;
  subjectId: number;
  studentCode: string;
  studentEmail: string;
  subjectName: string;
  professorName: string;
  status: 'Active' | 'Completed' | 'Cancelled';
  enrolledAt: string;
  updatedAt: string;
}

export interface PagedEnrollmentsResponse {
  enrollments: AdminEnrollment[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

@Injectable({
  providedIn: 'root',
})
export class AdminEnrollmentService {
  private readonly http = inject(HttpClient);

  readonly enrollments = signal<AdminEnrollment[]>([]);
  readonly loading = signal<boolean>(false);

  /**
   * Obtiene todas las inscripciones con filtros opcionales
   */
  getAll(
    page: number = 1,
    pageSize: number = 10,
    studentId?: number,
    subjectId?: number,
    status?: string
  ): Observable<PagedEnrollmentsResponse> {
    this.loading.set(true);
    const params: any = { page, pageSize };
    if (studentId) params.studentId = studentId;
    if (subjectId) params.subjectId = subjectId;
    if (status) params.status = status;

    return this.http
      .get<{ success: boolean; data: PagedEnrollmentsResponse; message: string | null }>(
        `${environment.apiUrl}/enrollments`,
        { params }
      )
      .pipe(
        map((response) => response.data),
        tap({
          next: (result) => {
            this.enrollments.set(result.enrollments);
            this.loading.set(false);
          },
          error: () => {
            this.loading.set(false);
          },
        })
      );
  }

  /**
   * Obtiene las inscripciones de un estudiante específico
   */
  getByStudentId(studentId: number): Observable<AdminEnrollment[]> {
    this.loading.set(true);
    return this.http
      .get<{ success: boolean; data: PagedEnrollmentsResponse; message: string | null }>(
        `${environment.apiUrl}/enrollments`,
        { params: { studentId, pageSize: 100 } }
      )
      .pipe(
        map((response) => response.data.enrollments),
        tap({
          next: (enrollments) => {
            this.enrollments.set(enrollments);
            this.loading.set(false);
          },
          error: () => {
            this.loading.set(false);
          },
        })
      );
  }

  /**
   * Cancela una inscripción (administrador)
   */
  cancel(studentId: number, subjectId: number): Observable<void> {
    this.loading.set(true);
    return this.http
      .delete<void>(`${environment.apiUrl}/enrollments/${studentId}/${subjectId}`)
      .pipe(
        tap({
          next: () => {
            this.loading.set(false);
          },
          error: () => {
            this.loading.set(false);
          },
        })
      );
  }
}
