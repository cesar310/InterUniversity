import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { ConfirmationService } from 'primeng/api';
import { StudentService } from '../../services/student';
import { StudentWithEnrollments } from '../../../../core/models/student.model';
import { PageHeader, HeaderButton } from '../../../../shared/components/page-header/page-header';
import { StudentForm } from '../student-form/student-form';
import { Router } from '@angular/router';
import { Notification } from '../../../../core/services/notification';

@Component({
  selector: 'app-student-list',
  imports: [
    CommonModule, 
    FormsModule, 
    TableModule, 
    ButtonModule, 
    TagModule, 
    InputTextModule, 
    CardModule, 
    ConfirmDialogModule,
    IconFieldModule,
    InputIconModule, 
    PageHeader, 
    StudentForm
  ],
  templateUrl: './student-list.html',
  styleUrl: './student-list.css',
  providers: [ConfirmationService]
})
export class StudentList implements OnInit {
  private readonly studentService = inject(StudentService);
  private readonly router = inject(Router);
  private readonly confirmationService = inject(ConfirmationService);
  private readonly notificationService = inject(Notification);

  readonly students = this.studentService.studentsWithEnrollments;
  readonly loading = this.studentService.loading;
  readonly searchTerm = signal<string>('');

  readonly filteredStudents = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    const students = this.students();
    
    if (!term) return students;

    return students.filter(student => 
      student.studentName.toLowerCase().includes(term) ||
      student.email.toLowerCase().includes(term) ||
      student.studentCode.toLowerCase().includes(term)
    );
  });

  readonly showFormDialog = signal<boolean>(false);
  readonly selectedStudent = signal<StudentWithEnrollments | null>(null);

  readonly headerButtons: HeaderButton[] = [
    {
      label: 'Crear Estudiante',
      icon: 'pi pi-plus',
      severity: 'success',
      action: () => this.createStudent()
    },
    {
      label: 'Ver Carga Académica',
      icon: 'pi pi-chart-bar',
      severity: 'info',
      action: () => this.viewAcademicLoad()
    }
  ];

  ngOnInit(): void {
    this.loadStudents();
  }

  loadStudents(): void {
    this.studentService.getStudentsWithEnrollments().subscribe();
  }

  createStudent(): void {
    this.selectedStudent.set(null);
    this.showFormDialog.set(true);
  }

  viewAcademicLoad(): void {
    // Ya está cargando con getStudentsWithEnrollments
    console.log('Ver carga académica');
  }

  onSearch(): void {
    // La búsqueda se maneja reactivamente con filteredStudents computed signal
  }

  viewStudent(student: StudentWithEnrollments): void {
    this.router.navigate(['/admin/students', student.studentId]);
  }

  editStudent(student: StudentWithEnrollments): void {
    this.selectedStudent.set(student);
    this.showFormDialog.set(true);
  }

  deleteStudent(student: StudentWithEnrollments): void {
    this.confirmationService.confirm({
      message: `¿Está seguro de que desea eliminar al estudiante ${student.studentName}?`,
      header: 'Confirmar Eliminación',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Eliminar',
      rejectLabel: 'Cancelar',
      accept: () => {
        this.studentService.delete(student.studentId).subscribe({
          next: () => {
            this.notificationService.success('Estudiante eliminado exitosamente');
            this.loadStudents();
          },
          error: () => {
            this.notificationService.error('Error al eliminar estudiante');
          }
        });
      }
    });
  }

  onFormClose(): void {
    this.showFormDialog.set(false);
  }

  onFormSuccess(): void {
    this.showFormDialog.set(false);
    this.loadStudents();
  }

  getStatusSeverity(isActive: boolean): 'success' | 'danger' {
    return isActive ? 'success' : 'danger';
  }

  getStatusLabel(isActive: boolean): string {
    return isActive ? 'Activo' : 'Inactivo';
  }
}
