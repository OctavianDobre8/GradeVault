import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StudentService {
  private apiUrl = `api/students`;

  constructor(private http: HttpClient) { }

  getStudentsByClass(classId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/class/${classId}`);
  }

  addStudentToClass(classId: string, studentId: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/class/${classId}/student/${studentId}`, {});
  }

  removeStudentFromClass(classId: string, studentId: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/class/${classId}/student/${studentId}`);
  }
}