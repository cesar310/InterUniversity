import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { DashboardStats } from '../../../core/models/dashboard.model';

interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class Dashboard {
  private readonly http = inject(HttpClient);

  readonly stats = signal<DashboardStats | null>(null);
  readonly loading = signal<boolean>(false);

  getStats(): Observable<DashboardStats> {
    this.loading.set(true);
    return this.http.get<ApiResponse<DashboardStats>>(`${environment.apiUrl}/dashboard/stats`).pipe(
      map(response => response.data),
      tap({
        next: (stats) => {
          this.stats.set(stats);
          this.loading.set(false);
        },
        error: () => {
          this.loading.set(false);
        }
      })
    );
  }
}
