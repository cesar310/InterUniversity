import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { SystemConfig, UpdateConfigRequest, ConfigAudit, ConfigValueType } from '../../../core/models/system-config.model';

@Injectable({
  providedIn: 'root',
})
export class SystemConfigService {
  private readonly http = inject(HttpClient);

  readonly configs = signal<SystemConfig[]>([]);
  readonly auditLogs = signal<ConfigAudit[]>([]);
  readonly loading = signal<boolean>(false);

  // Signals computados para valores clave
  readonly maxSubjectsPerStudent = computed(() => {
    const config = this.configs().find(c => c.configKey === 'max_subjects_per_student');
    return config ? parseInt(config.configValue) : 3;
  });

  readonly maxSubjectsPerProfessor = computed(() => {
    const config = this.configs().find(c => c.configKey === 'max_subjects_per_professor');
    return config ? parseInt(config.configValue) : 5;
  });

  readonly defaultCredits = computed(() => {
    const config = this.configs().find(c => c.configKey === 'default_credits');
    return config ? parseInt(config.configValue) : 3;
  });

  loadAll(): Observable<SystemConfig[]> {
    this.loading.set(true);
    return this.http.get<{ success: boolean; data: SystemConfig[]; message: string | null }>(`${environment.apiUrl}/config`).pipe(
      map(response => response.data),
      tap({
        next: (configs) => {
          this.configs.set(configs);
          this.loading.set(false);
        },
        error: () => {
          this.loading.set(false);
        }
      })
    );
  }

  getByKey(key: string): Observable<SystemConfig> {
    this.loading.set(true);
    return this.http.get<{ success: boolean; data: SystemConfig; message: string | null }>(`${environment.apiUrl}/config/${key}`).pipe(
      map(response => response.data),
      tap({
        next: () => this.loading.set(false),
        error: () => this.loading.set(false)
      })
    );
  }

  updateConfig(key: string, value: string): Observable<SystemConfig> {
    this.loading.set(true);
    const request: UpdateConfigRequest = { value };
    return this.http.patch<{ success: boolean; data: SystemConfig; message: string | null }>(`${environment.apiUrl}/config/${key}`, request).pipe(
      map(response => response.data),
      tap({
        next: (updatedConfig) => {
          // Actualizar el signal local
          const currentConfigs = this.configs();
          const index = currentConfigs.findIndex(c => c.configKey === key);
          if (index !== -1) {
            currentConfigs[index] = updatedConfig;
            this.configs.set([...currentConfigs]);
          }
          this.loading.set(false);
        },
        error: () => {
          this.loading.set(false);
        }
      })
    );
  }

  getAuditLog(): Observable<ConfigAudit[]> {
    this.loading.set(true);
    return this.http.get<{ success: boolean; data: ConfigAudit[]; message: string | null }>(`${environment.apiUrl}/config/audit`).pipe(
      map(response => response.data),
      tap({
        next: (logs) => {
          this.auditLogs.set(logs);
          this.loading.set(false);
        },
        error: () => {
          this.loading.set(false);
        }
      })
    );
  }

  // Método helper para obtener valor de configuración con valor por defecto
  getConfigValue<T>(key: string, defaultValue: T): T {
    const config = this.configs().find(c => c.configKey === key);
    if (!config) return defaultValue;

    switch (config.valueType) {
      case ConfigValueType.Int:
        return parseInt(config.configValue) as T;
      case ConfigValueType.Decimal:
        return parseFloat(config.configValue) as T;
      case ConfigValueType.Boolean:
        return (config.configValue.toLowerCase() === 'true') as T;
      default:
        return config.configValue as T;
    }
  }
}
