/**
 * @brief Interface representing a student user
 * 
 * Contains essential information about a student user,
 * typically used for displaying student lists and enrollment management.
 */
export interface Student {
  /**
   * @brief Unique identifier for the student
   */
  id: number;
  
  /**
   * @brief Student's first name
   */
  firstName: string;
  
  /**
   * @brief Student's last name
   */
  lastName: string;
  
  /**
   * @brief Student's email address
   */
  email: string;
}