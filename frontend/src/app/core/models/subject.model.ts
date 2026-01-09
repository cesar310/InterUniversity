export interface Subject {
  id: number;
  name: string;
  description?: string;
  credits: number;
  professorId: number;
  professorName: string;
  isActive: boolean;
  enrolledStudents: number;
}

export interface CreateSubjectRequest {
  name: string;
  description?: string;
  credits: number;
  professorId: number;
}

export interface UpdateSubjectRequest {
  subjectId: number;
  name: string;
  description?: string;
  credits: number;
  professorId: number;
  isActive: boolean;
}

export interface AcademicOffer {
  subjectId: number;
  subject: string;
  description?: string;
  credits: number;
  professor: string;
  specialization?: string;
  professorEmail?: string;
  enrolledStudents: number;
  available: boolean;
}

export interface PagedSubjectsResponse {
  subjects: Subject[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}