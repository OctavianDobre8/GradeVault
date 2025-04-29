export interface Grade {
  id: number;
  className: string | null;
  studentName: string | null;
  value: number;
  dateAssigned: string;
  studentId: number;
  classId?: number; // This property is missing but needed for creating/updating grades
}