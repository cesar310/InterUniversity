// Constantes globales del sistema

export const ROLES = {
  ADMIN: 'admin',
  STUDENT: 'student'
} as const;

export const ENROLLMENT_STATUS = {
  ACTIVE: 'active',
  INACTIVE: 'inactive'
} as const;

export const SUBJECT_STATUS = {
  ACTIVE: 'active',
  INACTIVE: 'inactive'
} as const;

/**
 * Claves de configuración del sistema en la base de datos
 * Estas claves se usan para obtener valores desde el servicio SystemConfigService
 */
export const CONFIG_KEYS = {
  MAX_SUBJECTS_PER_STUDENT: 'max_subjects_per_student',
  MIN_SUBJECTS_PER_STUDENT: 'min_subjects_per_student',
  DEFAULT_SUBJECT_CREDITS: 'default_subject_credits',
  MAX_SUBJECTS_PER_PROFESSOR: 'max_subjects_per_professor',
  ALLOW_SAME_PROFESSOR: 'allow_same_professor',
  SYSTEM_NAME: 'system_name',
  ACADEMIC_PERIOD: 'academic_period',
  ENROLLMENT_OPEN: 'enrollment_open'
} as const;