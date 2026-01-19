# Funcionalidades del Sistema

El **Student Enrollment System** ofrece una solución integral para la gestión académica, proporcionando interfaces especializadas para administradores y estudiantes, garantizando la seguridad y eficiencia en los procesos de matrícula y administración.

## Módulos Principales

### 1. Autenticación y Seguridad

El sistema implementa un robusto mecanismo de seguridad basado en JWT (JSON Web Tokens) y control de acceso basado en roles (RBAC).

*   **Inicio de Sesión (Login):** Acceso seguro para todos los usuarios con validación de credenciales.
*   **Auto-registro de Estudiantes:** Formulario público que permite a nuevos aspirantes crear su cuenta en el sistema.
*   **Gestión de Contraseñas:** Funcionalidad para la recuperación y restablecimiento de contraseñas olvidadas mediante correo electrónico.
*   **Verificación de Cuenta:** Confirmación de identidad a través de enlaces enviados al correo electrónico registrado.
*   **Protección de Rutas:** Uso de *Guards* en el frontend para restringir el acceso a módulos específicos según el rol del usuario (Admin/Estudiante).

### 2. Portal Administrativo

Diseñado para el personal de la institución, permite el control total sobre los datos académicos y la configuración del sistema.

*   **Dashboard de Control:** Vista general con métricas clave y estadísticas del sistema en tiempo real.
*   **Gestión de Usuarios:**
    *   **Estudiantes:** Administración de perfiles, visualización de estado académico y generación de credenciales.
    *   **Profesores:** Registro y mantenimiento de la información del cuerpo docente.
*   **Gestión Académica:**
    *   **Asignaturas:** Creación, edición y desactivación de materias, incluyendo la definición de créditos y cupos.
    *   **Programación:** Asignación de horarios y profesores a las asignaturas ofertadas.
*   **Configuración del Sistema:**
    *   Control de parámetros globales como límites de créditos por estudiante.
    *   Apertura y cierre de periodos de matrícula.

### 3. Portal del Estudiante

Interfaz intuitiva para que los alumnos gestionen su vida académica de manera autónoma.

*   **Matrícula en Línea:**
    *   Visualización de la oferta académica disponible.
    *   Inscripción en asignaturas en tiempo real con validación de cupos y cruces de horario.
    *   Confirmación inmediata de la matrícula.
*   **Mis Cursos:**
    *   Listado de asignaturas matriculadas en el periodo actual.
    *   Acceso a detalles de horarios y profesores asignados.
    *   Posibilidad de ver compañeros de clase (según configuración).
*   **Historial Académico:** Consulta del récord de notas y asignaturas cursadas en periodos anteriores.
*   **Perfil de Usuario:** Actualización de información personal y preferencias de cuenta.

## Características Técnicas Destacadas

*   **Validaciones en Tiempo Real:** Feedback inmediato al usuario durante la entrada de datos.
*   **Diseño Responsivo:** Interfaz adaptable a dispositivos de escritorio y móviles.
*   **Auditoría de Cambios:** Registro de modificaciones en configuraciones críticas del sistema.
*   **Generación de Códigos:** Asignación automática de códigos únicos estudiantiles mediante lógica de base de datos.