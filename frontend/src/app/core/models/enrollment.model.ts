export interface Enrollment {
  studentId: number;
  studentName: string;
  subjectId: number;
  subjectName: string;
  professorName: string;
  credits: number;
  status: 'active' | 'completed' | 'cancelled';
  enrolledAt: string;
}

export interface MyEnrollment {
  subjectId: number;
  subjectName: string;
  professorName: string;
  credits: number;
  status?: 'active' | 'completed' | 'cancelled';
  enrolledAt: string;
}

export interface EnrollRequest {
  subjectId: number;
  studentId?: number;
}