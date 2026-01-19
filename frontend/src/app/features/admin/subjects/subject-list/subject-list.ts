import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

// PrimeNG
import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { SelectModule } from 'primeng/select';
import { CardModule } from 'primeng/card';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmationService } from 'primeng/api';

// Services & Models
import { SubjectService } from '../../services/subject';
import { ProfessorService } from '../../services/professor';
import { Subject } from '../../../../core/models/subject.model';
import { PageHeader, HeaderButton } from '../../../../shared/components/page-header/page-header';
import { Professor } from '../../../../core/models/professor.model';
import { Notification } from '../../../../core/services/notification';
import { SubjectForm } from '../subject-form/subject-form';

@Component({
  selector: 'app-subject-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    TagModule,
    ConfirmDialogModule,
    SelectModule,
    CardModule,
    IconFieldModule,
    InputIconModule,
    TooltipModule,
    PageHeader,
    SubjectForm
  ],
  providers: [ConfirmationService],
  templateUrl: './subject-list.html',
  styleUrl: './subject-list.css'
})
export class SubjectList implements OnInit {
  // Services
  private readonly subjectService = inject(SubjectService);
  private readonly professorService = inject(ProfessorService);
  private readonly confirmationService = inject(ConfirmationService);
  private readonly notificationService = inject(Notification);

  // Signals for state
  readonly subjects = this.subjectService.subjects;
  readonly loading = this.subjectService.loading;
  readonly pagination = this.subjectService.pagination;
  readonly professors = signal<Professor[]>([]);

  // Dialog state
  readonly showFormDialog = signal<boolean>(false);
  readonly selectedSubject = signal<Subject | null>(null);

  // Header Buttons
  readonly headerButtons: HeaderButton[] = [
    { 
      label: 'Crear Materia', 
      icon: 'pi pi-plus', 
      action: () => this.createSubject(),
      severity: 'success' 
    }
  ];

  // Filters
  search = signal<string>('');
  selectedStatus = signal<boolean | undefined>(undefined);
  selectedProfessor = signal<number | undefined>(undefined);

  // Filter Options
  statusOptions = [
    { label: 'Todos', value: undefined },
    { label: 'Activo', value: true },
    { label: 'Inactivo', value: false }
  ];

  ngOnInit() {
    this.loadProfessors();
  }

  loadSubjects(event: TableLazyLoadEvent) {
    const page = (event.first || 0) / (event.rows || 10) + 1;
    const pageSize = event.rows || 10;
    const sortField = event.sortField as string | undefined;
    const sortOrder = event.sortOrder ?? undefined;
    
    this.subjectService.getAll(
      page, 
      pageSize, 
      this.search(), 
      this.selectedStatus(),
      this.selectedProfessor(),
      sortField,
      sortOrder
    ).subscribe();
  }

  loadProfessors() {
    this.professorService.getAll(1, 100).subscribe(response => {
      if (response && response.data) {
         this.professors.set(response.data.professors); 
      }
    });
  }

  onFilter() {
    const currentPage = 1;
    const pageSize = this.pagination()?.pageSize || 10;
    // We should probably reset the table page to 0 in UI, but accessing ViewChild is extra boilerplate.
    // If the user is on page 2 and filters, ideally they go to page 1.
    // The server call will return page 1 data.
    // The table component (p-table) needs to know 'first' is 0.
    // We can bind [(first)]="first" in template.
    this.first = 0;

    this.subjectService.getAll(
      currentPage, 
      pageSize, 
      this.search(), 
      this.selectedStatus(), 
      this.selectedProfessor(),
      undefined,
      undefined
    ).subscribe();
  }

  // Helper for table reset
  first = 0;

  onSearch(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.search.set(value);
    this.onFilter();
  }

  createSubject() {
    this.selectedSubject.set(null);
    this.showFormDialog.set(true);
  }

  editSubject(subject: Subject) {
    this.selectedSubject.set(subject);
    this.showFormDialog.set(true);
  }

  deleteSubject(subject: Subject) {
    this.confirmationService.confirm({
      message: `¿Estás seguro de que deseas eliminar la materia "${subject.name}"?`,
      header: 'Confirmar Eliminación',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sí, eliminar',
      rejectLabel: 'Cancelar',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        this.subjectService.delete(subject.id).subscribe({
          next: () => {
            this.notificationService.success('Materia eliminada correctamente');
            // Reload current page
            const page = (this.pagination()?.page || 1);
            const pageSize = this.pagination()?.pageSize || 10;
            this.subjectService.getAll(
              page, 
              pageSize, 
              this.search(), 
              this.selectedStatus(),
              this.selectedProfessor(),
              undefined,
              undefined
            ).subscribe();
          },
          error: () => {
            this.notificationService.error('No se pudo eliminar la materia');
          }
        });
      }
    });
  }

  onFormClose(): void {
    this.showFormDialog.set(false);
    this.selectedSubject.set(null);
  }

  onFormSuccess(): void {
    this.showFormDialog.set(false);
    this.selectedSubject.set(null);
    // Reload current page
    const page = (this.pagination()?.page || 1);
    const pageSize = this.pagination()?.pageSize || 10;
    this.subjectService.getAll(
      page,
      pageSize,
      this.search(),
      this.selectedStatus(),
      this.selectedProfessor(),
      undefined,
      undefined
    ).subscribe();
  }
}
