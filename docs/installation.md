# Guía de Instalación y Despliegue

Este documento detalla los pasos necesarios para configurar y ejecutar el entorno de desarrollo local del **Student Enrollment System**.

## Requisitos Previos

Antes de comenzar, asegúrese de tener instaladas las siguientes herramientas en su sistema:

1.  **Base de Datos:**
    *   [MySQL Server](https://dev.mysql.com/downloads/mysql/) 8.4.7 o superior.
    *   Cliente SQL (opcional): MySQL Workbench, DBeaver o similar.

2.  **Backend:**
    *   [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

3.  **Frontend:**
    *   [Node.js](https://nodejs.org/) (Versión LTS, recomendada v18.13.0+).
    *   [Angular CLI](https://angular.io/cli) v21 (Instalar globalmente vía `npm install -g @angular/cli`).

4.  **Control de Versiones:**
    *   [Git](https://git-scm.com/).

---

## Procedimiento de Instalación

### 1. Configuración de la Base de Datos

El sistema requiere una base de datos MySQL. Se proporciona un script SQL para la creación del esquema y la carga de datos iniciales.

1.  Asegúrese de que el servicio de MySQL esté en ejecución.
2.  Acceda a su cliente de MySQL o terminal.
3.  Ejecute el script ubicado en `database/script.sql`.

**Comando desde terminal:**
```bash
mysql -u root -p < database/script.sql
```

> **Nota:** Este script creará la base de datos `student_enrollment_system` (o `student_enrollment_db` según configuración), definirá las tablas, vistas, procedimientos almacenados e insertará datos iniciales (roles, usuarios administradores, configuraciones).

### 2. Configuración del Backend (.NET API)

La API requiere configurar la cadena de conexión y otras credenciales sensibles.

1.  Navegue al directorio del proyecto de la API:
    ```bash
    cd backend/StudentEnrollment.Api
    ```

2.  Cree un archivo de configuración local llamado `appsettings.Local.json` para sobrescribir las configuraciones por defecto sin afectar el repositorio.

    **Contenido de `appsettings.Local.json`:**
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=localhost;Port=3306;Database=student_enrollment_system;User=root;Password=SU_PASSWORD_MYSQL;"
      },
      "JwtSettings": {
        "Secret": "ESTA_ES_UNA_CLAVE_SECRETA_MUY_SEGURA_PARA_DESARROLLO_LOCAL_MINIMO_32_CHARS",
        "Issuer": "StudentEnrollmentApi",
        "Audience": "StudentEnrollmentClient",
        "ExpiryMinutes": 60
      },
      "SmtpSettings": {
        "Host": "smtp.example.com",
        "Port": 587,
        "Username": "user@example.com",
        "Password": "password",
        "SenderEmail": "no-reply@university.edu"
      }
    }
    ```
    > **Importante:** Reemplace `SU_PASSWORD_MYSQL` con la contraseña de su usuario root de MySQL. Asegúrese de que el nombre de la base de datos coincida con el creado por el script.

3.  Restaure las dependencias y ejecute la aplicación:
    ```bash
    dotnet restore
    dotnet run
    ```

4.  Verifique que la API esté funcionando accediendo a la documentación Swagger:
    *   URL: `https://localhost:5001/swagger` (o el puerto indicado en la consola).

### 3. Configuración del Frontend (Angular)

1.  Navegue al directorio del frontend:
    ```bash
    cd frontend
    ```

2.  Instale las dependencias del proyecto:
    ```bash
    npm install
    ```

3.  (Opcional) Verifique la configuración de entorno en `src/environments/environment.ts` si necesita apuntar a un puerto de backend diferente.

4.  Inicie el servidor de desarrollo:
    ```bash
    ng serve
    ```

5.  Acceda a la aplicación web:
    *   URL: `http://localhost:4200`

---

## Solución de Problemas Comunes

*   **Error de conexión a base de datos:** Verifique que el servicio MySQL esté corriendo y que las credenciales en `appsettings.Local.json` sean correctas. Asegúrese de que el usuario tenga permisos sobre la base de datos.
*   **Error de CORS:** Si el frontend no puede comunicarse con el backend, verifique que la configuración de CORS en `Program.cs` del backend permita el origen `http://localhost:4200`.
*   **Versiones de Node/Angular:** Si encuentra errores de compatibilidad, asegúrese de usar las versiones especificadas en los requisitos previos.