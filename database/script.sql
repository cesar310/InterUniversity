CREATE DATABASE IF NOT EXISTS student_enrollment_db;
USE student_enrollment_db;

CREATE TABLE roles (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(50) UNIQUE NOT NULL,
    description VARCHAR(255),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE users (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    email_verification_token VARCHAR(100) NULL,
    email_verification_token_expiry DATETIME NULL,
    email_verified TINYINT(1) NOT NULL DEFAULT 0,
    must_change_password TINYINT(1) NOT NULL DEFAULT 0,
    password_reset_token VARCHAR(100) NULL,
    password_reset_token_expiry DATETIME NULL,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_email (email),
    INDEX idx_active (is_active),
    INDEX idx_email_verification_token (email_verification_token),
    INDEX idx_password_reset_token (password_reset_token)
);

CREATE TABLE user_roles (
    user_id INT UNSIGNED NOT NULL,
    role_id INT UNSIGNED NOT NULL,
    assigned_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, role_id),
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (role_id) REFERENCES roles(id) ON DELETE CASCADE
);

CREATE TABLE students (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    user_id INT UNSIGNED UNIQUE NOT NULL,
    name VARCHAR(100) NOT NULL,
    student_code VARCHAR(20) UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by INT UNSIGNED,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (created_by) REFERENCES users(id) ON DELETE SET NULL,
    INDEX idx_student_code (student_code)
);

CREATE TABLE student_code_counters (
    year INT PRIMARY KEY,
    last_number INT DEFAULT 0
);

DELIMITER //

CREATE TRIGGER generate_student_code BEFORE INSERT ON students
 FOR EACH ROW
 BEGIN
     DECLARE current_year INT;
     DECLARE next_number INT;
     
     IF NEW.student_code IS NULL OR NEW.student_code = '' THEN
         SET current_year = YEAR(NOW());
         
         -- Insertar el año si no existe
         INSERT IGNORE INTO student_code_counters (year, last_number) VALUES (current_year, 0);
         
         -- Incrementar el contador y obtener el siguiente número
         UPDATE student_code_counters 
         SET last_number = last_number + 1 
         WHERE year = current_year;
         
         -- Obtener el número asignado
         SELECT last_number INTO next_number 
         FROM student_code_counters 
         WHERE year = current_year;
         
         -- Generar el código: YYYYNNNNN
         SET NEW.student_code = CONCAT(current_year, LPAD(next_number - 1, 5, '0'));
     END IF;
 END//

DELIMITER ;

CREATE TABLE professors (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    specialization VARCHAR(100),
    email VARCHAR(100),
    phone VARCHAR(20),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by INT UNSIGNED,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (created_by) REFERENCES users(id) ON DELETE SET NULL,
    INDEX idx_name (name),
    INDEX idx_active (is_active)
);

CREATE TABLE subjects (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) UNIQUE NOT NULL,
    description TEXT NULL,
    credits INT NOT NULL DEFAULT 3,
    professor_id INT UNSIGNED NOT NULL,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (professor_id) REFERENCES professors(id),
    INDEX idx_professor (professor_id),
    INDEX idx_active (is_active)
);

CREATE TABLE enrollments (
    student_id INT UNSIGNED NOT NULL,
    subject_id INT UNSIGNED NOT NULL,
    status ENUM('active', 'completed', 'cancelled') DEFAULT 'active',
    enrolled_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (student_id, subject_id),
    FOREIGN KEY (student_id) REFERENCES students(id) ON DELETE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES subjects(id) ON DELETE CASCADE,
    INDEX idx_status (status)
);

CREATE TABLE system_config (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    config_key VARCHAR(100) UNIQUE NOT NULL,
    config_value VARCHAR(255) NOT NULL,
    value_type ENUM('int', 'string', 'boolean', 'decimal') NOT NULL,
    description VARCHAR(500),
    is_editable BOOLEAN DEFAULT TRUE,
    updated_by INT UNSIGNED,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (updated_by) REFERENCES users(id) ON DELETE SET NULL,
    INDEX idx_key (config_key)
);

