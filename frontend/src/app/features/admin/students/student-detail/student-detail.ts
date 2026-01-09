import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { TableModule } from 'primeng/table';
import { StudentService } from '../../services/student';
import { Student } from '../../../../core/models/student.model';
import { PageHeader, HeaderButton } from '../../../../shared/components/page-header/page-header';
import { StudentForm } from '../student-form/student-form';

@Component({
  selector: 'app-student-detail',
  imports: [CommonModule, CardModule, ButtonModule, TagModule, TableModule, PageHeader, StudentForm],
  templateUrl: './student-detail.html',
  styleUrl: './student-detail.css',
})
export class StudentDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly studentService = inject(StudentService);

  readonly student = signal<Student | null>(null);
  readonly loading = signal<boolean>(false);
  readonly showEditDialog = signal<boolean>(false);

  readonly headerButtons: HeaderButton[] = [
    {
      label: 'Editar',
      icon: 'pi pi-pencil',
      severity: 'secondary',
      action: () => this.editStudent()
    },
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
    }
  }

  loadStudent(id: number): void {
    this.loading.set(true);
    this.studentService.getById(id).subscribe({
      next: (student) => {
        this.student.set(student);
      },
      error: () => {
        // TODO: Mostrar error y redirigir
      },
      complete: () => this.loading.set(false)
    });
  }

  editStudent(): void {
    this.showEditDialog.set(true);
  }

  onEditClose(): void {
    this.showEditDialog.set(false);
  }

  onEditSuccess(): void {
    this.showEditDialog.set(false);
    // Navigate back to list to see changes
    this.router.navigate(['/admin/students']);
  }

  goBack(): void {
    this.router.navigate(['/admin/students']);
  }

  getStatusSeverity(isActive: boolean): 'success' | 'danger' {
    return isActive ? 'success' : 'danger';
  }

  getStatusLabel(isActive: boolean): string {
    return isActive ? 'Activo' : 'Inactivo';
  }
}
