import { Component, computed, inject, input, output, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { form, Field, required } from '@angular/forms/signals';

// PrimeNG
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { CheckboxModule } from 'primeng/checkbox';
import { TagModule } from 'primeng/tag';
import { TextareaModule } from 'primeng/textarea';

// Services & Models
import { SubjectService } from '../../services/subject';
import { ProfessorService } from '../../services/professor';
import { Professor } from '../../../../core/models/professor.model';
import { Subject, CreateSubjectRequest, UpdateSubjectRequest } from '../../../../core/models/subject.model';
import { Notification } from '../../../../core/services/notification';
import { extractErrorMessage } from '../../../../core/interceptors/error.interceptor';

@Component({
  selector: 'app-subject-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    Field,
    DialogModule,
    ButtonModule,
    InputTextModule,
    InputNumberModule,
    SelectModule,
    CheckboxModule,
    TagModule,
    TextareaModule
  ],
  templateUrl: './subject-form.html',
  styleUrl: './subject-form.css'
})
export class SubjectForm {
  readonly visible = input<boolean>(false);
  readonly subject = input<Subject | null>(null);
  readonly onClose = output<void>();
  readonly onSuccess = output<void>();

  private readonly subjectService = inject(SubjectService);
  private readonly professorService = inject(ProfessorService);
  private readonly notificationService = inject(Notification);

  readonly professors = signal<Professor[]>([]);
  readonly loading = signal<boolean>(false);

  // Model Signal
  readonly subjectModel = signal<CreateSubjectRequest & { isActive: boolean }>({ 
    name: '',
    description: '',
    credits: 3,
    professorId: 0,
    isActive: true
  });

  // Signal Form
  readonly registerForm = form(this.subjectModel, (fieldPath) => {
    required(fieldPath.name);
    required(fieldPath.credits);
    required(fieldPath.professorId);
  });

  readonly dialogTitle = computed(() => this.subject() ? 'Editar Materia' : 'Crear Materia');

  readonly selectedProfessor = computed(() => {
    const id = this.subjectModel().professorId;
    return this.professors().find(p => p.id === id);
  });

  readonly isProfessorLimitReached = computed(() => {
    const prof = this.selectedProfessor();
    if (!prof) return false;
    const active = prof.activeSubjectsCount || 0;
    const max = prof.maxAllowed || 3;
    return active >= max;
  });

  constructor() {
    effect(() => {
      const reached = this.isProfessorLimitReached();
      if (reached && this.selectedProfessor()) {
        this.notificationService.warn(
          `El profesor ${this.selectedProfessor()?.name} ha alcanzado o superado su límite de materias (${this.selectedProfessor()?.maxAllowed}).`
        );
      }
    });

    effect(() => {
      const subj = this.subject();
      if (subj) {
        this.subjectModel.set({
          name: subj.name,
          description: subj.description || '',
          credits: subj.credits,
          professorId: subj.professorId,
          isActive: subj.isActive
        });
      } else {
        this.subjectModel.set({
          name: '',
          description: '',
          credits: 3,
          professorId: 0,
          isActive: true
        });
      }
    });

    effect(() => {
      if (this.visible()) {
        this.loadProfessors();
      }
    });
  }

  loadProfessors() {
    this.professorService.getAll(1, 100).subscribe(response => {
      if (response && response.data) {
        const allProfs = response.data.professors;
        const activeProfs = allProfs.filter(p => p.isActive);
        this.professors.set(activeProfs);
      }
    });
  }

  handleCancel() {
    this.onClose.emit();
  }

  handleSubmit() {
    if (this.registerForm().invalid()) {
      this.notificationService.error('Formulario inválido');
      return;
    }

    const { name, description, credits, professorId, isActive } = this.subjectModel();
    this.loading.set(true);

    const subj = this.subject();
    const request$ = subj
      ? this.subjectService.update(subj.id, {
          subjectId: subj.id,
          name,
          description: description || undefined,
          credits,
          professorId,
          isActive
        })
      : this.subjectService.create({
          name,
          description: description || undefined,
          credits,
          professorId: professorId!
        });

    request$.subscribe({
      next: () => {
        this.loading.set(false);
        this.notificationService.success(
          subj ? 'Materia actualizada correctamente' : 'Materia creada correctamente'
        );
        this.onSuccess.emit();
      },
      error: (err) => {
        this.loading.set(false);
        this.notificationService.error(extractErrorMessage(err));
      }
    });
  }

  updateField(field: keyof CreateSubjectRequest | 'isActive', value: any) {
    this.subjectModel.update(m => ({ ...m, [field]: value }));
  }
}
