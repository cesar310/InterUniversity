import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AcademicOffer } from '../../../core/models/subject.model';

@Injectable({
  providedIn: 'root',
})
export class Subject {
  private readonly http = inject(HttpClient);

  readonly academicOffer = signal<AcademicOffer[]>([]);
  readonly loading = signal<boolean>(false);
  readonly searchTerm = signal<string>('');

  // Signal computado para filtrar por búsqueda
  readonly filteredOffer = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    const offer = this.academicOffer();

    if (!term) return offer;

    return offer.filter(subject =>
      subject.subject.toLowerCase().includes(term) ||
      subject.professor.toLowerCase().includes(term) ||
      subject.specialization?.toLowerCase().includes(term)
    );
  });

  /**
   * Obtiene la oferta académica completa
   */
  getAcademicOffer(): Observable<AcademicOffer[]> {
    this.loading.set(true);
    return this.http.get<{ success: boolean; data: AcademicOffer[]; message: string | null }>(
      `${environment.apiUrl}/subjects/academic-offer`
    ).pipe(
      map(response => response.data),
      tap({
        next: (data) => {
          this.academicOffer.set(data);
          this.loading.set(false);
        },
        error: () => {
          this.loading.set(false);
        }
      })
    );
  }

  /**
   * Actualiza el término de búsqueda
   */
  setSearchTerm(term: string): void {
    this.searchTerm.set(term);
  }
}
