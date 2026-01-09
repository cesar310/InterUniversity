import { Component, computed, inject, input, output, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { form, Field, required, email, minLength } from '@angular/forms/signals';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { ProfessorService } from '../../services/professor';
import { Professor } from '../../../../core/models/professor.model';
import { Notification } from '../../../../core/services/notification';
import { extractErrorMessage } from '../../../../core/interceptors/error.interceptor';

@Component({
  selector: 'app-professor-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DialogModule,
    ButtonModule,
    InputTextModule,
    CheckboxModule,
    Field
  ],
  templateUrl: './professor-form.html',
  styleUrl: './professor-form.css'
})
export class ProfessorForm {
    readonly visible = input<boolean>(false);
    readonly professor = input<Professor | null>(null);
    readonly onClose = output<void>();
    readonly onSuccess = output<void>();

    private readonly professorService = inject(ProfessorService);
    private readonly notificationService = inject(Notification);

    // Model Signal
    readonly professorModel = signal({
        name: '',
        specialization: '',
        email: '',
        phone: '',
        isActive: true
    });

    // Signal Form
    readonly professorForm = form(this.professorModel, (f) => {
        required(f.name);
        minLength(f.name, 3);
        required(f.specialization);
        email(f.email);
    });

    readonly loading = signal<boolean>(false);
    readonly dialogTitle = computed(() => this.professor() ? 'Editar Profesor' : 'Crear Profesor');

    constructor() {
        effect(() => {
            const prof = this.professor();
            if (prof) {
                // Update model directly
                this.professorModel.set({
                    name: prof.name,
                    specialization: prof.specialization,
                    email: prof.email || '',
                    phone: prof.phone || '',
                    isActive: prof.isActive
                });
            } else {
                this.professorModel.set({
                    name: '',
                    specialization: '',
                    email: '',
                    phone: '',
                    isActive: true
                });
            }
        });
    }

    handleCancel() {
        this.onClose.emit();
    }

    handleSubmit() {
        if (this.professorForm().invalid()) {
            return;
        }

        this.loading.set(true);
        const values = this.professorModel();
        const prof = this.professor();

        const request$ = prof 
            ? this.professorService.update(prof.id, { 
                professorId: prof.id,
                name: values.name,
                specialization: values.specialization,
                email: values.email || '',
                phone: values.phone || '',
                isActive: values.isActive
              })
            : this.professorService.create(values as any); 

        request$.subscribe({
            next: () => {
                 this.loading.set(false);
                 this.onSuccess.emit();
            },
            error: (err) => {
                this.loading.set(false);
                this.notificationService.error(extractErrorMessage(err));
            }
        });
    }
}
