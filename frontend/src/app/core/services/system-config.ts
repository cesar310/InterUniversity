import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SystemConfig as SystemConfigModel } from '../models/system-config.model';

/**
 * Servicio para gestionar las configuraciones del sistema desde la base de datos.
 * Las configuraciones se cargan al inicio de la aplicación y se almacenan en signals.
 */
@Injectable({
  providedIn: 'root',
})
export class SystemConfigService {
  private http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/config`;

  // Signal que almacena todas las configuraciones
  private configsSignal = signal<SystemConfigModel[]>([]);
  
  // Signal que indica si las configuraciones han sido cargadas
  private loadedSignal = signal<boolean>(false);
  
  // Signal que indica si hay un error al cargar las configuraciones
  private errorSignal = signal<string | null>(null);

  // Computed signals para acceder a configuraciones específicas (numéricos)
  readonly maxSubjectsPerStudent = computed(() => {
    const config = this.getConfigByKey('max_subjects_per_student');
    return config ? parseInt(config.configValue) : 3; // Valor por defecto
  });

  readonly minSubjectsPerStudent = computed(() => {
    const config = this.getConfigByKey('min_subjects_per_student');
    return config ? parseInt(config.configValue) : 1; // Valor por defecto
  });

  readonly defaultSubjectCredits = computed(() => {
    const config = this.getConfigByKey('default_subject_credits');
    return config ? parseInt(config.configValue) : 3; // Valor por defecto
  });

  readonly maxSubjectsPerProfessor = computed(() => {
    const config = this.getConfigByKey('max_subjects_per_professor');
    return config ? parseInt(config.configValue) : 2; // Valor por defecto
  });

  // Computed signals para configuraciones booleanas
  readonly allowSameProfessor = computed(() => {
    const config = this.getConfigByKey('allow_same_professor');
    return config ? config.configValue.toLowerCase() === 'true' : false; // Valor por defecto
  });

  readonly enrollmentOpen = computed(() => {
    const config = this.getConfigByKey('enrollment_open');
    return config ? config.configValue.toLowerCase() === 'true' : true; // Valor por defecto
  });

  // Computed signals para configuraciones de texto
  readonly systemName = computed(() => {
    const config = this.getConfigByKey('system_name');
    return config ? config.configValue : 'Sistema de Inscripción Estudiantil'; // Valor por defecto
  });

  readonly academicPeriod = computed(() => {
    const config = this.getConfigByKey('academic_period');
    return config ? config.configValue : '2026-1'; // Valor por defecto
  });

  // Exponer signals como readonly
  readonly configs = this.configsSignal.asReadonly();
  readonly loaded = this.loadedSignal.asReadonly();
  readonly error = this.errorSignal.asReadonly();

  /**
   * Carga todas las configuraciones desde la API
   */
  async loadConfigurations(): Promise<void> {
    try {
      this.errorSignal.set(null);
      const response = await firstValueFrom(
        this.http.get<{ success: boolean; data: SystemConfigModel[]; message: string | null }>(this.apiUrl)
      );
      this.configsSignal.set(response.data || []);
      this.loadedSignal.set(true);
    } catch (error: any) {
      console.error('Error al cargar configuraciones del sistema:', error);
      this.errorSignal.set(error?.message || 'Error al cargar configuraciones');
      // Mantener valores por defecto si falla la carga
      this.configsSignal.set([]);
      this.loadedSignal.set(true);
    }
  }

  /**
   * Obtiene una configuración por su clave
   */
  getConfigByKey(key: string): SystemConfigModel | undefined {
    return this.configsSignal().find(config => config.configKey === key);
  }

  /**
   * Obtiene el valor de una configuración por su clave
   */
  getConfigValue(key: string): string | undefined {
    return this.getConfigByKey(key)?.configValue;
  }

  /**
   * Obtiene el valor numérico de una configuración
   */
  getConfigValueAsNumber(key: string, defaultValue: number = 0): number {
    const value = this.getConfigValue(key);
    return value ? parseInt(value) : defaultValue;
  }

  /**
   * Obtiene el valor booleano de una configuración
   */
  getConfigValueAsBoolean(key: string, defaultValue: boolean = false): boolean {
    const value = this.getConfigValue(key);
    return value ? value.toLowerCase() === 'true' : defaultValue;
  }

  /**
   * Actualiza una configuración
   */
  async updateConfig(key: string, value: string): Promise<void> {
    try {
      await firstValueFrom(
        this.http.patch<SystemConfigModel>(`${this.apiUrl}/${key}`, { value })
      );
      // Recargar configuraciones después de actualizar
      await this.loadConfigurations();
    } catch (error) {
      console.error('Error al actualizar configuración:', error);
      throw error;
    }
  }

  /**
   * Fuerza la recarga de configuraciones
   */
  async reloadConfigurations(): Promise<void> {
    this.loadedSignal.set(false);
    await this.loadConfigurations();
  }
}
