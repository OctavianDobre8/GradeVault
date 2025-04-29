/**
 * @brief Interface representing a class/course
 * 
 * Contains essential information about a class including its name,
 * description, room number, and optionally the teacher's name.
 */
export interface Class {
  /**
   * @brief Unique identifier for the class
   */
  id: number;
  
  /**
   * @brief Name of the class
   */
  name: string;
  
  /**
   * @brief Description of the class content or objectives
   */
  description: string;
  
  /**
   * @brief Physical room number where the class is held
   */
  roomNumber: string;
  
  /**
   * @brief Name of the teacher instructing this class (optional)
   */
  teacherName?: string;  
}