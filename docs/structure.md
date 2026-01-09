# Estructura del Proyecto

A continuación se detalla la organización de archivos y carpetas del repositorio.

```plaintext
/
├── backend/                    # Solución .NET 10 (Backend)
│   ├── StudentEnrollment.Api/          # API REST, Controladores y Configuración
│   ├── StudentEnrollment.Application/  # Lógica de Negocio, Comandos y Consultas (CQRS)
│   ├── StudentEnrollment.Domain/       # Entidades del Dominio y Reglas de Negocio
│   └── StudentEnrollment.Infrastructure/ # Acceso a Datos (EF Core) y Servicios Externos
│
├── frontend/                   # Aplicación Angular 21 (Frontend)
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/           # Servicios singleton, guards e interceptores
│   │   │   ├── features/       # Módulos funcionales (admin, auth, student)
│   │   │   └── shared/         # Componentes y utilidades reutilizables
│   │   └── environments/       # Configuración de entorno (API URL, etc.)
│   └── angular.json            # Configuración de Angular CLI
│
├── database/                   # Scripts de Base de Datos
│   └── script.sql              # Script SQL inicial para estructura y datos semilla
│
├── docs/                       # Documentación del Proyecto
│   ├── installation.md         # Guía de instalación
│   ├── architecture.md         # Detalles de arquitectura y tecnología
│   ├── features.md             # Descripción funcional
│   └── structure.md            # Este archivo
│
└── README.md                   # Punto de entrada principal
```
