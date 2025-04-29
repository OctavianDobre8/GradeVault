import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Class } from '../../shared/models/class.model';

/**
 * @brief Service for managing class/course operations
 * 
 * This service provides methods for retrieving and managing classes
 * that the current user is associated with.
 */
@Injectable({
  providedIn: 'root',
})
export class ClassesService {
  /**
   * @brief Base API URL for classes endpoints
   */
  private apiUrl = '/api/grades';

  /**
   * @brief Constructor for the classes service
   * 
   * @param http HttpClient for making API requests
   */
  constructor(private http: HttpClient) {}

  /**
   * @brief Gets classes for the currently logged-in user
   * 
   * Retrieves classes where the user is either a teacher or an enrolled student.
   * 
   * @returns Observable<Class[]> Observable that emits the user's classes
   */
  getMyClasses(): Observable<Class[]> {
        console.log(
          'Attempting to fetch classes from: ' + `${this.apiUrl}/my-classes`
        );
    return this.http.get<Class[]>(`${this.apiUrl}/my-classes`);
  }
}