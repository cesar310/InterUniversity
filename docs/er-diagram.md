# Diagrama ER del Sistema de Inscripción Estudiantil

```mermaid
erDiagram
    ROLES {
        INT id PK
        VARCHAR name "unique"
        VARCHAR description
        TIMESTAMP created_at
    }

    USERS {
        INT id PK
        VARCHAR email "unique"
        VARCHAR password_hash
        VARCHAR email_verification_token
        DATETIME email_verification_token_expiry
        BOOLEAN email_verified
        BOOLEAN must_change_password
        VARCHAR password_reset_token
        DATETIME password_reset_token_expiry
        BOOLEAN is_active
        TIMESTAMP created_at
        TIMESTAMP updated_at
    }

    USER_ROLES {
        INT user_id FK
        INT role_id FK
        TIMESTAMP assigned_at
        PK "user_id, role_id"
    }

    STUDENTS {
        INT id PK
        INT user_id FK "unique"
        VARCHAR name
        VARCHAR student_code "unique"
        TIMESTAMP created_at
        INT created_by FK
    }

    STUDENT_CODE_COUNTERS {
        INT year PK
        INT last_number
    }

    PROFESSORS {
        INT id PK
        VARCHAR name
        VARCHAR specialization
        VARCHAR email
        VARCHAR phone
        BOOLEAN is_active
        TIMESTAMP created_at
        INT created_by FK
        TIMESTAMP updated_at
    }

    SUBJECTS {
        INT id PK
        VARCHAR name "unique"
        TEXT description
        INT credits
        INT professor_id FK
        BOOLEAN is_active
        TIMESTAMP created_at
    }

    ENROLLMENTS {
        INT student_id FK
        INT subject_id FK
        VARCHAR status
        TIMESTAMP enrolled_at
        TIMESTAMP updated_at
        PK "student_id, subject_id"
    }

    SYSTEM_CONFIG {
        INT id PK
        VARCHAR config_key "unique"
        VARCHAR config_value
        VARCHAR value_type
        VARCHAR description
        BOOLEAN is_editable
        INT updated_by FK
        TIMESTAMP created_at
        TIMESTAMP updated_at
    }

    CONFIG_AUDIT_LOG {
        INT id PK
        VARCHAR config_key
        VARCHAR old_value
        VARCHAR new_value
        INT changed_by FK
        TIMESTAMP changed_at
    }

    USERS ||--o{ USER_ROLES : "assigned"
    ROLES ||--o{ USER_ROLES : "assigned"
    USERS ||--|| STUDENTS : "account"
    USERS ||--o{ STUDENTS : "created_by"
    USERS ||--o{ PROFESSORS : "created_by"
    PROFESSORS ||--o{ SUBJECTS : "teaches"
    STUDENTS ||--o{ ENROLLMENTS : "has"
    SUBJECTS ||--o{ ENROLLMENTS : "has"
    USERS ||--o{ SYSTEM_CONFIG : "updated_by"
    USERS ||--o{ CONFIG_AUDIT_LOG : "changed_by"
```

Notas:
- `ENROLLMENTS.status` y `SYSTEM_CONFIG.value_type` se representan como `VARCHAR` para alinear con EF Core (conversión de enums a minúsculas).
- `STUDENT_CODE_COUNTERS` es auxiliar para el trigger `generate_student_code` (no tiene FKs).
