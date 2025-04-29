/**
 * @brief Interface representing a student grade
 * 
 * Contains information about a grade assigned to a student for a specific class,
 * including the value, date assigned, and related student/class information.
 */
export interface Grade {
  /**
   * @brief Unique identifier for the grade
   */
  id: number;
  
  /**
   * @brief Name of the class the grade belongs to
   */
  className: string | null;
  
  /**
   * @brief Full name of the student who received the grade
   */
  studentName: string | null;
  
  /**
   * @brief Numeric value of the grade (typically 1-10)
   */
  value: number;
  
  /**
   * @brief Date when the grade was assigned
   */
  dateAssigned: string;
  
  /**
   * @brief ID of the student who received the grade
   */
  studentId: number;
  
  /**
   * @brief ID of the class the grade belongs to (optional for GET operations)
   */
  classId?: number; // This property is missing but needed for creating/updating grades
}