CREATE TABLE config_audit_log (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    config_key VARCHAR(100) NOT NULL,
    old_value VARCHAR(255),
    new_value VARCHAR(255) NOT NULL,
    changed_by INT UNSIGNED,
    changed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (changed_by) REFERENCES users(id) ON DELETE SET NULL,
    INDEX idx_config_key (config_key),
    INDEX idx_changed_at (changed_at)
);

DELIMITER //

CREATE FUNCTION get_config_int(p_key VARCHAR(100)) 
RETURNS INT
DETERMINISTIC
READS SQL DATA
BEGIN
    DECLARE v_value INT;
    SELECT CAST(config_value AS SIGNED) INTO v_value 
    FROM system_config 
    WHERE config_key = p_key;
    RETURN v_value;
END//

CREATE PROCEDURE get_system_statistics()
BEGIN
    SELECT 
        (SELECT COUNT(*) FROM students) AS total_students,
        (SELECT COUNT(*) FROM professors) AS total_professors,
        (SELECT COUNT(*) FROM subjects WHERE is_active = TRUE) AS active_subjects,
        (SELECT COUNT(*) FROM enrollments WHERE status = 'active') AS active_enrollments,
        (SELECT AVG(enrollment_count) FROM (
            SELECT COUNT(*) as enrollment_count 
            FROM enrollments 
            WHERE status = 'active'
            GROUP BY student_id
        ) AS avg_calc) AS avg_subjects_per_student,
        (SELECT MAX(enrollment_count) FROM (
            SELECT COUNT(*) as enrollment_count 
            FROM enrollments 
            WHERE status = 'active'
            GROUP BY student_id
        ) AS max_calc) AS max_subjects_enrolled;
END//

-- Trigger para auditoría de cambios en configuraciones
CREATE TRIGGER audit_config_update 
AFTER UPDATE ON system_config
FOR EACH ROW
BEGIN
    IF OLD.config_value != NEW.config_value THEN
        INSERT INTO config_audit_log (config_key, old_value, new_value, changed_by, changed_at)
        VALUES (NEW.config_key, OLD.config_value, NEW.config_value, NEW.updated_by, NEW.updated_at);
    END IF;
END//

DELIMITER ;

CREATE VIEW view_classmates AS
SELECT 
    s.name AS subject_name,
    st.name AS student_name
FROM enrollments e
JOIN subjects s ON e.subject_id = s.id
JOIN students st ON e.student_id = st.id
WHERE e.status = 'active'
ORDER BY s.name, st.name;

CREATE VIEW view_academic_offer AS
SELECT 
    s.id AS subject_id, 
    s.name AS subject,
    s.description,
    s.credits, 
    p.name AS professor,
    p.specialization,
    p.email AS professor_email,
    COUNT(e.student_id) AS enrolled_students,
    s.is_active AS available
FROM subjects s
JOIN professors p ON s.professor_id = p.id
LEFT JOIN enrollments e ON s.id = e.subject_id AND e.status = 'active'
WHERE p.is_active = TRUE
GROUP BY s.id, s.name, s.description, s.credits, p.name, p.specialization, p.email, s.is_active
ORDER BY s.name;

CREATE VIEW view_current_config AS
SELECT 
    config_key,
    config_value,
    value_type,
    description,
    is_editable,
    u.email AS last_updated_by,
    sc.updated_at
FROM system_config sc
LEFT JOIN users u ON sc.updated_by = u.id
ORDER BY config_key;

CREATE VIEW view_student_enrollments AS
SELECT 
    st.id AS student_id,
    st.name AS student_name,
    st.student_code,
    u.email,
    u.is_active,
    COUNT(e.subject_id) AS enrolled_subjects,
    get_config_int('max_subjects_per_student') AS max_allowed,
    GROUP_CONCAT(s.name SEPARATOR ', ') AS subjects
