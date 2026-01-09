import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, tap, finalize, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { 
  Subject, 
  CreateSubjectRequest, 
  UpdateSubjectRequest, 
  PagedSubjectsResponse,
  AcademicOffer 
} from '../../../core/models/subject.model';

@Injectable({
  providedIn: 'root',
})
export class SubjectService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/subjects`;

  readonly subjects = signal<Subject[]>([]);
  readonly academicOffer = signal<AcademicOffer[]>([]);
  readonly loading = signal<boolean>(false);
  readonly pagination = signal<{ page: number; pageSize: number; totalItems: number; totalPages: number } | null>(null);

  getAll(page: number = 1, pageSize: number = 10, search?: string, isActive?: boolean, professorId?: number): Observable<PagedSubjectsResponse> {
    this.loading.set(true);
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (search) params = params.set('search', search);
    if (isActive !== undefined) params = params.set('isActive', isActive.toString());
    if (professorId) params = params.set('professorId', professorId.toString());

    return this.http.get<{ success: boolean; data: PagedSubjectsResponse; message: string | null }>(this.apiUrl, { params }).pipe(
      map(response => response.data),
      tap({
        next: (response) => {
          this.subjects.set(response.subjects);
          this.pagination.set({
            page: response.page,
            pageSize: response.pageSize,
            totalItems: response.totalCount,
            totalPages: response.totalPages
          });
          this.loading.set(false);
        },
        error: () => {
          this.subjects.set([]);
          this.loading.set(false);
        }
      })
    );
  }

  getAcademicOffer(): Observable<AcademicOffer[]> {
    this.loading.set(true);
    return this.http.get<{ success: boolean; data: AcademicOffer[]; message: string | null }>(`${this.apiUrl}/academic-offer`).pipe(
      map(response => response.data),
      tap({
        next: (data) => {
          this.academicOffer.set(data);
          this.loading.set(false);
        },
        error: () => {
          this.academicOffer.set([]);
          this.loading.set(false);
        }
      })
    );
  }

  getById(id: number): Observable<Subject> {
    this.loading.set(true);
    return this.http.get<{ success: boolean; data: Subject; message: string | null }>(`${this.apiUrl}/${id}`).pipe(
      map(response => response.data),
      tap({
        next: () => this.loading.set(false),
        error: () => this.loading.set(false)
      })
    );
  }

  create(request: CreateSubjectRequest): Observable<Subject> {
    this.loading.set(true);
    return this.http.post<{ success: boolean; data: Subject; message: string | null }>(this.apiUrl, request).pipe(
      map(response => response.data),
      tap({
        next: () => this.loading.set(false),
        error: () => this.loading.set(false)
      })
    );
  }

  update(id: number, request: UpdateSubjectRequest): Observable<Subject> {
    this.loading.set(true);
    // Asegurar que el ID vaya en el cuerpo del request
    const payload = { ...request, subjectId: id };
    return this.http.put<{ success: boolean; data: Subject; message: string | null }>(`${this.apiUrl}/${id}`, payload).pipe(
      map(response => response.data),
      tap({
        next: () => this.loading.set(false),
        error: () => this.loading.set(false)
      })
    );
  }

  delete(id: number): Observable<void> {
    this.loading.set(true);
    return this.http.delete<{ success: boolean; data: void; message: string | null }>(`${this.apiUrl}/${id}`).pipe(
      map(() => void 0),
      tap({
        next: () => this.loading.set(false),
        error: () => this.loading.set(false)
      })
    );
  }
}
