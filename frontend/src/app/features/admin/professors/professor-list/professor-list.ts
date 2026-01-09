import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { ConfirmationService } from 'primeng/api';
import { SkeletonModule } from 'primeng/skeleton';
import { TooltipModule } from 'primeng/tooltip';
import { ProfessorService } from '../../services/professor';
import { Professor } from '../../../../core/models/professor.model';
import { PageHeader, HeaderButton } from '../../../../shared/components/page-header/page-header';
import { ProfessorForm } from '../professor-form/professor-form';
import { Notification } from '../../../../core/services/notification';

@Component({
  selector: 'app-professor-list',
  standalone: true,
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
    SkeletonModule,
    TooltipModule,
    PageHeader, 
    ProfessorForm
  ],
  templateUrl: './professor-list.html',
  styleUrl: './professor-list.css',
  providers: [ConfirmationService]
})
export class ProfessorList implements OnInit {
  private readonly professorService = inject(ProfessorService);
  private readonly confirmationService = inject(ConfirmationService);
  private readonly notificationService = inject(Notification);

  readonly professors = this.professorService.professors;
  readonly loading = this.professorService.loading;
  readonly pagination = this.professorService.pagination;
  
  readonly totalRecords = computed(() => this.pagination()?.totalItems || 0);
  
  readonly searchTerm = signal<string>('');
  
  readonly showFormDialog = signal<boolean>(false);
  readonly selectedProfessor = signal<Professor | null>(null);

  readonly headerButtons: HeaderButton[] = [
    {
      label: 'Crear Profesor',
      icon: 'pi pi-plus',
      severity: 'success',
      action: () => this.createProfessor()
    }
  ];

  ngOnInit(): void {
    // Initial load handled by p-table lazy or we can trigger if needed
  }

  loadProfessors(event: TableLazyLoadEvent): void {
    const page = (event.first || 0) / (event.rows || 10) + 1;
    const pageSize = event.rows || 10;
    this.professorService.getAll(page, pageSize, this.searchTerm()).subscribe();
  }

  onSearch(): void {
    this.professorService.getAll(1, 10, this.searchTerm()).subscribe();
  }

  createProfessor(): void {
    this.selectedProfessor.set(null);
    this.showFormDialog.set(true);
  }

  editProfessor(professor: Professor): void {
    this.selectedProfessor.set(professor);
    this.showFormDialog.set(true);
  }

  deleteProfessor(professor: Professor): void {
    this.confirmationService.confirm({
      message: `¿Estás seguro de eliminar a ${professor.name}?`,
      header: 'Confirmar Eliminación',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.professorService.delete(professor.id).subscribe({
            next: () => this.notificationService.success('Profesor eliminado'),
            error: (err) => this.notificationService.error('Error al eliminar profesor')
        });
      }
    });
  }

  onFormClose(): void {
    this.showFormDialog.set(false);
    this.selectedProfessor.set(null);
  }

  onFormSuccess(): void {
    this.onFormClose();
    this.notificationService.success('Operación exitosa');
  }

  getAvailabilitySeverity(professor: Professor): 'success' | 'warn' | 'danger' | 'info' | 'secondary' | 'contrast' | undefined {
    const max = professor.maxAllowed ?? professor.maxSubjects ?? 3; 
    const current = professor.totalSubjects ?? professor.activeSubjectsCount ?? 0;
    
    if (current >= max) return 'danger';
    if (current >= max - 1) return 'warn';
    return 'success';
  }

  getAvailabilityLabel(professor: Professor): string {
    const max = professor.maxAllowed ?? professor.maxSubjects ?? 3; 
    const current = professor.totalSubjects ?? professor.activeSubjectsCount ?? 0;
    if (current >= max) return 'Completo';
    return 'Disponible';
  }

  getStatusSeverity(isActive: boolean): 'success' | 'danger' | 'info' | 'secondary' | 'contrast' | undefined {
    return isActive ? 'success' : 'danger';
  }
}