FROM students st
JOIN users u ON st.user_id = u.id
LEFT JOIN enrollments e ON st.id = e.student_id AND e.status = 'active'
LEFT JOIN subjects s ON e.subject_id = s.id
GROUP BY st.id, st.name, st.student_code, u.email, u.is_active;

CREATE VIEW view_professors AS
SELECT 
    p.id,
    p.name,
    p.specialization,
    p.email,
    p.phone,
    p.is_active,
    COUNT(s.id) AS total_subjects,
    get_config_int('max_subjects_per_professor') AS max_allowed,
    CASE 
        WHEN COUNT(s.id) >= get_config_int('max_subjects_per_professor') 
        THEN 'Completo' 
        ELSE 'Disponible' 
    END AS status
FROM professors p
LEFT JOIN subjects s ON p.id = s.professor_id AND s.is_active = TRUE
GROUP BY p.id, p.name, p.specialization, p.email, p.phone, p.is_active;

CREATE VIEW view_config_audit AS
SELECT 
    cal.id,
    cal.config_key,
    cal.old_value,
    cal.new_value,
    u.email AS changed_by,
    cal.changed_at
FROM config_audit_log cal
LEFT JOIN users u ON cal.changed_by = u.id
ORDER BY cal.changed_at DESC;

INSERT INTO roles (name, description) VALUES 
('administrator', 'Administrador del sistema con permisos completos'),
('student', 'Estudiante que puede inscribirse en materias');

INSERT INTO system_config (config_key, config_value, value_type, description, is_editable) VALUES 
('max_subjects_per_student', '3', 'int', 'Máximo de materias que un estudiante puede inscribir', TRUE),
('min_subjects_per_student', '1', 'int', 'Mínimo de materias que un estudiante debe inscribir', TRUE),
('default_subject_credits', '3', 'int', 'Créditos predeterminados por materia', TRUE),
('max_subjects_per_professor', '2', 'int', 'Máximo de materias que puede impartir un profesor', TRUE),
('allow_same_professor', 'false', 'boolean', 'Permitir que un estudiante tome múltiples materias del mismo profesor', TRUE),
('system_name', 'Sistema de Inscripción Estudiantil', 'string', 'Nombre del sistema', TRUE),
('academic_period', '2026-1', 'string', 'Período académico actual', TRUE),
('enrollment_open', 'true', 'boolean', 'Indica si las inscripciones están abiertas', TRUE),
('system_version', '2.0', 'string', 'Versión del sistema', FALSE),
('database_version', '1.0', 'string', 'Versión del esquema de base de datos', FALSE);

INSERT INTO users (email, password_hash, is_active, email_verified, must_change_password) VALUES
('admin@sistema.com', '$2a$11$xI9iX4u2ryZqDJ3zdE31AedRtAAg.MuH3C08Xt9zZ9JCJ33X1xPXm', TRUE, TRUE, FALSE);

SET @admin_user_id = LAST_INSERT_ID();

INSERT INTO user_roles (user_id, role_id) 
SELECT @admin_user_id, id FROM roles WHERE name = 'administrator';

INSERT INTO professors (name, specialization, email, created_by) VALUES 
('Prof. Alan Turing', 'Ciencias de la Computación - Teoría de Algoritmos', 'alan.turing@universidad.edu', @admin_user_id),
('Prof. Edgar Codd', 'Ingeniería de Software - Bases de Datos', 'edgar.codd@universidad.edu', @admin_user_id),
('Prof. Tim Berners-Lee', 'Ingeniería Web - Tecnologías Web', 'tim.berners@universidad.edu', @admin_user_id),
('Prof. Linus Torvalds', 'Ingeniería de Sistemas - Sistemas y Redes', 'linus.torvalds@universidad.edu', @admin_user_id),
('Prof. Grace Hopper', 'Ingeniería de Software - Arquitectura y Diseño', 'grace.hopper@universidad.edu', @admin_user_id);

