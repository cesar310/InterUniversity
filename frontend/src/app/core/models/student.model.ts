export interface Student {
  id: number;
  userId: number;
  name: string;
  studentCode: string;
  email: string;
  isActive: boolean;
  createdAt: string;
}

export interface RegisterStudentRequest {
  name: string;
  email: string;
}

export interface UpdateStudentRequest {
  studentId: number;
  name: string;
  studentCode: string;
}

export interface RegisterStudentResponse {
  studentId: number;
  userId: number;
  studentCode: string;
  email: string;
  temporaryPassword: string;
}

export interface PagedStudentsResponse {
  data: Student[];
  pagination: {
    page: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
  };
}

export interface StudentWithEnrollments {
  studentId: number;
  studentName: string;
  studentCode: string;
  email: string;
  isActive: boolean;
  enrolledSubjects: number;
  maxAllowed: number;
  subjects: string | null;
}