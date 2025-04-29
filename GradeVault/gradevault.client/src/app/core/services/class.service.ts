import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Class } from '../../shared/models/class.model';
import { Student } from '../../shared/models/student.model';

/**
 * @brief Service for managing class/course operations for teachers
 * 
 * This service provides methods for retrieving, creating, updating and deleting classes,
 * as well as managing student enrollments within classes.
 */
@Injectable({
  providedIn: 'root'
})
export class ClassService {
  /**
   * @brief Base API URL for class endpoints
   */
  private apiUrl = 'api/classes';

  /**
   * @brief Constructor for the class service
   * 
   * @param http HttpClient for making API requests
   */
  constructor(private http: HttpClient) { }

  /**
   * @brief Gets all classes taught by the current teacher
   * 
   * @returns Observable<Class[]> Observable that emits the teacher's classes
   */
  getTeacherClasses(): Observable<Class[]> {
    return this.http.get<Class[]>(`${this.apiUrl}/teacher-classes`);
  }

  /**
   * @brief Gets all students enrolled in a specific class
   * 
   * @param classId ID of the class to get students for
   * @returns Observable<Student[]> Observable that emits the enrolled students
   */
  getStudentsByClass(classId: number): Observable<Student[]> {
    return this.http.get<Student[]>(`${this.apiUrl}/${classId}/students`);
  }

  /**
   * @brief Gets all students available to be added to a class
   * 
   * Returns students who are not currently enrolled in the specified class.
   * 
   * @param classId ID of the class to get available students for
   * @returns Observable<Student[]> Observable that emits the available students
   */
  getAvailableStudents(classId: number): Observable<Student[]> {
    return this.http.get<Student[]>(`${this.apiUrl}/${classId}/available-students`);
  }

  /**
   * @brief Creates a new class
   * 
   * @param classData Data for the new class including name, description, and room number
   * @returns Observable<Class> Observable that emits the created class
   */
  createClass(classData: any): Observable<Class> {
    return this.http.post<Class>(this.apiUrl, classData);
  }

  /**
   * @brief Updates an existing class
   * 
   * @param id ID of the class to update
   * @param classData Updated class data
   * @returns Observable<any> Observable that completes when class is updated
   */
  updateClass(id: number, classData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, classData);
  }

  /**
   * @brief Deletes a class
   * 
   * @param id ID of the class to delete
   * @returns Observable<any> Observable that completes when class is deleted
   */
  deleteClass(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  /**
   * @brief Enrolls a student in a class
   * 
   * @param classId ID of the class to add student to
   * @param studentId ID of the student to be enrolled
   * @returns Observable<any> Observable that completes when student is added
   */
  addStudentToClass(classId: number, studentId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${classId}/students/${studentId}`, {});
  }

  /**
   * @brief Removes a student from a class
   * 
   * @param classId ID of the class to remove student from
   * @param studentId ID of the student to be removed
   * @returns Observable<any> Observable that completes when student is removed
   */
  removeStudentFromClass(classId: number, studentId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${classId}/students/${studentId}`);
  }
}