# Guía de Desarrollo Backend

Este documento sirve como referencia técnica para desarrolladores que trabajan en la API del **Student Enrollment System**. El backend está construido sobre **.NET 10** siguiendo los principios de **Clean Architecture** y **CQRS**.

## Arquitectura y Patrones

### Clean Architecture
La solución está dividida en capas concéntricas, donde las dependencias fluyen hacia adentro.

1.  **Domain (Núcleo):** Contiene Entidades, Enums y Excepciones. No tiene dependencias de otros proyectos.
2.  **Application (Capa de Servicio):** Define *qué* hace el sistema. Contiene los casos de uso (Commands/Queries), Interfaces, DTOs y Validaciones.
3.  **Infrastructure (Implementación):** Implementa las interfaces de Application. Contiene EF Core, Repositorios, y servicios externos (Email, Auth).
4.  **Api (Presentación):** Punto de entrada REST. Solo se encarga de recibir peticiones y delegarlas a la capa Application.

### CQRS (Command Query Responsibility Segregation)
Se utiliza la librería **MediatR** para desacoplar la ejecución de la lógica.

*   **Commands:** Modifican el estado del sistema (Create, Update, Delete). Retornan `Result<T>` o `Unit`.
*   **Queries:** Leen datos sin efectos secundarios. Retornan DTOs.

---

## Flujo de una Petición

Cuando llega una solicitud HTTP (ej. `POST /api/students`), el flujo es el siguiente:

1.  **Controller:** Recibe el Request DTO.
2.  **Mediator:** Envía el comando correspondiente (ej. `CreateStudentCommand`).
3.  **Pipeline Behavior (Validación):** Antes de llegar al Handler, **FluentValidation** intercepta el comando. Si falla, lanza una `ValidationException` automática.
4.  **Handler:** Ejecuta la lógica de negocio (ej. `CreateStudentHandler`). Interactúa con los Repositorios del Dominio.
5.  **Repository:** Realiza la operación en base de datos.
6.  **Response:** El Handler retorna el resultado, y el Controller lo transforma en un `IActionResult` (200 OK, 400 Bad Request, etc.).

---

## Componentes Clave

### Validación
Se utiliza **FluentValidation**. Cada comando debe tener su validador correspondiente en la misma carpeta.

```csharp
public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).EmailAddress();
    }
}
```

### Manejo de Errores
Existe un **Middleware Global de Excepciones** que captura errores no controlados y los transforma en respuestas JSON estandarizadas (RFC 7807 Problem Details), ocultando detalles sensibles en producción.

### Autenticación y Autorización
*   **JWT:** Los tokens se generan en `Infrastructure` y se validan en el middleware de .NET.
*   **Políticas:** Se definen en `Program.cs` y se aplican mediante atributos `[Authorize(Roles = "Administrator")]`.

---

## Guía: Añadir una Nueva Funcionalidad

Para agregar un nuevo caso de uso (ej. "Aprobar Matrícula"), siga estos pasos:

1.  **Domain:** ¿Necesita una nueva Entidad o método en una existente?
2.  **Application (Command/Query):**
    *   Cree la clase `ApproveEnrollmentCommand` (record record).
    *   Cree el `ApproveEnrollmentHandler`.
    *   Cree el validador `ApproveEnrollmentValidator`.
3.  **Infrastructure:** ¿Necesita un nuevo método en el Repositorio?
4.  **Api:** Agregue el endpoint en el Controller correspondiente llamando a `_mediator.Send()`.

## Estándares de Código

*   **Nombres:** PascalCase para clases y métodos, camelCase para variables locales.
*   **Async:** Todo el I/O debe ser asíncrono (`await`, `CancellationToken`).
*   **Records:** Use `record` para DTOs y Comandos (inmutabilidad).
*   **Result Pattern:** Evite lanzar excepciones para flujo de control. Use un objeto `Result` o similar para operaciones fallidas predecibles.