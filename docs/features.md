# Características del Sistema

## Resumen Funcional
La plataforma ofrece portales dedicados para administradores y estudiantes, asegurando una experiencia de usuario fluida y adaptada a las necesidades de cada rol.

## Módulos Detallados

### Autenticación y Seguridad
-   **Registro de Usuarios (Self-Register):** Permite a los nuevos estudiantes crear su propia cuenta.
-   **Login:** Autenticación segura mediante JWT (JSON Web Tokens).
-   **Recuperación de Contraseña:** Flujo seguro para restablecer credenciales olvidadas vía correo electrónico.
-   **Verificación de Email:** Confirmación de identidad mediante enlace único enviado al correo.
-   **Guards:** Protección de rutas `authGuard`, `adminGuard`, `studentGuard` para asegurar que solo usuarios autorizados accedan a ciertas vistas.

### Portal Administrativo
Dirigido al personal de la universidad.
-   **Dashboard:** Visualización de estadísticas clave (total de estudiantes, inscripciones recientes, etc.).
-   **Gestión de Estudiantes:** ABM (Alta, Baja, Modificación) completo de estudiantes.
-   **Gestión de Profesores:** Administración de la planta docente.
-   **Gestión de Asignaturas:** Creación y edición de materias y cursos.
-   **Configuración del Sistema:** Auditoría de cambios y gestión de parámetros globales.

### Portal del Estudiante
Dirigido a los alumnos.
-   **Inscripción en Línea:** Catálogo de materias disponibles para el periodo actual.
-   **Mis Inscripciones:** Vista detallada de las materias en las que el estudiante está inscrito.
-   **Historial Académico:** (Futura implementación) Visualización de notas y progreso.
