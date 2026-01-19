# Estructura del Proyecto

La organización del código fuente sigue las mejores prácticas de desarrollo para asegurar mantenibilidad, escalabilidad y una clara separación de responsabilidades.

## Estructura General

El repositorio se divide en tres directorios principales que albergan los componentes clave del sistema:

*   `backend/`: Contiene la solución .NET con la API y la lógica de negocio.
*   `frontend/`: Alberga la aplicación web desarrollada en Angular.
*   `database/`: Scripts SQL para la creación y gestión de la base de datos.
*   `docs/`: Documentación técnica y funcional del proyecto.

---

## Backend (`/backend`)

El backend implementa una **Arquitectura Limpia (Clean Architecture)** distribuida en cuatro capas principales, contenidas en una solución .NET.

### Proyectos de la Solución

*   **StudentEnrollment.Api/**
    *   Punto de entrada de la aplicación (Web API).
    *   `Controllers/`: Controladores REST que manejan las solicitudes HTTP.
    *   `Program.cs`: Configuración de servicios, middleware y pipeline de la aplicación.
    *   `appsettings.json`: Configuraciones del entorno.

*   **StudentEnrollment.Application/**
    *   Contiene la lógica de negocio y casos de uso.
    *   `Features/`: Implementación del patrón CQRS (Comandos y Consultas) organizados por entidad (Students, Courses, Auth).
    *   `DTOs/`: Objetos de Transferencia de Datos para la comunicación entre capas.
    *   `Interfaces/`: Contratos para servicios e infraestructura.
    *   `Mappings/`: Perfiles de AutoMapper.

*   **StudentEnrollment.Domain/**
    *   El núcleo del sistema, libre de dependencias externas.
    *   `Entities/`: Definición de modelos de dominio (Student, Subject, Enrollment, etc.).
    *   `Enums/`: Enumeraciones para estados y tipos (Role, EnrollmentStatus).
    *   `Exceptions/`: Excepciones personalizadas del dominio.

*   **StudentEnrollment.Infrastructure/**
    *   Implementación de detalles técnicos y acceso a datos.
    *   `Data/`: Contexto de Entity Framework Core (`ApplicationDbContext`).
    *   `Repositories/`: Implementación de repositorios para acceso a datos.
    *   `Services/`: Servicios externos como envío de correos (EmailService) o generación de tokens (JwtTokenGenerator).

---

## Frontend (`/frontend`)

La aplicación cliente está construida con **Angular**, siguiendo una arquitectura modular basada en características (Feature-Based Architecture).

### Directorios Clave (`src/app`)

*   **core/**
    *   Servicios singleton y componentes fundamentales.
    *   `guards/`: Guardias de ruta para seguridad (`auth.guard`, `admin.guard`).
    *   `interceptors/`: Interceptores HTTP para manejo de tokens y errores (`jwt.interceptor`).
    *   `services/`: Servicios de comunicación con la API (`auth.service`, `api.service`).
    *   `models/`: Interfaces TypeScript que reflejan los DTOs del backend.

*   **features/**
    *   Módulos funcionales de la aplicación, cargados frecuentemente mediante Lazy Loading.
    *   `auth/`: Componentes de login, registro y recuperación de contraseña.
    *   `dashboard/`: Vista principal para administradores.
    *   `enrollment/`: Flujos de inscripción para estudiantes.
    *   `admin/`: Gestión de usuarios, asignaturas y configuraciones.

*   **shared/**
    *   Componentes, directivas y pipes reutilizables en toda la aplicación.
    *   Componentes UI comunes (tablas, modales, botones personalizados).

*   **environments/**
    *   Archivos de configuración para diferentes entornos (desarrollo, producción).

---

## Base de Datos (`/database`)

*   `script.sql`: Script maestro que contiene:
    *   Definición del esquema (DDL): Tablas, Vistas, Índices.
    *   Lógica de base de datos: Procedimientos Almacenados, Triggers, Funciones.
    *   Datos semilla (Seed Data): Roles iniciales, usuario administrador por defecto, configuración base.
