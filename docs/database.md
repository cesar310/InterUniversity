# Documentación de Base de Datos

Este documento describe en detalle la estructura y lógica implementada en la base de datos MySQL del **Student Enrollment System**.

## Visión General

La base de datos está diseñada para mantener la integridad referencial y ejecutar lógica de negocio crítica directamente en el motor de base de datos, asegurando consistencia independientemente de la aplicación que acceda a los datos.

*   **Motor:** MySQL 8.4.7
*   **Nombre por defecto:** `student_enrollment_db` o `student_enrollment_system`
*   **Diagrama ER:** [Ver Diagrama Entidad-Relación](./er-diagram.md)

---

## Tablas Principales

### Gestión de Usuarios y Roles
*   **`users`**: Almacena las credenciales, estado de verificación de email y tokens de recuperación. Es la tabla central de identidad.
*   **`roles`**: Define los roles del sistema (`administrator`, `student`).
*   **`user_roles`**: Tabla intermedia para la relación N:M entre usuarios y roles.

### Dominio Académico
*   **`students`**: Información específica del perfil de estudiante. Vinculada 1:1 con `users`.
*   **`professors`**: Información del cuerpo docente.
*   **`subjects`**: Catálogo de asignaturas, incluyendo créditos y profesor asignado.
*   **`enrollments`**: Registro de inscripciones de estudiantes en asignaturas. Maneja el estado (`active`, `dropped`).

### Configuración y Sistema
*   **`system_config`**: Almacenamiento Key-Value para parámetros globales del sistema (ej. límites de créditos).
*   **`config_audit_log`**: Historial de cambios realizados en la configuración del sistema (Trigger-based).
*   **`student_code_counters`**: Tabla auxiliar para la generación secuencial de códigos de estudiante.

---

## Lógica de Negocio (Triggers y Funciones)

El sistema utiliza triggers para automatizar procesos críticos y garantizar la integridad de los datos.

### Generación de Código Estudiantil
**Trigger:** `generate_student_code`
*   **Evento:** `BEFORE INSERT ON students`
*   **Lógica:** Genera automáticamente un código único para cada nuevo estudiante con el formato `YYYYNNNNN` (ej. `202400001`).
*   **Funcionamiento:**
    1.  Detecta el año actual.
    2.  Consulta y actualiza la tabla `student_code_counters` para obtener el siguiente secuencial del año.
    3.  Formatea el código y lo asigna antes de la inserción.

### Funciones Utilitarias
**Función:** `get_config_int(key_name)`
*   Retorna el valor entero de una configuración del sistema, facilitando su uso en vistas y procedimientos almacenados sin realizar subconsultas repetitivas.

---

## Vistas del Sistema

Las vistas simplifican consultas complejas y proveen una capa de abstracción para el backend y reportes.

| Vista | Descripción |
| :--- | :--- |
| **`view_academic_offer`** | Muestra las asignaturas activas, detalles del profesor y cupos disponibles. Utilizada para el catálogo de matrícula. |
| **`view_student_enrollments`** | Resumen del estado actual de cada estudiante: materias inscritas, créditos totales y validación contra el límite permitido. |
| **`view_professors`** | Detalle de la carga académica de los profesores y su disponibilidad (basada en `max_subjects_per_professor`). |
| **`view_current_config`** | Listado de configuraciones activas, mostrando quién realizó la última modificación. |
| **`view_config_audit`** | Historial de auditoría mostrando valores anteriores y nuevos de las configuraciones modificadas. |

---

## Parámetros de Configuración (`system_config`)

El comportamiento del sistema es dinámico y controlable mediante la tabla `system_config`.

| Clave (`config_key`) | Tipo | Descripción | Default |
| :--- | :--- | :--- | :--- |
| `max_subjects_per_student` | INT | Límite máximo de asignaturas por estudiante. | 3 |
| `min_subjects_per_student` | INT | Mínimo requerido para validar matrícula. | 1 |
| `max_subjects_per_professor` | INT | Carga máxima docente permitida. | 2 |
| `enrollment_open` | BOOL | Interruptor maestro para abrir/cerrar inscripciones. | `true` |
| `allow_same_professor` | BOOL | Permite inscribir varias materias con el mismo docente. | `false` |

---

## Procedimientos Almacenados

**`get_system_statistics`**
*   Provee un resumen ejecutivo para el Dashboard Administrativo.
*   **Retorna:** Total de estudiantes, profesores, asignaturas activas, y porcentaje de ocupación de cupos.
