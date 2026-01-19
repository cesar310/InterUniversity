import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { TableModule } from 'primeng/table';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { StudentService } from '../../services/student';
import { AdminEnrollmentService, AdminEnrollment } from '../../services/admin-enrollment';
import { Student } from '../../../../core/models/student.model';
import { Notification } from '../../../../core/services/notification';
import { PageHeader, HeaderButton } from '../../../../shared/components/page-header/page-header';

@Component({
  selector: 'app-student-detail',
  imports: [CommonModule, CardModule, ButtonModule, TagModule, TableModule, ConfirmDialogModule, PageHeader],
  providers: [ConfirmationService],
  templateUrl: './student-detail.html',
  styleUrl: './student-detail.css',
})
export class StudentDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly studentService = inject(StudentService);
  private readonly enrollmentService = inject(AdminEnrollmentService);
  private readonly notificationService = inject(Notification);
  private readonly confirmationService = inject(ConfirmationService);

  readonly student = signal<Student | null>(null);
  readonly enrollments = signal<AdminEnrollment[]>([]);
  readonly loading = signal<boolean>(false);
  readonly enrollmentsLoading = signal<boolean>(false);
  readonly cancellingEnrollment = signal<{ studentId: number; subjectId: number } | null>(null);

  // Computed para contar inscripciones activas
  readonly activeEnrollmentsCount = computed(() => {
    return this.enrollments().filter(e => e.status === 'Active').length;
  });

  // Computed para el total de inscripciones
  readonly totalEnrollmentsCount = computed(() => {
    return this.enrollments().length;
  });

  readonly headerButtons: HeaderButton[] = [
    {
      label: 'Volver',
      icon: 'pi pi-arrow-left',
      severity: 'secondary',
      action: () => this.goBack()
    }
  ];

  ngOnInit(): void {
    const id = this.route.snapshot.params['id'];
    if (id) {
      this.loadStudent(+id);
      this.loadEnrollments(+id);
    }
  }

  loadStudent(id: number): void {
    this.loading.set(true);
    this.studentService.getById(id).subscribe({
      next: (student) => {
        this.student.set(student);
      },
      error: () => {
        this.notificationService.error('Error al cargar estudiante');
        this.router.navigate(['/admin/students']);
      },
      complete: () => this.loading.set(false)
    });
  }

  loadEnrollments(studentId: number): void {
    this.enrollmentsLoading.set(true);
    this.enrollmentService.getByStudentId(studentId).subscribe({
      next: (enrollments) => {
        this.enrollments.set(enrollments);
      },
      error: () => {
        this.notificationService.error('Error al cargar inscripciones');
      },
      complete: () => this.enrollmentsLoading.set(false)
    });
  }

  confirmCancelEnrollment(enrollment: AdminEnrollment): void {
    this.confirmationService.confirm({
      message: `¿Estás seguro de que deseas cancelar la inscripción de ${enrollment.subjectName}?`,
      header: 'Confirmar Cancelación',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sí, cancelar',
      rejectLabel: 'No',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.cancelEnrollment(enrollment)
    });
  }

  cancelEnrollment(enrollment: AdminEnrollment): void {
    this.cancellingEnrollment.set({ studentId: enrollment.studentId, subjectId: enrollment.subjectId });
    this.enrollmentService.cancel(enrollment.studentId, enrollment.subjectId).subscribe({
      next: () => {
        this.notificationService.success('Inscripción cancelada exitosamente');
        this.loadEnrollments(enrollment.studentId);
      },
      error: () => {
        this.notificationService.error('Error al cancelar inscripción');
      },
      complete: () => this.cancellingEnrollment.set(null)
    });
  }

  isCancelling(studentId: number, subjectId: number): boolean {
    const cancelling = this.cancellingEnrollment();
    return cancelling?.studentId === studentId && cancelling?.subjectId === subjectId;
  }

  goBack(): void {
    this.router.navigate(['/admin/students']);
  }

  getStatusSeverity(status: string): 'success' | 'warn' | 'danger' {
    switch (status) {
      case 'active': return 'success';
      case 'completed': return 'warn';
      case 'cancelled': return 'danger';
      default: return 'success';
    }
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'active': return 'Activa';
      case 'completed': return 'Completada';
      case 'cancelled': return 'Cancelada';
      default: return status;
    }
  }

  getActiveStatusSeverity(isActive: boolean): 'success' | 'danger' {
    return isActive ? 'success' : 'danger';
  }

  getActiveStatusLabel(isActive: boolean): string {
    return isActive ? 'Activo' : 'Inactivo';
  }
}
