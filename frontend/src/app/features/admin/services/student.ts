import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Student, StudentWithEnrollments, PagedStudentsResponse, RegisterStudentRequest, RegisterStudentResponse, UpdateStudentRequest } from '../../../core/models/student.model';

@Injectable({
  providedIn: 'root',
})
export class StudentService {
  private readonly http = inject(HttpClient);

  readonly students = signal<Student[]>([]);
  readonly studentsWithEnrollments = signal<StudentWithEnrollments[]>([]);
  readonly loading = signal<boolean>(false);
  readonly pagination = signal<{ page: number; pageSize: number; totalItems: number; totalPages: number } | null>(null);

  getAll(page: number = 1, pageSize: number = 10, search?: string): Observable<PagedStudentsResponse> {
    this.loading.set(true);
    const params: any = { page, pageSize };
    if (search) params.search = search;

    return this.http.get<{ success: boolean; data: PagedStudentsResponse; message: string | null }>(`${environment.apiUrl}/students`, { params }).pipe(
      map(response => response.data),
      tap({
        next: (result) => {
          this.students.set(result.data);
          this.pagination.set(result.pagination);
          this.loading.set(false);
        },
        error: () => {
          this.loading.set(false);
        }
      })
    );
  }

  getById(id: number): Observable<Student> {
    this.loading.set(true);
    return this.http.get<{ success: boolean; data: Student; message: string | null }>(`${environment.apiUrl}/students/${id}`).pipe(
      map(response => response.data),
      tap({
        next: () => this.loading.set(false),
        error: () => this.loading.set(false)
      })
    );
  }

  getStudentsWithEnrollments(): Observable<StudentWithEnrollments[]> {
    this.loading.set(true);
    return this.http.get<{success: boolean, data: StudentWithEnrollments[], message: string | null}>(`${environment.apiUrl}/students/with-enrollments`).pipe(
      map(response => response.data),
      tap({
        next: (students) => {
          this.studentsWithEnrollments.set(students);
          this.loading.set(false);
        },
        error: () => {
          this.loading.set(false);
        }
      })
    );
  }

  create(request: RegisterStudentRequest): Observable<RegisterStudentResponse> {
    this.loading.set(true);
    return this.http.post<{ success: boolean; data: RegisterStudentResponse; message: string | null }>(`${environment.apiUrl}/auth/register`, request).pipe(
      map(response => response.data),
      tap({
        next: () => this.loading.set(false),
        error: () => this.loading.set(false)
      })
    );
  }

  update(id: number, request: UpdateStudentRequest): Observable<Student> {
    this.loading.set(true);
    // Asegurar que el ID vaya en el cuerpo del request también
    const payload = { ...request, studentId: id };
    
    return this.http.put<{ success: boolean; data: Student; message: string | null }>(`${environment.apiUrl}/students/${id}`, payload).pipe(
      map(response => response.data),
      tap({
        next: () => this.loading.set(false),
        error: () => this.loading.set(false)
      })
    );
  }

  delete(id: number): Observable<void> {
    this.loading.set(true);
    return this.http.delete<void>(`${environment.apiUrl}/students/${id}`).pipe(
      tap({
        next: () => this.loading.set(false),
        error: () => this.loading.set(false)
      })
    );
  }
}
