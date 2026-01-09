import { Component, inject, input, output, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { form, required, minLength, email, Field } from '@angular/forms/signals';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { PasswordModule } from 'primeng/password';
import { DialogModule } from 'primeng/dialog';
import { StudentService } from '../../services/student';
import { Student, RegisterStudentRequest, StudentWithEnrollments } from '../../../../core/models/student.model';
import { Notification } from '../../../../core/services/notification';

@Component({
  selector: 'app-student-form',
  imports: [CommonModule, FormsModule, CardModule, InputTextModule, ButtonModule, PasswordModule, DialogModule, Field],
  templateUrl: './student-form.html',
  styleUrl: './student-form.css',
})
export class StudentForm {
  private readonly studentService = inject(StudentService);
  private readonly notificationService = inject(Notification);

  readonly visible = input<boolean>(false);
  readonly student = input<Student | StudentWithEnrollments | null>(null);
  readonly onClose = output<void>();
  readonly onSuccess = output<void>();

  readonly isEdit = computed(() => !!this.student());
  readonly loading = signal<boolean>(false);

  readonly formModel = signal<RegisterStudentRequest>({
    name: '',
    email: ''
  });

  readonly form = form(this.formModel, (schemaPath) => {
    required(schemaPath.name, { message: 'El nombre es requerido' });
    minLength(schemaPath.name, 3, { message: 'El nombre debe tener al menos 3 caracteres' });
    required(schemaPath.email, { message: 'El email es requerido' });
    email(schemaPath.email, { message: 'Ingrese un email válido' });
  });

  readonly temporaryPassword = signal<string>('');

  constructor() {
    // Reset form when dialog opens/closes
    effect(() => {
      if (this.visible()) {
        this.resetForm();
      }
    });
  }

  resetForm(): void {
    if (this.isEdit()) {
      const student = this.student()!;
      const name = 'studentName' in student ? student.studentName : student.name;
      const email = student.email;
      this.formModel.set({
        name,
        email
      });
    } else {
      this.formModel.set({
        name: '',
        email: ''
      });
    }
    this.temporaryPassword.set('');
  }

  onSubmit(): void {
    // Mark all as touched
    this.form.name().markAsTouched();
    this.form.email().markAsTouched();

    if (!this.form.name().valid() || !this.form.email().valid()) {
      return;
    }

    this.loading.set(true);

    if (this.isEdit()) {
      this.updateStudent();
    } else {
      this.createStudent();
    }
  }

  private createStudent(): void {
    const request: RegisterStudentRequest = this.formModel();

    this.studentService.create(request).subscribe({
      next: (response) => {
        this.temporaryPassword.set(response.temporaryPassword);
        this.notificationService.success('Estudiante creado exitosamente');
        this.onSuccess.emit();
      },
      error: (error) => {
        if (error.status === 409) {
          this.notificationService.error('El email ya está registrado');
        } else {
          this.notificationService.error('Error al crear estudiante');
        }
      },
      complete: () => this.loading.set(false)
    });
  }

  private updateStudent(): void {
    const student = this.student()!;
    const request = this.formModel();
    const studentId = 'studentId' in student ? student.studentId : student.id;
    const studentCode = 'studentCode' in student ? student.studentCode : '';

    this.studentService.update(studentId, {
      studentId,
      name: request.name,
      studentCode
    }).subscribe({
      next: () => {
        this.notificationService.success('Estudiante actualizado exitosamente');
        this.onSuccess.emit();
        this.closeDialog();
      },
      error: () => {
        this.notificationService.error('Error al actualizar estudiante');
      },
      complete: () => this.loading.set(false)
    });
  }

  closeDialog(): void {
    this.onClose.emit();
  }

  getFieldError(fieldName: keyof RegisterStudentRequest): string {
    const fieldState = this.form[fieldName]();
    if (fieldState.errors().length > 0 && fieldState.touched()) {
      return fieldState.errors()[0].message || 'Error de validación';
    }
    return '';
  }
}
