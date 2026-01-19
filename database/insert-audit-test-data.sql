-- Script para insertar datos de prueba en el historial de auditoría
-- Esto es solo para probar que el frontend funciona correctamente

USE student_enrollment_db;

-- Obtener el ID del usuario administrador
SET @admin_id = (SELECT id FROM users WHERE email = 'admin@sistema.com' LIMIT 1);

-- Insertar algunos cambios de prueba
INSERT INTO config_audit_log (config_key, old_value, new_value, changed_by, changed_at) VALUES
('max_subjects_per_student', '2', '3', @admin_id, DATE_SUB(NOW(), INTERVAL 5 DAY)),
('enrollment_open', 'false', 'true', @admin_id, DATE_SUB(NOW(), INTERVAL 3 DAY)),
('allow_same_professor', 'true', 'false', @admin_id, DATE_SUB(NOW(), INTERVAL 2 DAY)),
('max_subjects_per_professor', '2', '3', @admin_id, DATE_SUB(NOW(), INTERVAL 1 DAY)),
('academic_period', '2025-2', '2026-1', @admin_id, DATE_SUB(NOW(), INTERVAL 12 HOUR)),
('allow_same_professor', 'false', 'true', @admin_id, NOW());

-- Verificar los datos insertados
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
