export interface Professor {
  id: number;
  name: string;
  specialization: string;
  email?: string;
  phone?: string;
  isActive: boolean;
  activeSubjectsCount: number;
  totalSubjects?: number;
  maxSubjects?: number; // Optional on interface, but used in Logic
  maxAllowed?: number; // From API JSON
}

export interface CreateProfessorRequest {
  name: string;
  specialization: string;
  email?: string;
  phone?: string;
}

export interface UpdateProfessorRequest {
  professorId: number;
  name: string;
  specialization: string;
  email: string;
  phone: string;
  isActive: boolean;
}

export interface PagedProfessorsResponse {
  success: boolean;
  data: {
    professors: Professor[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
  };
  message: string | null;
}

