import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Grade } from '../../shared/models/grade.model';
import { AuthService } from './auth.service';

/**
 * @brief Service for managing grade operations
 * 
 * This service provides methods for retrieving, creating, updating and deleting grades,
 * with different methods for teacher and student roles.
 */
@Injectable({
  providedIn: 'root'
})
export class GradesService {
  /**
   * @brief Base API URL for grade endpoints
   */
  private apiUrl = 'api/grades';

  /**
   * @brief Constructor for the grades service
   * 
   * @param http HttpClient for making API requests
   * @param authService Authentication service for checking user roles
   */
  constructor(private http: HttpClient,
              private authService: AuthService
  ) { }

  /**
   * @brief Gets grades for the currently logged-in student
   * 
   * @returns Observable<Grade[]> Observable that emits the student's grades
   */
  getMyGrades(): Observable<Grade[]> {
    return this.http.get<Grade[]>(`${this.apiUrl}/my-grades`);
  }

  /**
   * @brief Gets grades for a specific class
   * 
   * Uses different endpoints based on whether the current user is a teacher or student,
   * as teachers can see all grades in a class while students only see their own.
   * 
   * @param classId ID of the class to get grades for
   * @returns Observable<Grade[]> Observable that emits the grades for the class
   */
  getGradesByClass(classId: number): Observable<Grade[]> {
    // Determine if user is a student or teacher
    const user = this.authService.currentUserValue;
    if (user?.role === 'Teacher') {
      return this.http.get<Grade[]>(`${this.apiUrl}/class/${classId}/teacher`);
    } else {
      return this.http.get<Grade[]>(`${this.apiUrl}/class/${classId}/student`);
    }
  }

  /**
   * @brief Creates a new grade
   * 
   * @param grade Grade data containing studentId, classId, and value
   * @returns Observable<any> Observable that completes when grade is created
   */
  createGrade(grade: { studentId: number; classId: number; value: number }): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}`, grade);
  }
  
  /**
   * @brief Updates an existing grade
   * 
   * @param id ID of the grade to update
   * @param grade Object containing the new grade value
   * @returns Observable<any> Observable that completes when grade is updated
   */
  updateGrade(id: number, grade: { value: number }): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, grade);
  }

  /**
   * @brief Deletes a grade
   * 
   * @param id ID of the grade to delete
   * @returns Observable<any> Observable that completes when grade is deleted
   */
  deleteGrade(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  /**
   * @brief Uploads multiple grades via CSV file
   * 
   * @param formData FormData object containing the CSV file
   * @returns Observable<any> Observable that completes when grades are uploaded
   */
  bulkUploadGrades(formData: FormData): Observable<any> {
    return this.http.post(`${this.apiUrl}/bulk-upload`, formData);
  }
}