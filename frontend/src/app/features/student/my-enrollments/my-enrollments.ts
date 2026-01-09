import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { CardModule } from 'primeng/card';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmationService } from 'primeng/api';
import { Enrollment, ClassmateResponse } from '../services/enrollment';
import { Auth } from '../../../core/services/auth';
import { Notification } from '../../../core/services/notification';
import { SystemConfigService } from '../../../core/services/system-config';
import { PageHeader } from '../../../shared/components/page-header/page-header';
import { extractErrorMessage } from '../../../core/interceptors/error.interceptor';

@Component({
  selector: 'app-my-enrollments',
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    TagModule,
    CardModule,
    DialogModule,
    ConfirmDialogModule,
    TooltipModule,
    PageHeader
  ],
  templateUrl: './my-enrollments.html',
  styleUrl: './my-enrollments.css',
  providers: [ConfirmationService]
})
export class MyEnrollments implements OnInit {
  private readonly enrollmentService = inject(Enrollment);
  private readonly authService = inject(Auth);
  private readonly confirmationService = inject(ConfirmationService);
  private readonly notificationService = inject(Notification);
  readonly configService = inject(SystemConfigService);

  readonly myEnrollments = this.enrollmentService.myEnrollments;
  readonly activeEnrollmentsCount = this.enrollmentService.activeEnrollmentsCount;
  readonly maxAllowed = this.enrollmentService.maxAllowed;
  readonly totalCredits = this.enrollmentService.totalCredits;
  readonly loading = this.enrollmentService.loading;

  readonly showClassmatesDialog = signal<boolean>(false);
  readonly classmates = signal<ClassmateResponse[]>([]);
  readonly selectedSubjectName = signal<string>('');
  readonly loadingClassmates = signal<boolean>(false);

  ngOnInit(): void {
    this.loadEnrollments();
  }

  loadEnrollments(): void {
    this.enrollmentService.getMyEnrollments().subscribe({
      error: (error) => {
        const message = extractErrorMessage(error, 'Error al cargar inscripciones');
        this.notificationService.error(message);
      }
    });
  }

  viewClassmates(subjectId: number, subjectName: string): void {
    this.selectedSubjectName.set(subjectName);
    this.loadingClassmates.set(true);
    this.showClassmatesDialog.set(true);

    this.enrollmentService.getClassmates(subjectId).subscribe({
      next: (data) => {
        this.classmates.set(data);
        this.loadingClassmates.set(false);
      },
      error: (error) => {
        const message = extractErrorMessage(error, 'Error al cargar compañeros');
        this.notificationService.error(message);
        this.loadingClassmates.set(false);
        this.showClassmatesDialog.set(false);
      }
    });
  }

  cancelEnrollment(subjectId: number, subjectName: string): void {
    const user = this.authService.currentUser();
    if (!user?.studentId) {
      this.notificationService.error('No se pudo identificar al estudiante');
      return;
    }

    this.confirmationService.confirm({
      message: `¿Estás seguro de que deseas cancelar tu inscripción en ${subjectName}?`,
      header: 'Confirmar Cancelación',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sí, cancelar',
      rejectLabel: 'No',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.enrollmentService.cancel(user.studentId!, subjectId).subscribe({
          next: () => {
            this.notificationService.success('Inscripción cancelada exitosamente');
          },
          error: (error) => {
            const message = extractErrorMessage(error, 'Error al cancelar inscripción');
            this.notificationService.error(message);
          }
        });
      }
    });
  }

  getStatusSeverity(status: string): 'success' | 'warn' | 'danger' | 'info' {
    switch (status) {
      case 'active':
        return 'success';
      case 'completed':
        return 'info';
      case 'cancelled':
        return 'danger';
      default:
        return 'warn';
    }
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'active':
        return 'Activa';
      case 'completed':
        return 'Completada';
      case 'cancelled':
        return 'Cancelada';
      default:
        return status;
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }
}
