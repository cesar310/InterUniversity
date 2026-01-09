import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { DashboardStats } from '../../../core/models/dashboard.model';

@Injectable({
  providedIn: 'root',
})
export class Dashboard {
  private readonly http = inject(HttpClient);

  readonly stats = signal<DashboardStats | null>(null);
  readonly loading = signal<boolean>(false);

  getStats(): Observable<DashboardStats> {
    this.loading.set(true);
    return this.http.get<DashboardStats>(`${environment.apiUrl}/dashboard/stats`).pipe(
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
