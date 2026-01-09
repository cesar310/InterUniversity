import { Injectable, inject } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable({
  providedIn: 'root',
})
export class Notification {
  private readonly messageService = inject(MessageService);

  success(message: string, title = 'Éxito'): void {
    this.messageService.add({
      severity: 'success',
      summary: title,
      detail: message
    });
  }

  error(message: string, title = 'Error'): void {
    this.messageService.add({
      severity: 'error',
      summary: title,
      detail: message
    });
  }

  info(message: string, title = 'Información'): void {
    this.messageService.add({
      severity: 'info',
      summary: title,
      detail: message
    });
  }

  warn(message: string, title = 'Advertencia'): void {
    this.messageService.add({
      severity: 'warn',
      summary: title,
      detail: message
    });
  }
}
