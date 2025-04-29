/**
 * @brief Interface representing an authenticated user
 * 
 * Contains essential user information and authentication details
 * returned by the server after successful login or registration.
 */
export interface User{
  /**
   * @brief Unique identifier for the user
   */
  id:string;
  
  /**
   * @brief User's email address (used as username)
   */
  email:string;
  
  /**
   * @brief User's first name
   */
  firstName:string;
  
  /**
   * @brief User's last name
   */
  lastName:string;
  
  /**
   * @brief User's assigned role (Teacher or Student)
   */
  role:string;
  
  /**
   * @brief JWT authentication token for API requests
   */
  token:string;
}