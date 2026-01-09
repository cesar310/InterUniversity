import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Professor, CreateProfessorRequest, UpdateProfessorRequest, PagedProfessorsResponse } from '../../../core/models/professor.model';

@Injectable({
  providedIn: 'root',
})
export class ProfessorService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/professors`;

  readonly professors = signal<Professor[]>([]);
  readonly loading = signal<boolean>(false);
  readonly pagination = signal<{ page: number; pageSize: number; totalItems: number; totalPages: number } | null>(null);

  getAll(page: number = 1, pageSize: number = 10, search?: string): Observable<PagedProfessorsResponse> {
    this.loading.set(true);
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (search) {
      params = params.set('search', search);
    }

    return this.http.get<PagedProfessorsResponse>(this.apiUrl, { params }).pipe(
      tap((response) => {
        if (response.success && response.data) {
          this.professors.set(response.data.professors);
          this.pagination.set({
             page: response.data.page,
             pageSize: response.data.pageSize,
             totalItems: response.data.totalCount,
             totalPages: response.data.totalPages
          });
        }
        this.loading.set(false);
      }),
      tap({ error: () => this.loading.set(false) })
    );
  }


  getById(id: number): Observable<Professor> {
    return this.http.get<Professor>(`${this.apiUrl}/${id}`);
  }

  create(professor: CreateProfessorRequest): Observable<Professor> {
    return this.http.post<Professor>(this.apiUrl, professor).pipe(
      tap(() => this.refreshList())
    );
  }

  update(id: number, professor: UpdateProfessorRequest): Observable<Professor> {
    return this.http.put<Professor>(`${this.apiUrl}/${id}`, professor).pipe(
       tap(() => this.refreshList())
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
       tap(() => this.refreshList())
    );
  }

  private refreshList() {
    this.getAll(
      this.pagination()?.page || 1, 
      this.pagination()?.pageSize || 10
    ).subscribe();
  }

  canAssignMoreSubjects(professor: Professor): boolean {
    // If maxSubjects is not provided by backend, we should validation logic elsewhere or assume a default
    // But ideally it comes from backend view
    return professor.maxSubjects ? professor.activeSubjectsCount < professor.maxSubjects : true;
  }
}

