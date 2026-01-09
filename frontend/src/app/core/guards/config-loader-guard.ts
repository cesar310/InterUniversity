import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { SystemConfigService } from '../services/system-config';

/**
 * Guard que asegura que las configuraciones del sistema estén cargadas
 * antes de permitir el acceso a una ruta.
 * 
 * Este guard debe aplicarse a las rutas principales (admin, student) para
 * garantizar que las reglas de negocio estén disponibles.
 */
export const configLoaderGuard: CanActivateFn = async (route, state) => {
  const configService = inject(SystemConfigService);
  
  // Si las configuraciones ya están cargadas, permitir acceso
  if (configService.loaded()) {
    return true;
  }
  
  // Si no están cargadas, cargarlas y luego permitir acceso
  try {
    await configService.loadConfigurations();
    return true;
  } catch (error) {
    console.error('Error al cargar configuraciones en guard:', error);
    // Permitir acceso incluso si falla, ya que el servicio usa valores por defecto
    return true;
  }
};
