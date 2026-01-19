# Arquitectura y Stack Tecnológico

## Arquitectura del Sistema

El sistema **Student Enrollment System** ha sido diseñado siguiendo los principios de **Clean Architecture** y **Domain-Driven Design (DDD)**, garantizando una clara separación de responsabilidades y facilitando la evolución y mantenimiento del software. Asimismo, se implementa el patrón **CQRS (Command Query Responsibility Segregation)** para optimizar las operaciones de lectura y escritura.

### Estructura del Backend (.NET)

La solución se divide en cuatro capas principales, con dependencias dirigidas hacia el interior:

1.  **StudentEnrollment.Domain (Núcleo)**
    *   Contiene las entidades del dominio, enumeraciones, objetos de valor, excepciones de dominio e interfaces de repositorio.
    *   No posee dependencias de otras capas ni de frameworks externos.
    *   Define las reglas de negocio fundamentales.

2.  **StudentEnrollment.Application (Casos de Uso)**
    *   Implementa la lógica de aplicación y orquesta los flujos de trabajo.
    *   Utiliza **MediatR** para la implementación de CQRS (Commands y Queries).
    *   Define DTOs (Data Transfer Objects), validadores (FluentValidation) e interfaces de servicios.
    *   Depende únicamente de la capa de Dominio.

3.  **StudentEnrollment.Infrastructure (Infraestructura)**
    *   Provee la implementación de las interfaces definidas en Dominio y Aplicación.
    *   Gestiona el acceso a datos mediante **Entity Framework Core**.
    *   Implementa servicios externos (SMTP, almacenamiento de archivos, etc.).
    *   Maneja la autenticación y autorización (Identity).

4.  **StudentEnrollment.Api (Presentación)**
    *   Punto de entrada de la aplicación (REST API).
    *   Configura la inyección de dependencias y el pipeline de middleware.
    *   Expone los endpoints consumidos por el cliente frontend.
    *   Utiliza Swagger/OpenAPI para la documentación de la API.

### Arquitectura del Frontend (Angular)

La aplicación cliente está construida con **Angular 21**, adoptando una arquitectura modular basada en características (Feature-Based Architecture):

*   **Core Module:** Servicios singleton, interceptores HTTP, guards de seguridad y configuración global.
*   **Shared Module:** Componentes de UI reutilizables, directivas y pipes comunes.
*   **Feature Modules:** Módulos independientes para cada dominio funcional (Auth, Admin, Student), cargados mediante Lazy Loading para optimizar el rendimiento.
*   **State Management:** Gestión reactiva del estado utilizando **Angular Signals** y **RxJS**.

## Modelo de Datos

El diseño de la base de datos es relacional y normalizado. Se encuentra disponible un diagrama entidad-relación detallado para su consulta.

*   [Ver Diagrama Entidad-Relación (ER)](er-diagram.md)

## Stack Tecnológico

### Backend
*   **Framework:** .NET 10
*   **Lenguaje:** C# 13
*   **ORM:** Entity Framework Core 9.0.0
*   **Base de Datos:** MySQL 8.4.7
*   **Patrones:** CQRS, Mediator, Repository, Unit of Work
*   **Librerías Clave:**
    *   MediatR (Orquestación de mensajes)
    *   FluentValidation (Reglas de validación)
    *   Serilog (Logging estructurado)
    *   AutoMapper (Mapeo de objetos)
    *   Swashbuckle (Documentación de API)

### Frontend
*   **Framework:** Angular 21
*   **Lenguaje:** TypeScript 5.9
*   **Estilos:** Tailwind CSS 4
*   **Componentes UI:** PrimeNG 21
*   **Iconografía:** PrimeIcons
*   **Build Tool:** Angular CLI (ESBuild)

### DevOps y Herramientas
*   **Control de Versiones:** Git
*   **Contenedorización:** Docker (Soporte planificado)