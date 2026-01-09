ALTER DATABASE CHARACTER SET utf8mb4;


CREATE TABLE `roles` (
    `id` int NOT NULL AUTO_INCREMENT,
    `name` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `description` varchar(255) CHARACTER SET utf8mb4 NULL,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `PK_roles` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `users` (
    `id` int NOT NULL AUTO_INCREMENT,
    `email` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `password_hash` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `is_active` tinyint(1) NOT NULL DEFAULT TRUE,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT `PK_users` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `config_audit_log` (
    `id` int NOT NULL AUTO_INCREMENT,
    `config_key` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `old_value` varchar(255) CHARACTER SET utf8mb4 NULL,
    `new_value` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `changed_by` int NULL,
    `changed_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `PK_config_audit_log` PRIMARY KEY (`id`),
    CONSTRAINT `FK_config_audit_log_users_changed_by` FOREIGN KEY (`changed_by`) REFERENCES `users` (`id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;


CREATE TABLE `professors` (
    `id` int NOT NULL AUTO_INCREMENT,
    `name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `specialization` varchar(100) CHARACTER SET utf8mb4 NULL,
    `email` varchar(100) CHARACTER SET utf8mb4 NULL,
    `phone` varchar(20) CHARACTER SET utf8mb4 NULL,
    `is_active` tinyint(1) NOT NULL DEFAULT TRUE,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `created_by` int NULL,
    `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT `PK_professors` PRIMARY KEY (`id`),
    CONSTRAINT `FK_professors_users_created_by` FOREIGN KEY (`created_by`) REFERENCES `users` (`id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;


CREATE TABLE `students` (
    `id` int NOT NULL AUTO_INCREMENT,
    `user_id` int NOT NULL,
    `name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `student_code` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `created_by` int NULL,
    CONSTRAINT `PK_students` PRIMARY KEY (`id`),
    CONSTRAINT `FK_students_users_created_by` FOREIGN KEY (`created_by`) REFERENCES `users` (`id`) ON DELETE SET NULL,
    CONSTRAINT `FK_students_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `system_config` (
    `id` int NOT NULL AUTO_INCREMENT,
    `config_key` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `config_value` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `value_type` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
    `description` varchar(500) CHARACTER SET utf8mb4 NULL,
    `is_editable` tinyint(1) NOT NULL DEFAULT TRUE,
    `updated_by` int NULL,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT `PK_system_config` PRIMARY KEY (`id`),
    CONSTRAINT `FK_system_config_users_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `users` (`id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;


CREATE TABLE `user_roles` (
    `user_id` int NOT NULL,
    `role_id` int NOT NULL,
    `assigned_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `PK_user_roles` PRIMARY KEY (`user_id`, `role_id`),
    CONSTRAINT `FK_user_roles_roles_role_id` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`) ON DELETE CASCADE,
    CONSTRAINT `FK_user_roles_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `subjects` (
    `id` int NOT NULL AUTO_INCREMENT,
    `name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `credits` int NOT NULL DEFAULT 3,
    `professor_id` int NOT NULL,
    `is_active` tinyint(1) NOT NULL DEFAULT TRUE,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `PK_subjects` PRIMARY KEY (`id`),
    CONSTRAINT `FK_subjects_professors_professor_id` FOREIGN KEY (`professor_id`) REFERENCES `professors` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `enrollments` (
    `student_id` int NOT NULL,
    `subject_id` int NOT NULL,
    `status` varchar(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'active',
    `enrolled_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT `PK_enrollments` PRIMARY KEY (`student_id`, `subject_id`),
    CONSTRAINT `FK_enrollments_students_student_id` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`) ON DELETE CASCADE,
    CONSTRAINT `FK_enrollments_subjects_subject_id` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


INSERT INTO `roles` (`id`, `created_at`, `description`, `name`)
VALUES (1, TIMESTAMP '2026-01-08 02:57:25.901634', 'Gestiona configuración, profesores, materias', 'administrator'),
(2, TIMESTAMP '2026-01-08 02:57:25.901673', 'Se inscribe en materias', 'student');


INSERT INTO `system_config` (`id`, `config_key`, `config_value`, `created_at`, `description`, `is_editable`, `updated_at`, `updated_by`, `value_type`)
VALUES (1, 'max_subjects_per_student', '3', TIMESTAMP '2026-01-08 02:57:25.916', 'Máximo de materias que un estudiante puede inscribir simultáneamente', TRUE, TIMESTAMP '2026-01-08 02:57:25.916025', NULL, 'int'),
(2, 'max_subjects_per_professor', '3', TIMESTAMP '2026-01-08 02:57:25.916044', 'Máximo de materias que un profesor puede dictar', TRUE, TIMESTAMP '2026-01-08 02:57:25.916044', NULL, 'int'),
(3, 'default_credits', '3', TIMESTAMP '2026-01-08 02:57:25.916045', 'Créditos por defecto para nuevas materias', TRUE, TIMESTAMP '2026-01-08 02:57:25.916045', NULL, 'int'),
(4, 'system_mode', 'production', TIMESTAMP '2026-01-08 02:57:25.916045', 'Modo del sistema (production/maintenance)', TRUE, TIMESTAMP '2026-01-08 02:57:25.916045', NULL, 'string');


CREATE INDEX `IX_config_audit_log_changed_at` ON `config_audit_log` (`changed_at`);


CREATE INDEX `IX_config_audit_log_changed_by` ON `config_audit_log` (`changed_by`);


CREATE INDEX `IX_config_audit_log_config_key` ON `config_audit_log` (`config_key`);


CREATE INDEX `IX_enrollments_status` ON `enrollments` (`status`);


CREATE INDEX `IX_enrollments_subject_id` ON `enrollments` (`subject_id`);


CREATE INDEX `idx_active` ON `professors` (`is_active`);


CREATE INDEX `idx_name` ON `professors` (`name`);


CREATE INDEX `IX_professors_created_by` ON `professors` (`created_by`);


CREATE UNIQUE INDEX `IX_roles_name` ON `roles` (`name`);


CREATE INDEX `IX_students_created_by` ON `students` (`created_by`);


CREATE UNIQUE INDEX `IX_students_student_code` ON `students` (`student_code`);


CREATE UNIQUE INDEX `IX_students_user_id` ON `students` (`user_id`);


CREATE INDEX `idx_active` ON `subjects` (`is_active`);


CREATE INDEX `idx_professor` ON `subjects` (`professor_id`);


CREATE UNIQUE INDEX `IX_subjects_name` ON `subjects` (`name`);


CREATE UNIQUE INDEX `IX_system_config_config_key` ON `system_config` (`config_key`);


CREATE INDEX `IX_system_config_updated_by` ON `system_config` (`updated_by`);


CREATE INDEX `IX_user_roles_role_id` ON `user_roles` (`role_id`);


CREATE INDEX `idx_active` ON `users` (`is_active`);


CREATE UNIQUE INDEX `IX_users_email` ON `users` (`email`);


