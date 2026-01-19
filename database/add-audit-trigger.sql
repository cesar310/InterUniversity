-- Script para agregar trigger de auditoría de configuraciones
-- Ejecutar este script si la base de datos ya está creada

USE student_enrollment_db;

-- Eliminar trigger si existe
DROP TRIGGER IF EXISTS audit_config_update;

DELIMITER //

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

-- Verificar que el trigger se creó correctamente
SHOW TRIGGERS WHERE `Table` = 'system_config';
