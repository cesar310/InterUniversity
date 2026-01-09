import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { CardModule } from 'primeng/card';
import { TimelineModule } from 'primeng/timeline';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { SystemConfigService } from '../../services/system-config';
import { ConfigAudit as ConfigAuditModel } from '../../../../core/models/system-config.model';
import { PageHeader, HeaderButton } from '../../../../shared/components/page-header/page-header';
import { Router } from '@angular/router';

@Component({
  selector: 'app-config-audit',
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    TagModule,
    CardModule,
    TimelineModule,
    SelectModule,
    DatePickerModule,
    IconFieldModule,
    InputIconModule,
    PageHeader
  ],
  templateUrl: './config-audit.html',
  styleUrl: './config-audit.css'
})
export class ConfigAudit implements OnInit {
  private readonly configService = inject(SystemConfigService);
  private readonly router = inject(Router);

  readonly auditLogs = this.configService.auditLogs;
  readonly loading = this.configService.loading;

  readonly selectedKey = signal<string | null>(null);
  readonly dateRange = signal<Date[] | null>(null);
  readonly viewMode = signal<'table' | 'timeline'>('table');

  readonly configKeys = computed(() => {
    const logs = this.auditLogs();
    const keys = new Set(logs.map(log => log.configKey));
    return Array.from(keys).map(key => ({ label: key, value: key }));
  });

  readonly filteredLogs = computed(() => {
    let logs = this.auditLogs();
    
    // Filtrar por clave
    const key = this.selectedKey();
    if (key) {
      logs = logs.filter(log => log.configKey === key);
    }

    // Filtrar por rango de fechas
    const range = this.dateRange();
    if (range && range.length === 2 && range[0] && range[1]) {
      const start = range[0].getTime();
      const end = range[1].getTime();
      logs = logs.filter(log => {
        const logDate = new Date(log.changedAt).getTime();
        return logDate >= start && logDate <= end;
      });
    }

    return logs;
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
    this.loadAuditLog();
  }

  loadAuditLog(): void {
    this.configService.getAuditLog().subscribe();
  }

  goBack(): void {
    this.router.navigate(['/admin/system-config']);
  }

  clearFilters(): void {
    this.selectedKey.set(null);
    this.dateRange.set(null);
  }

  toggleViewMode(): void {
    this.viewMode.update(mode => mode === 'table' ? 'timeline' : 'table');
  }

  getChangeColor(log: ConfigAuditModel): string {
    // Colorear cambios según tipo
    const oldNum = parseFloat(log.oldValue);
    const newNum = parseFloat(log.newValue);
    
    if (!isNaN(oldNum) && !isNaN(newNum)) {
      if (newNum > oldNum) return 'text-green-600'; // Aumento
      if (newNum < oldNum) return 'text-red-600';   // Disminución
    }
    
    return 'text-blue-600'; // Cambio de string/boolean
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleString('es-ES', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    });
  }

  formatDateShort(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleString('es-ES', {
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
