import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Grade } from '../../shared/models/grade.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class GradesService {
  private apiUrl = 'api/grades';

  constructor(private http: HttpClient,
              private authService: AuthService
  ) { }

  // Student methods
  getMyGrades(): Observable<Grade[]> {
    return this.http.get<Grade[]>(`${this.apiUrl}/my-grades`);
  }

  // Teacher methods
  getGradesByClass(classId: number): Observable<Grade[]> {
    // Determine if user is a student or teacher
    const user = this.authService.currentUserValue;
    if (user?.role === 'Teacher') {
      return this.http.get<Grade[]>(`${this.apiUrl}/class/${classId}/teacher`);
    } else {
      return this.http.get<Grade[]>(`${this.apiUrl}/class/${classId}/student`);
    }
  }

  createGrade(grade: { studentId: number; classId: number; value: number }): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}`, grade);
  }
  
  updateGrade(id: number, grade: { value: number }): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, grade);
  }
  deleteGrade(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  bulkUploadGrades(formData: FormData): Observable<any> {
    return this.http.post(`${this.apiUrl}/bulk-upload`, formData);
  }
}