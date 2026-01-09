
You are an expert in TypeScript, Angular, and scalable web application development. You write functional, maintainable, performant, and accessible code following Angular and TypeScript best practices.

## 🚨 INSTRUCCIONES OBLIGATORIAS

### Angular CLI (Obligatorio)
- Para crear cualquier elemento de Angular (servicios, componentes, directivas, pipes, etc.) es **OBLIGATORIO** utilizar Angular CLI para mantener la coherencia del proyecto
- Usar comandos como: `ng generate component`, `ng generate service`, etc.
- **NUNCA** crear archivos manualmente si existe un comando de Angular CLI para hacerlo

### Documentación y Contexto (Obligatorio)
- Siempre que se va a realizar una tarea, se debe obtener información de la documentación oficial o herramientas MCP de Angular y PrimeNG
- Consultar los archivos ANGULAR-FRONTEND-CONTEXT.md y API-DOCUMENTATION.md para entender el contexto del proyecto
- Verificar endpoints y DTOs antes de implementar llamadas a la API

### Estilos y Componentes UI (Obligatorio)
- **NO se deben aplicar estilos CSS adicionales personalizados**
- Utilizar **únicamente componentes de PrimeNG** con estilo sobrio
- Adaptar los componentes usando **Tailwind CSS** para ajustes menores
- El proyecto ya tiene configurado PrimeNG y Tailwind, por lo que en términos de UI está listo para trabajar
- No crear archivos CSS personalizados ni aplicar estilos inline complejos

## 📋 Contexto del Proyecto

Este es un sistema de matrícula estudiantil desarrollado con Angular 21, PrimeNG y Tailwind CSS que consume una API REST en .NET 9.

### Entidades Principales
1. **Users**: Autenticación y autorización (administradores y estudiantes)
2. **Students**: Estudiantes del sistema (relación 1:1 con users)
3. **Professors**: Profesores (NO son usuarios del sistema)
4. **Subjects**: Materias (relaciónMany-to-One con professors)
5. **Enrollments**: Inscripciones (relación Many-to-Many entre students y subjects)
6. **System_Config**: Configuraciones dinámicas del sistema

### Reglas de Negocio Críticas
- **Materia activa**: Solo subjects con `is_active = TRUE`
- **Límite por estudiante**: No exceder `max_subjects_per_student` (configurable, por defecto 3)
- **No repetir profesor**: Un estudiante NO puede inscribir 2 materias del mismo profesor
- **No repetir materia**: Una materia por estudiante
- **Solo estado active**: Contar solo enrollments con `status = 'active'`
- **Límite de materias por profesor**: Un profesor no puede tener más de `max_subjects_per_professor` materias activas
- **NUNCA hardcodear** límites (siempre leer de system_config)

### API Backend
- **Base URL**: `http://localhost:5137/api/v1`
- **Autenticación**: JWT Bearer tokens
- **Endpoints principales**:
  - `/api/v1/auth/*` - Autenticación y registro
  - `/api/v1/students/*` - Gestión de estudiantes
  - `/api/v1/professors/*` - Gestión de profesores
  - `/api/v1/subjects/*` - Gestión de materias
  - `/api/v1/enrollments/*` - Gestión de inscripciones
  - `/api/v1/config/*` - Configuraciones del sistema

### Roles y Permisos
- **Administradores**: Gestionan todo (profesores, materias, configuraciones)
- **Estudiantes**: Solo ven y gestionan sus propias inscripciones
- **Profesores**: NO acceden al sistema (solo son entidades de datos)

Para más detalles, consultar:
- ANGULAR-FRONTEND-CONTEXT.md (Estructura completa del frontend)
- API-DOCUMENTATION.md (Documentación completa de la API)

## TypeScript Best Practices

- Use strict type checking
- Prefer type inference when the type is obvious
- Avoid the `any` type; use `unknown` when type is uncertain

## Angular Best Practices

- Always use standalone components over NgModules
- Must NOT set `standalone: true` inside Angular decorators. It's the default in Angular v20+.
- Use signals for state management
- **🚨 OBLIGATORIO: Usar Signal Forms** para todos los formularios
  - Importar `form()`, `Field` desde `@angular/forms/signals`
  - NO usar Reactive Forms tradicionales (FormControl, FormGroup)
  - Signal Forms son la forma moderna y recomendada en Angular 21+
- Implement lazy loading for feature routes
- Do NOT use the `@HostBinding` and `@HostListener` decorators. Put host bindings inside the `host` object of the `@Component` or `@Directive` decorator instead
- Use `NgOptimizedImage` for all static images.
  - `NgOptimizedImage` does not work for inline base64 images.

## Accessibility Requirements

- It MUST pass all AXE checks.
- It MUST follow all WCAG AA minimums, including focus management, color contrast, and ARIA attributes.

### Components

- Keep components small and focused on a single responsibility
- Use `input()` and `output()` functions instead of decorators
- Use `computed()` for derived state
- Set `changeDetection: ChangeDetectionStrategy.OnPush` in `@Component` decorator
- Prefer inline templates for small components
- **Use Signal Forms** for all forms (NOT Reactive or Template-driven)
- Do NOT use `ngClass`, use `class` bindings instead
- Do NOT use `ngStyle`, use `style` bindings instead
- When using external templates/styles, use paths relative to the component TS file.

## State Management

- Use signals for local component state
- Use `computed()` for derived state
- Keep state transformations pure and predictable
- Do NOT use `mutate` on signals, use `update` or `set` instead

## Templates

- Keep templates simple and avoid complex logic
- Use native control flow (`@if`, `@for`, `@switch`) instead of `*ngIf`, `*ngFor`, `*ngSwitch`
- Use the async pipe to handle observables
- Do not assume globals like (`new Date()`) are available.
- Do not write arrow functions in templates (they are not supported).

## Services

- Design services around a single responsibility
- Use the `providedIn: 'root'` option for singleton services
- Use the `inject()` function instead of constructor injection
