import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { CardModule } from 'primeng/card';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmationService } from 'primeng/api';
import { SystemConfigService } from '../../services/system-config';
import { SystemConfig, ConfigValueType } from '../../../../core/models/system-config.model';
import { PageHeader, HeaderButton } from '../../../../shared/components/page-header/page-header';
import { Notification } from '../../../../core/services/notification';
import { Router } from '@angular/router';

@Component({
  selector: 'app-config-manager',
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    TagModule,
    InputTextModule,
    InputNumberModule,
    ToggleSwitchModule,
    CardModule,
    ConfirmDialogModule,
    TooltipModule,
    PageHeader
  ],
  templateUrl: './config-manager.html',
  styleUrl: './config-manager.css',
  providers: [ConfirmationService]
})
export class ConfigManager implements OnInit {
  private readonly configService = inject(SystemConfigService);
  private readonly confirmationService = inject(ConfirmationService);
  private readonly notificationService = inject(Notification);
  private readonly router = inject(Router);

  readonly configs = computed(() => 
    this.configService.configs().filter(c => c.isEditable)
  );
  readonly loading = this.configService.loading;

  readonly editingConfig = signal<number | null>(null);
  readonly editingValue = signal<string | number>('');
  readonly editingBoolValue = signal<boolean>(false);

  readonly headerButtons: HeaderButton[] = [
    {
      label: 'Ver Historial de Auditoría',
      icon: 'pi pi-history',
      severity: 'info',
      action: () => this.viewAudit()
    }
  ];

  ngOnInit(): void {
    this.loadConfigs();
  }

  loadConfigs(): void {
    this.configService.loadAll().subscribe();
  }

  viewAudit(): void {
    this.router.navigate(['/admin/config-audit']);
  }

  startEdit(config: SystemConfig): void {
    this.editingConfig.set(config.id);
    
    if (config.valueType === ConfigValueType.Boolean) {
      this.editingBoolValue.set(config.configValue.toLowerCase() === 'true');
      this.editingValue.set('');
    } else if (config.valueType === ConfigValueType.Int) {
      this.editingValue.set(parseInt(config.configValue));
    } else if (config.valueType === ConfigValueType.Decimal) {
      this.editingValue.set(parseFloat(config.configValue));
    } else {
      this.editingValue.set(config.configValue);
    }
  }

  cancelEdit(): void {
    this.editingConfig.set(null);
    this.editingValue.set('');
    this.editingBoolValue.set(false);
  }

  saveEdit(config: SystemConfig): void {
    let newValue: string;
    
    // Para boolean, usar el valor del toggle
    if (config.valueType === ConfigValueType.Boolean) {
      newValue = this.editingBoolValue().toString();
    } else {
      newValue = this.editingValue().toString();
    }
    
    // Validar tipo de dato
    if (!this.validateValue(config.valueType, newValue)) {
      this.notificationService.error(`Valor inválido para tipo ${config.valueType}`);
      return;
    }

    this.confirmationService.confirm({
      message: `¿Está seguro de actualizar "${config.configKey}" de "${config.configValue}" a "${newValue}"?`,
      header: 'Confirmar Actualización',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Actualizar',
      rejectLabel: 'Cancelar',
      accept: () => {
        this.configService.updateConfig(config.configKey, newValue).subscribe({
          next: () => {
            this.notificationService.success('Configuración actualizada exitosamente');
            this.cancelEdit();
            this.loadConfigs();
          },
          error: () => {
            this.notificationService.error('Error al actualizar configuración');
          }
        });
      }
    });
  }

  private validateValue(type: ConfigValueType, value: string): boolean {
    if (!value || value.trim() === '') return false;

    switch (type) {
      case ConfigValueType.Int:
        return !isNaN(parseInt(value)) && parseInt(value).toString() === value;
      case ConfigValueType.Decimal:
        return !isNaN(parseFloat(value));
      case ConfigValueType.Boolean:
        return value.toLowerCase() === 'true' || value.toLowerCase() === 'false';
      case ConfigValueType.String:
        return true;
      default:
        return false;
    }
  }

  isEditing(configId: number): boolean {
    return this.editingConfig() === configId;
  }

  getTypeSeverity(type: ConfigValueType): 'success' | 'info' | 'warn' | 'secondary' {
    switch (type) {
      case ConfigValueType.Int:
        return 'info';
      case ConfigValueType.Decimal:
        return 'success';
      case ConfigValueType.Boolean:
        return 'warn';
      case ConfigValueType.String:
        return 'secondary';
      default:
        return 'secondary';
    }
  }

  getTypeName(type: ConfigValueType): string {
    // El tipo ya viene como string del backend, solo retornarlo
    return type || 'Unknown';
  }

  formatDate(dateString?: string): string {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.toLocaleString('es-ES', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
