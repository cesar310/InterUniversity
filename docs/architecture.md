# Arquitectura y Tecnologías

## Visión General de la Arquitectura

El **Student Enrollment System** está construido siguiendo los principios de **Clean Architecture** y **CQRS** (Command Query Responsibility Segregation) para asegurar la escalabilidad, mantenibilidad y testabilidad del código.

### Capas del Backend

1.  **StudentEnrollment.Domain**: El núcleo de la aplicación. Contiene las entidades, enumeraciones y lógica de negocio pura. No tiene dependencias externas.
2.  **StudentEnrollment.Application**: Contiene los casos de uso (Features), interfaces, y la implementación del patrón CQRS usando MediatR.
3.  **StudentEnrollment.Infrastructure**: Implementa las interfaces definidas en la capa de aplicación. Maneja la persistencia de datos (Entity Framework), servicios de correo, y otras integraciones externas.
4.  **StudentEnrollment.Api**: La capa de presentación. Expone los endpoints REST y maneja la configuración de inicio, inyección de dependencias y middlewares.

### Frontend
El frontend en Angular sigue una estructura modular basada en características (`features`), con un núcleo fuerte (`core`) para servicios singleton y utilidades compartidas (`shared`).

## Stack Tecnológico

### Backend
-   **Framework:** .NET 10 (C#)
-   **ORM:** Entity Framework Core
-   **Validación:** FluentValidation
-   **Mediator:** MediatR
-   **Logging:** Serilog (recomendado)

### Frontend
-   **Framework:** Angular 21
-   **Compilador:** ESBuild (Angular default)
-   **Estilos:** Tailwind CSS 4 + PrimeNG 21
-   **Gestión de Estado:** Angular Signals y RxJS
-   **Iconos:** PrimeIcons

### Base de Datos
-   **Motor:** MySQL 8.4.7
