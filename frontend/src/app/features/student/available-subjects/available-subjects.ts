import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataViewModule } from 'primeng/dataview';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { DialogModule } from 'primeng/dialog';
import { TooltipModule } from 'primeng/tooltip';
import { TagModule } from 'primeng/tag';
import { DividerModule } from 'primeng/divider';
import { Subject } from '../services/subject';
import { Enrollment } from '../services/enrollment';
import { Auth } from '../../../core/services/auth';
import { Notification } from '../../../core/services/notification';
import { SystemConfigService } from '../../../core/services/system-config';
import { PageHeader } from '../../../shared/components/page-header/page-header';
import { AcademicOffer } from '../../../core/models/subject.model';
import { extractErrorMessage } from '../../../core/interceptors/error.interceptor';

@Component({
  selector: 'app-available-subjects',
  imports: [
    CommonModule,
    FormsModule,
    DataViewModule,
    ButtonModule,
    CardModule,
    InputTextModule,
    IconFieldModule,
    InputIconModule,
    DialogModule,
    TooltipModule,
    TagModule,
    DividerModule,
    PageHeader
  ],
  templateUrl: './available-subjects.html',
  styleUrl: './available-subjects.css'
})
export class AvailableSubjects implements OnInit {
  private readonly subjectService = inject(Subject);
  private readonly enrollmentService = inject(Enrollment);
  private readonly authService = inject(Auth);
  private readonly notificationService = inject(Notification);
  readonly configService = inject(SystemConfigService);

  readonly academicOffer = this.subjectService.academicOffer;
  readonly loading = this.subjectService.loading;
  readonly enrollingSubjectId = signal<number | null>(null);
  readonly showConfirmDialog = signal<boolean>(false);
  readonly showDetailDialog = signal<boolean>(false);
  readonly selectedSubject = signal<AcademicOffer | null>(null);
  
  // Usar el filtro del servicio que ya está implementado
  readonly filteredAcademicOffer = this.subjectService.filteredOffer;
  readonly searchTermValue = this.subjectService.searchTerm;

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    // Cargar ambos: oferta académica e inscripciones del estudiante
    this.loadAcademicOffer();
    this.loadMyEnrollments();
  }

  loadAcademicOffer(): void {
    this.subjectService.getAcademicOffer().subscribe({
      error: (error) => {
        const message = extractErrorMessage(error, 'Error al cargar oferta académica');
        this.notificationService.error(message);
      }
    });
  }

  loadMyEnrollments(): void {
    this.enrollmentService.getMyEnrollments().subscribe({
      error: (error) => {
        const message = extractErrorMessage(error, 'Error al cargar tus inscripciones');
        this.notificationService.error(message);
      }
    });
  }

  onSearch(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.subjectService.setSearchTerm(target.value);
  }

  enrollInSubject(subject: AcademicOffer): void {
    // Validar si las inscripciones están abiertas
    if (!this.configService.enrollmentOpen()) {
      this.notificationService.warn('Las inscripciones están cerradas en este momento');
      return;
    }

    const validation = this.enrollmentService.canEnrollInSubject(subject);

    if (!validation.canEnroll) {
      this.notificationService.warn(validation.reason!);
      return;
    }

    // Mostrar el diálogo de confirmación
    this.selectedSubject.set(subject);
    this.showConfirmDialog.set(true);
  }

  confirmEnrollment(): void {
    const subject = this.selectedSubject();
    if (!subject) return;

    this.enrollingSubjectId.set(subject.subjectId);
    this.enrollmentService.enroll(subject.subjectId).subscribe({
      next: () => {
        this.enrollingSubjectId.set(null);
        this.showConfirmDialog.set(false);
        this.selectedSubject.set(null);
        this.notificationService.success(`Te has inscrito exitosamente en ${subject.subject}`);
        // Recargar la oferta académica para actualizar contadores
        this.loadAcademicOffer();
      },
      error: (error) => {
        this.enrollingSubjectId.set(null);
        this.showConfirmDialog.set(false);
        this.selectedSubject.set(null);
        const message = extractErrorMessage(error, 'Error al inscribirse en la materia');
        this.notificationService.error(message);
      }
    });
  }

  cancelEnrollment(): void {
    if (this.enrollingSubjectId() === null) {
      this.showConfirmDialog.set(false);
      this.selectedSubject.set(null);
    }
  }

  getEnrollButtonTooltip(subject: AcademicOffer): string {
    // Primero verificar si las inscripciones están cerradas
    if (!this.configService.enrollmentOpen()) {
      return 'Las inscripciones están cerradas';
    }
    
    const validation = this.enrollmentService.canEnrollInSubject(subject);
    return validation.canEnroll ? 'Inscribirse en esta materia' : validation.reason!;
  }

  getEnrollButtonSeverity(subject: AcademicOffer): 'success' | 'secondary' {
    // Si las inscripciones están cerradas, mostrar como deshabilitado
    if (!this.configService.enrollmentOpen()) {
      return 'secondary';
    }
    
    const validation = this.enrollmentService.canEnrollInSubject(subject);
    return validation.canEnroll ? 'success' : 'secondary';
  }

  getEnrollButtonDisabled(subject: AcademicOffer): boolean {
    // Deshabilitar si las inscripciones están cerradas
    if (!this.configService.enrollmentOpen()) {
      return true;
    }
    
    // Deshabilitar si cualquier materia está siendo inscrita
    if (this.enrollingSubjectId() !== null) {
      return true;
    }
    
    const validation = this.enrollmentService.canEnrollInSubject(subject);
    return !validation.canEnroll;
  }

  isSubjectEnrolling(subjectId: number): boolean {
    return this.enrollingSubjectId() === subjectId;
  }

  openSubjectDetail(subject: AcademicOffer): void {
    this.selectedSubject.set(subject);
    this.showDetailDialog.set(true);
  }

  closeDetailDialog(): void {
    this.showDetailDialog.set(false);
  }

  enrollFromDetail(): void {
    const subject = this.selectedSubject();
    if (!subject) return;
    
    this.showDetailDialog.set(false);
    this.enrollInSubject(subject);
  }
}