INSERT INTO subjects (name, description, credits, professor_id, is_active) VALUES 
('Fundamentos de Programación', 'Introducción a la programación utilizando conceptos básicos: variables, tipos de datos, estructuras de control, funciones y resolución de problemas algorítmicos.', 3, (SELECT id FROM professors WHERE name = 'Prof. Alan Turing'), TRUE),
('Estructura de Datos y Algoritmos', 'Estudio de estructuras de datos fundamentales y algoritmos eficientes para su manipulación. Análisis de complejidad temporal y espacial.', 3, (SELECT id FROM professors WHERE name = 'Prof. Alan Turing'), TRUE),
('Bases de Datos Relacionales', 'Diseño y modelado de bases de datos relacionales. Normalización, SQL, integridad referencial y optimización de consultas.', 3, (SELECT id FROM professors WHERE name = 'Prof. Edgar Codd'), TRUE),
('Administración de Bases de Datos', 'Gestión avanzada de sistemas de bases de datos: configuración, respaldos, seguridad y monitoreo de rendimiento.', 3, (SELECT id FROM professors WHERE name = 'Prof. Edgar Codd'), TRUE),
('Desarrollo Web Frontend', 'Construcción de interfaces modernas y responsivas utilizando HTML5, CSS3 y JavaScript con frameworks modernos.', 3, (SELECT id FROM professors WHERE name = 'Prof. Tim Berners-Lee'), TRUE),
('Desarrollo Web Backend', 'Desarrollo de servicios web y APIs RESTful. Arquitectura servidor-cliente, autenticación y seguridad.', 3, (SELECT id FROM professors WHERE name = 'Prof. Tim Berners-Lee'), TRUE),
('Sistemas Operativos', 'Principios de sistemas operativos: gestión de procesos, hilos, memoria, sistemas de archivos y concurrencia.', 3, (SELECT id FROM professors WHERE name = 'Prof. Linus Torvalds'), TRUE),
('Redes de Computadoras', 'Arquitectura de redes y el modelo OSI/TCP-IP. Protocolos, enrutamiento, direccionamiento IP y seguridad.', 3, (SELECT id FROM professors WHERE name = 'Prof. Linus Torvalds'), TRUE),
('Ingeniería de Software', 'Metodologías de desarrollo (Ágil, Scrum), ciclo de vida del software, gestión de requisitos y pruebas.', 3, (SELECT id FROM professors WHERE name = 'Prof. Grace Hopper'), TRUE),
('Arquitectura de Software', 'Patrones de diseño arquitectónico, principios SOLID, arquitecturas escalables y documentación técnica.', 3, (SELECT id FROM professors WHERE name = 'Prof. Grace Hopper'), TRUE);

