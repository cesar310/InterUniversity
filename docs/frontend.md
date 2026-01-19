# Guía de Desarrollo Frontend

Este documento orienta a los desarrolladores sobre la arquitectura y prácticas del cliente web del **Student Enrollment System**, desarrollado en **Angular 21**.

## Arquitectura Modular

La aplicación sigue una arquitectura basada en **Features** (Características), donde cada dominio funcional (Auth, Admin, Student) es un módulo autocontenido.

### Estructura de Directorios (`src/app`)

*   **`core/`**: Código esencial que se carga una sola vez.
    *   `guards/`: Protección de rutas (`auth.guard.ts`).
    *   `interceptors/`: Manipulación de HTTP (`jwt.interceptor.ts`).
    *   `services/`: Servicios singleton globales (`auth.service.ts`).
    *   `models/`: Interfaces TypeScript compartidas.
*   **`features/`**: Módulos de negocio (Lazy Loaded).
    *   Cada carpeta (`dashboard`, `enrollment`) contiene sus propios componentes, servicios locales y rutas.
*   **`shared/`**: Componentes reutilizables de UI, Pipes y Directivas.
*   **`layouts/`**: Estructuras de página (ej. `MainLayout` con Sidebar, `AuthLayout` limpio).

---

## Tecnologías y Patrones

### Componentes Standalone
El proyecto utiliza el enfoque **Standalone Components** de Angular, eliminando la necesidad de `NgModules` complejos. Cada componente importa directamente sus dependencias.

```typescript
@Component({
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonModule],
  // ...
})
export class LoginComponent {}
```

### Gestión de Estado
Se implementa un enfoque reactivo basado en **Services con Signals/BehaviorSubjects**.
*   Los servicios actúan como almacenes de estado para datos compartidos (ej. `CurrentUser`, `Cart`).
*   Los componentes consumen estos datos de manera reactiva.

### Diseño y UI
*   **Tailwind CSS:** Para maquetación, espaciado y tipografía utilitaria.
*   **PrimeNG:** Librería de componentes avanzados (Tablas, Modales, Inputs).

---

## Seguridad e Integración

### Autenticación (JWT)
1.  **Login:** El `AuthService` obtiene el token y lo almacena en `localStorage`.
2.  **Interceptor:** `JwtInterceptor` inyecta el header `Authorization: Bearer ...` en cada petición HTTP saliente.
3.  **Guards:**
    *   `AuthGuard`: Verifica si existe un token válido.
    *   `RoleGuard`: Verifica si el usuario posee el rol necesario.

### Comunicación con Backend
Todas las llamadas a la API se centralizan en servicios dentro de `core/services` o `features/.../services`. Se utilizan **Interfaces** estrictas para el tipado de respuestas.

```typescript
// Ejemplo de llamada tipada
getStudents(): Observable<Student[]> {
  return this.http.get<Student[]>(`${this.apiUrl}/students`);
}
```

---

## Guía: Crear una Nueva Feature

Para agregar una nueva sección (ej. "Reportes"):

1.  **Crear Directorio:** `src/app/features/reports`.
2.  **Rutas:** Definir `reports.routes.ts` con carga diferida (lazy loading).
3.  **Componentes:** Crear los componentes de vista (Page) y componentes menores.
4.  **Servicio:** Crear `reports.service.ts` para la lógica de datos específica.
5.  **Registro:** Agregar la ruta en `app.routes.ts`:
    ```typescript
    {
      path: 'reports',
      loadChildren: () => import('./features/reports/reports.routes').then(m => m.REPORTS_ROUTES)
    }
    ```