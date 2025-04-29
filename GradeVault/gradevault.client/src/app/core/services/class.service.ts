import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Class } from '../../shared/models/class.model';
import { Student } from '../../shared/models/student.model';

@Injectable({
  providedIn: 'root'
})
export class ClassService {
  private apiUrl = 'api/classes';

  constructor(private http: HttpClient) { }

  getTeacherClasses(): Observable<Class[]> {
    return this.http.get<Class[]>(`${this.apiUrl}/teacher-classes`);
  }

  getStudentsByClass(classId: number): Observable<Student[]> {
    return this.http.get<Student[]>(`${this.apiUrl}/${classId}/students`);
  }

  getAvailableStudents(classId: number): Observable<Student[]> {
    return this.http.get<Student[]>(`${this.apiUrl}/${classId}/available-students`);
  }

  createClass(classData: any): Observable<Class> {
    return this.http.post<Class>(this.apiUrl, classData);
  }

  updateClass(id: number, classData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, classData);
  }

  deleteClass(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  addStudentToClass(classId: number, studentId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${classId}/students/${studentId}`, {});
  }

  removeStudentFromClass(classId: number, studentId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${classId}/students/${studentId}`);
  }
}