INSERT INTO users (email, password_hash, is_active, email_verified) VALUES
('maria.garcia@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('carlos.rodriguez@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('ana.martinez@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('luis.fernandez@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('laura.lopez@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('pedro.gonzalez@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('sofia.sanchez@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('diego.ramirez@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('valentina.torres@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('andres.flores@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('camila.morales@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('miguel.jimenez@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('isabella.ruiz@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('daniel.castro@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('martina.herrera@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('santiago.mendoza@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('emilia.vargas@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('nicolas.silva@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('valeria.ortiz@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('sebastian.reyes@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('lucia.romero@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('mateo.navarro@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('paula.gutierrez@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('gabriel.cruz@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE),
('adriana.diaz@estudiante.edu', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/lewKV.GfpJy8kSNcu', TRUE, TRUE);

INSERT INTO students (user_id, name, student_code, created_by) VALUES 
((SELECT id FROM users WHERE email = 'maria.garcia@estudiante.edu'), 'María García Pérez', '20260001', @admin_user_id),
((SELECT id FROM users WHERE email = 'carlos.rodriguez@estudiante.edu'), 'Carlos Rodríguez Sánchez', '20260002', @admin_user_id),
((SELECT id FROM users WHERE email = 'ana.martinez@estudiante.edu'), 'Ana Martínez López', '20260003', @admin_user_id),
((SELECT id FROM users WHERE email = 'luis.fernandez@estudiante.edu'), 'Luis Fernández García', '20260004', @admin_user_id),
((SELECT id FROM users WHERE email = 'laura.lopez@estudiante.edu'), 'Laura López Martínez', '20260005', @admin_user_id),
((SELECT id FROM users WHERE email = 'pedro.gonzalez@estudiante.edu'), 'Pedro González Rodríguez', '20260006', @admin_user_id),
((SELECT id FROM users WHERE email = 'sofia.sanchez@estudiante.edu'), 'Sofía Sánchez Fernández', '20260007', @admin_user_id),
((SELECT id FROM users WHERE email = 'diego.ramirez@estudiante.edu'), 'Diego Ramírez López', '20260008', @admin_user_id),
((SELECT id FROM users WHERE email = 'valentina.torres@estudiante.edu'), 'Valentina Torres González', '20260009', @admin_user_id),
((SELECT id FROM users WHERE email = 'andres.flores@estudiante.edu'), 'Andrés Flores Sánchez', '20260010', @admin_user_id),
((SELECT id FROM users WHERE email = 'camila.morales@estudiante.edu'), 'Camila Morales Ramírez', '20260011', @admin_user_id),
((SELECT id FROM users WHERE email = 'miguel.jimenez@estudiante.edu'), 'Miguel Jiménez Torres', '20260012', @admin_user_id),
((SELECT id FROM users WHERE email = 'isabella.ruiz@estudiante.edu'), 'Isabella Ruiz Flores', '20260013', @admin_user_id),
((SELECT id FROM users WHERE email = 'daniel.castro@estudiante.edu'), 'Daniel Castro Morales', '20260014', @admin_user_id),
((SELECT id FROM users WHERE email = 'martina.herrera@estudiante.edu'), 'Martina Herrera Jiménez', '20260015', @admin_user_id),
((SELECT id FROM users WHERE email = 'santiago.mendoza@estudiante.edu'), 'Santiago Mendoza Ruiz', '20260016', @admin_user_id),
((SELECT id FROM users WHERE email = 'emilia.vargas@estudiante.edu'), 'Emilia Vargas Castro', '20260017', @admin_user_id),
((SELECT id FROM users WHERE email = 'nicolas.silva@estudiante.edu'), 'Nicolás Silva Herrera', '20260018', @admin_user_id),
((SELECT id FROM users WHERE email = 'valeria.ortiz@estudiante.edu'), 'Valeria Ortiz Mendoza', '20260019', @admin_user_id),
((SELECT id FROM users WHERE email = 'sebastian.reyes@estudiante.edu'), 'Sebastián Reyes Vargas', '20260020', @admin_user_id),
((SELECT id FROM users WHERE email = 'lucia.romero@estudiante.edu'), 'Lucía Romero Silva', '20260021', @admin_user_id),
((SELECT id FROM users WHERE email = 'mateo.navarro@estudiante.edu'), 'Mateo Navarro Ortiz', '20260022', @admin_user_id),
((SELECT id FROM users WHERE email = 'paula.gutierrez@estudiante.edu'), 'Paula Gutiérrez Reyes', '20260023', @admin_user_id),
((SELECT id FROM users WHERE email = 'gabriel.cruz@estudiante.edu'), 'Gabriel Cruz Romero', '20260024', @admin_user_id),
((SELECT id FROM users WHERE email = 'adriana.diaz@estudiante.edu'), 'Adriana Díaz Navarro', '20260025', @admin_user_id);

INSERT INTO user_roles (user_id, role_id)
SELECT u.id, r.id 
FROM users u
CROSS JOIN roles r
WHERE u.email LIKE '%@estudiante.edu' AND r.name = 'student';

SET @fund_prog = (SELECT id FROM subjects WHERE name = 'Fundamentos de Programación');
SET @estr_datos = (SELECT id FROM subjects WHERE name = 'Estructura de Datos y Algoritmos');
SET @bd_rel = (SELECT id FROM subjects WHERE name = 'Bases de Datos Relacionales');
SET @admin_bd = (SELECT id FROM subjects WHERE name = 'Administración de Bases de Datos');
SET @web_front = (SELECT id FROM subjects WHERE name = 'Desarrollo Web Frontend');
SET @web_back = (SELECT id FROM subjects WHERE name = 'Desarrollo Web Backend');
SET @sistemas = (SELECT id FROM subjects WHERE name = 'Sistemas Operativos');
SET @redes = (SELECT id FROM subjects WHERE name = 'Redes de Computadoras');
SET @ing_soft = (SELECT id FROM subjects WHERE name = 'Ingeniería de Software');
SET @arq_soft = (SELECT id FROM subjects WHERE name = 'Arquitectura de Software');

INSERT INTO enrollments (student_id, subject_id) VALUES
((SELECT id FROM students WHERE name = 'María García Pérez'), @fund_prog),
((SELECT id FROM students WHERE name = 'María García Pérez'), @estr_datos),
((SELECT id FROM students WHERE name = 'María García Pérez'), @web_front),
((SELECT id FROM students WHERE name = 'Carlos Rodríguez Sánchez'), @fund_prog),
((SELECT id FROM students WHERE name = 'Carlos Rodríguez Sánchez'), @web_front),
((SELECT id FROM students WHERE name = 'Carlos Rodríguez Sánchez'), @web_back),
((SELECT id FROM students WHERE name = 'Ana Martínez López'), @fund_prog),
((SELECT id FROM students WHERE name = 'Ana Martínez López'), @estr_datos),
((SELECT id FROM students WHERE name = 'Ana Martínez López'), @bd_rel),
((SELECT id FROM students WHERE name = 'Luis Fernández García'), @estr_datos),
((SELECT id FROM students WHERE name = 'Luis Fernández García'), @web_front),
((SELECT id FROM students WHERE name = 'Luis Fernández García'), @web_back),
((SELECT id FROM students WHERE name = 'Laura López Martínez'), @fund_prog),
((SELECT id FROM students WHERE name = 'Laura López Martínez'), @bd_rel),
((SELECT id FROM students WHERE name = 'Laura López Martínez'), @admin_bd),
((SELECT id FROM students WHERE name = 'Pedro González Rodríguez'), @fund_prog),
((SELECT id FROM students WHERE name = 'Pedro González Rodríguez'), @estr_datos),
((SELECT id FROM students WHERE name = 'Pedro González Rodríguez'), @web_front),
((SELECT id FROM students WHERE name = 'Sofía Sánchez Fernández'), @fund_prog),
((SELECT id FROM students WHERE name = 'Sofía Sánchez Fernández'), @web_front),
((SELECT id FROM students WHERE name = 'Sofía Sánchez Fernández'), @bd_rel),
((SELECT id FROM students WHERE name = 'Diego Ramírez López'), @estr_datos),
((SELECT id FROM students WHERE name = 'Diego Ramírez López'), @sistemas),
((SELECT id FROM students WHERE name = 'Diego Ramírez López'), @redes),
((SELECT id FROM students WHERE name = 'Valentina Torres González'), @fund_prog),
((SELECT id FROM students WHERE name = 'Valentina Torres González'), @estr_datos),
((SELECT id FROM students WHERE name = 'Valentina Torres González'), @bd_rel),
((SELECT id FROM students WHERE name = 'Andrés Flores Sánchez'), @fund_prog),
((SELECT id FROM students WHERE name = 'Andrés Flores Sánchez'), @web_front),
((SELECT id FROM students WHERE name = 'Andrés Flores Sánchez'), @web_back),
((SELECT id FROM students WHERE name = 'Camila Morales Ramírez'), @fund_prog),
((SELECT id FROM students WHERE name = 'Camila Morales Ramírez'), @estr_datos),
((SELECT id FROM students WHERE name = 'Camila Morales Ramírez'), @web_front),
((SELECT id FROM students WHERE name = 'Miguel Jiménez Torres'), @estr_datos),
((SELECT id FROM students WHERE name = 'Miguel Jiménez Torres'), @ing_soft),
((SELECT id FROM students WHERE name = 'Miguel Jiménez Torres'), @arq_soft),
((SELECT id FROM students WHERE name = 'Isabella Ruiz Flores'), @fund_prog),
((SELECT id FROM students WHERE name = 'Isabella Ruiz Flores'), @web_front),
((SELECT id FROM students WHERE name = 'Isabella Ruiz Flores'), @web_back),
((SELECT id FROM students WHERE name = 'Daniel Castro Morales'), @fund_prog),
((SELECT id FROM students WHERE name = 'Daniel Castro Morales'), @bd_rel),
((SELECT id FROM students WHERE name = 'Daniel Castro Morales'), @admin_bd),
((SELECT id FROM students WHERE name = 'Martina Herrera Jiménez'), @fund_prog),
((SELECT id FROM students WHERE name = 'Martina Herrera Jiménez'), @estr_datos),
((SELECT id FROM students WHERE name = 'Martina Herrera Jiménez'), @web_front),
((SELECT id FROM students WHERE name = 'Santiago Mendoza Ruiz'), @estr_datos),
((SELECT id FROM students WHERE name = 'Santiago Mendoza Ruiz'), @web_front),
((SELECT id FROM students WHERE name = 'Santiago Mendoza Ruiz'), @web_back),
((SELECT id FROM students WHERE name = 'Emilia Vargas Castro'), @fund_prog),
((SELECT id FROM students WHERE name = 'Emilia Vargas Castro'), @estr_datos),
((SELECT id FROM students WHERE name = 'Emilia Vargas Castro'), @bd_rel),
((SELECT id FROM students WHERE name = 'Nicolás Silva Herrera'), @fund_prog),
((SELECT id FROM students WHERE name = 'Nicolás Silva Herrera'), @sistemas),
((SELECT id FROM students WHERE name = 'Nicolás Silva Herrera'), @redes),
((SELECT id FROM students WHERE name = 'Valeria Ortiz Mendoza'), @fund_prog),
((SELECT id FROM students WHERE name = 'Valeria Ortiz Mendoza'), @web_front),
((SELECT id FROM students WHERE name = 'Valeria Ortiz Mendoza'), @bd_rel),
((SELECT id FROM students WHERE name = 'Sebastián Reyes Vargas'), @fund_prog),
((SELECT id FROM students WHERE name = 'Sebastián Reyes Vargas'), @estr_datos),
((SELECT id FROM students WHERE name = 'Sebastián Reyes Vargas'), @web_front),
((SELECT id FROM students WHERE name = 'Lucía Romero Silva'), @estr_datos),
((SELECT id FROM students WHERE name = 'Lucía Romero Silva'), @bd_rel),
((SELECT id FROM students WHERE name = 'Lucía Romero Silva'), @admin_bd),
((SELECT id FROM students WHERE name = 'Mateo Navarro Ortiz'), @fund_prog),
((SELECT id FROM students WHERE name = 'Mateo Navarro Ortiz'), @web_front),
((SELECT id FROM students WHERE name = 'Mateo Navarro Ortiz'), @web_back),
((SELECT id FROM students WHERE name = 'Paula Gutiérrez Reyes'), @fund_prog),
((SELECT id FROM students WHERE name = 'Paula Gutiérrez Reyes'), @estr_datos),
((SELECT id FROM students WHERE name = 'Paula Gutiérrez Reyes'), @bd_rel),
((SELECT id FROM students WHERE name = 'Gabriel Cruz Romero'), @fund_prog),
((SELECT id FROM students WHERE name = 'Gabriel Cruz Romero'), @ing_soft),
((SELECT id FROM students WHERE name = 'Gabriel Cruz Romero'), @arq_soft),
((SELECT id FROM students WHERE name = 'Adriana Díaz Navarro'), @fund_prog),
((SELECT id FROM students WHERE name = 'Adriana Díaz Navarro'), @arq_soft),
((SELECT id FROM students WHERE name = 'Adriana Díaz Navarro'), @web_back);
